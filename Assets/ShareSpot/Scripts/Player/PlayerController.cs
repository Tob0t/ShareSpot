using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// PlayerController is responsible for updating the player position and handle all the actions performed by the player.
/// </summary>
public class PlayerController : NetworkBehaviour{
	#region [Static fields]
	public static int ShowChallengesRate = 2;	///< Static var for inidicating how many pickups are needed until a challenge is shown.
	#endregion

	#region [Public fields]
	[SyncVar]
	public Quaternion Rotation;	///< SyncVar for the Rotation of the player object.
	[SyncVar]
	public Vector3 Position;	///< SyncVar for the Position of the player object.
	[SyncVar]
	public bool HasControllingPlayer;	///< SyncVar for indicating if the player is controlled by a client.
	[SyncVar]
	public string PlayerName;	///< SyncVar for the name of the player.
	[SyncVar]
	public int ConnectionId;	///< SyncVar for the unique connectionId to the server.
	[SyncVar]
	public int CollectedPickups;	///< SyncVar for the amount of collected Pickups (only used for Game mode).
	[SyncVar]
	public int SharingMode;	///< SyncVar for the sharing mode which is assigned to the player.

	public GameObject ControllingPlayer;	///< The reference to the TrackedPlayer who is forwarding the position data.
	public GameObject FilePrefab;	///< The prefab of a file object.
	public Transform FileSpawn;	///< The location of the the spawn of a file.
	public float ShootingSpeed = 0.01f;	///< The shoothing speed of a file.
	public int PlayerSpeed;	///< The speed of the player when moving manually (only for debugging reasons).

	public GameObject GameManager; ///< Reference to the GameManger.

	#endregion

	#region [Private fields]
	private IInputMode inputMode;	///< The input mode depending wheter its mobile or computer, TODO: necessary?
	// TODO implement LERP ?
	//private Vector3 _previousPos;	///< The previous position of the object.
	//private float distance;	///< The distance traveled since the last frame.
	//private float _timePassed;	///< The time passed since the last frame.
	//private bool _staticPos;
	private bool initialSetup;	///< Boolean indicating whether the initial setup has already been done.
	private UserInterfaceController _userInterfaceController;	///< A reference to the player user interface
	private GameObject ARCamera;	///< A reference to the ARCamera object

	#endregion

	// Use this for initialization
	void Start () {
		CollectedPickups = 0;
		if (isServer) {
			// Disable the object as long as it is not assigned
			transform.GetComponent<MeshRenderer>().enabled = false;

			// create Reference to the game manager
			GameManager = GameObject.FindGameObjectWithTag ("GameManager");

			// Set the name of the transform
			transform.name = "Client #" + connectionToClient.connectionId;

			// only add the connected client and button if there are not more than @param MaxClients
			if (connectionToClient.connectionId <= Admin.Instance.MaxClients) {
				Admin.Instance.ConnectedClients [connectionToClient.connectionId] = gameObject;
				ConnectionId = connectionToClient.connectionId;
			}
		}
	}

	/// <summary>
	/// Raises the start local player event.
	/// </summary>
	public override void OnStartLocalPlayer(){

		// Find the user interface and the AR Camera object 
		_userInterfaceController = GameObject.FindGameObjectWithTag("PlayerUserInterface").GetComponent<UserInterfaceController>();
		ARCamera = GameObject.FindGameObjectWithTag ("ARCamera");

		#if UNITY_ANDROID
			inputMode = new InputMobile ();
		#else
			inputMode = new InputComputer ();
		#endif
		// Disable the MeshRenderer
		transform.GetComponent<MeshRenderer>().enabled = false;

		// Create reference to Userinterface controller
		_userInterfaceController.AddPlayerController(this.gameObject);
	}

	// Update is called once per frame
	void Update () {
		// Set PlayerName from Textfield
		this.GetComponentInChildren<TextMesh>().text = PlayerName;

		// only called if there is already a connection to a tracked player (set by admin)
		if (HasControllingPlayer) {
			// show player layover
			transform.GetComponent<MeshRenderer>().enabled = true;

			#region Client
			// only called if its the localPlayer (Client)
			if (isLocalPlayer) {

				// Check if initial setup is already done
				if (!initialSetup) {
					RecalibrateDevice();

					// Create Initial Setup
					_userInterfaceController.InitialSetup();

/*****************************************************************************************************************************/
					// Temp set User to 0,0,0
					//transform.position = new Vector3(500,185,500);
/*****************************************************************************************************************************/
					// set to true
					initialSetup = true;
				}

				// Update position and rotation

				// allow rotation by keyboard for non android runtimes
				if (Application.platform != RuntimePlatform.Android) {
					
					float x = inputMode.Turn () * Time.deltaTime * 150.0f;
					transform.Rotate (0, x, 0);
					ARCamera.transform.rotation = transform.rotation;
/*****************************************************************************************************************************/					
					// TEMP to be able to move
					//float z = inputMode.Move() * Time.deltaTime * PlayerSpeed;
					//transform.Translate(0,0,z);
/*****************************************************************************************************************************/
				} /*else{*/
					// update ARCamera and player position from the syncVar obtained by the trackingClient from the server
					transform.position = Position;
				//}
				ARCamera.transform.position = transform.Find("Hand").position;

				// update rotation obtained from the gyroclient of the ARCamera
				transform.rotation = ARCamera.transform.rotation;
			}
			#endregion
		}

		#region Server
		// only called if server instance
		if (isServer) {

			// update position of the player object (obtained by the tracking system)
			if (ControllingPlayer != null) {
				transform.position = ControllingPlayer.transform.position;
				Position = transform.position;
			}
		}
		#endregion

	}
		
	/// <summary>
	/// Recalibrates the device facing the oposite wall.
	/// </summary>
	public void RecalibrateDevice(){
		ARCamera.GetComponent<GyroController> ().Recalibrate (0);
	}

	/// <summary>
	/// Command executed on the server for shooting a SharedFile.
	/// </summary>
	/// <param name="force">Force vector for inidicating the direction.</param>
	[Command]
	public void CmdShootFile(Vector3 force){
		// Create the SharedFile from the file Prefab
		var sharedFile = (GameObject)Instantiate(
			FilePrefab,
			FileSpawn.position,
			FileSpawn.rotation);

		Debug.Log("Force Vector: "+force.ToString());

		// Transform the force in a direction first and set is as velocity
		sharedFile.GetComponent<Rigidbody> ().velocity = transform.TransformDirection (force*ShootingSpeed);
		sharedFile.GetComponent<SharedFile> ().SourceId = connectionToClient.connectionId;

		// Spawn the file on the Clients
		NetworkServer.Spawn(sharedFile);

		// Destroy the file after 30 seconds
		Destroy(sharedFile, 30.0f);
	}

	/// <summary>
	/// Command executed on the server for changing the name.
	/// <param name="newName">A new name.</param>
	/// </summary>
	[Command]
	public void CmdChangeName(string newName){
		PlayerName = newName;
		Admin.Instance.ClientButtons [ConnectionId].GetComponentInChildren<Text> ().text = newName;
	}

	/// <summary>
	/// Command executed on the server for moving the pickup to a new random location.
	/// <param name="pickup">The collected pickup.</param>
	/// </summary>
	[Command]
	public void CmdPickupCollected(GameObject pickup){
		Debug.Log ("Move Pickup");
		CollectedPickups++;
		if (CollectedPickups % ShowChallengesRate == 0) {
			string description = GameManager.GetComponent<GameController> ().CreateChallenge (gameObject);
			RpcShowChallenge (description);
		}
		// Get valid area for finding random location
		Vector3 randomLocation = new Vector3 (Random.Range (GlobalHelper.GetPickupAreaMinValues().x, GlobalHelper.GetPickupAreaMaxValues().x), Random.Range (GlobalHelper.GetPickupAreaMinValues().y, GlobalHelper.GetPickupAreaMaxValues().y), Random.Range (GlobalHelper.GetPickupAreaMinValues().z, GlobalHelper.GetPickupAreaMaxValues().z));
		pickup.transform.position = randomLocation;
	}

	/// <summary>
	/// Command executed on the server for receiving a file.
	/// <param name="file">The received file.</param>
	/// <param name="senderId">The player id of the sender.</param>
	/// <param name="receiverId">The player id of the receiver.</param>
	/// </summary>
	// TODO Show file (change to SharedFile instead of GameObject?
	[Command]
	public void CmdReceiveFile(GameObject file, int senderId, int receiverId){
		Debug.Log ("Sender ID :" + senderId);
		Debug.Log ("ReceiverId ID :" + receiverId);
		bool challengeState = false;

		// Check if there is a game currently running
		if(GameManager.GetComponent<GameController> ().isGameActive){
			challengeState = GameManager.GetComponent<GameController> ().VerifyChallenge (senderId, receiverId);
		} else{
			// if the game is not active
			// Call the rpc receiving file on the receiving player
			Admin.Instance.ConnectedClients [receiverId].GetComponent<PlayerController> ().RpcReceiveFile (file);
		}
		// Forward to sending client that the file was sent
		Admin.Instance.ConnectedClients [senderId].GetComponent<PlayerController> ().RpcFileSent (challengeState);
	}
		
	/// <summary>
	/// ClientRpc executed on the client for feedback of a sent file.
	/// </summary>
	/// <param name="challengeState">Indicates whether the challenge is succesful or failed.</param>
	[ClientRpc]
	public void RpcFileSent(bool challengeState){
		if (isLocalPlayer) {
			// Check if there is a game currently running
			if(_userInterfaceController.GameManager.GetComponent<GameController> ().isGameActive){
				// Adpat all necessary panels
				_userInterfaceController.AdaptPanels (challengeState);
			} else {
				// Just activate the success panel
				_userInterfaceController.ShowSuccessPanel ();
			}
		}
	}

	/// <summary>
	/// ClientRpc executed on the client for displaying the new challenge.
	/// <param name="description">The Description of the challenge.</param>
	/// </summary>
	[ClientRpc]
	public void RpcShowChallenge(string description){
		if (isLocalPlayer) {
			// Display the new challenge on the clients UI
			_userInterfaceController.ShowNewChallenge (description);

			// Enable the sharing mode panel which is assigned to the client
			_userInterfaceController.TogglePlayersSharingModePanel(true);
		}
	}

	/// <summary>
	/// ClientRpc to Show only the own pickups when collecting (called only once when game is started by GameController).
	/// </summary>
	[ClientRpc]
	public void RpcStartGame(){
		if (isLocalPlayer) {
			// Disable all current panels
			_userInterfaceController.DisableAllGamePanels ();

			// Enable Game start panel
			_userInterfaceController.ShowGameStartPanel();

			// Loop through all pickups and only activate the right one
			foreach (GameObject pickup  in GameObject.FindGameObjectsWithTag("Pickup")) {
				if (pickup.GetComponent<PickupController> ().ValidForConnectionId == ConnectionId) {
					pickup.SetActive (true);
				} else {
					pickup.SetActive (false);
				}
			}
		}
	}

	/// <summary>
	/// Show trigger when local player receives physically a file (called by localPlayer).
	/// For game not needed
	/// <param name="file">The file to receive.</param>
	/// </summary>
	public void ReceiveFile(GameObject file){
		if (isLocalPlayer) {

			CmdReceiveFile(file, file.GetComponent<SharedFile>().SourceId, ConnectionId);

			// TODO: not needed right?
			// Show incoming file on the local client on the receiver only if the game is not active
			/*if (!MyNetworkManager.Instance.isGameActive) {
				PlayerUserInterface.GetComponent<UserInterfaceController> ().ShowIncomingFile (file);
			}*/
		}
	}
		
	/// <summary>
	/// When the player object is colliding with another object.
	/// <param name="other">The collider of the object which is colliding with the player object.</param>
	/// </summary>
	void OnTriggerEnter(Collider other){
		if (isLocalPlayer) {
			// if it collides with a Pickup item
			if (other.gameObject.CompareTag ("Pickup")) {
				Debug.Log ("Pickup collected");
				CmdPickupCollected (other.gameObject);
			} else {
				Debug.Log (other.ToString () + " collided with Player");
			}
		}
	}

	/// <summary>
	/// Called when the player object is selected.
	/// </summary>
	void OnMouseDown(){
		Debug.Log ("Mouse Down on " + gameObject);
		if (isServer) {
			Admin.Instance.CurrentTrackedPlayer = gameObject;
			StartCoroutine (SelectPlayer());
		}
	}

	/// <summary>
	/// Coroutine to change the color for 4 seconds.
	/// </summary>
	IEnumerator SelectPlayer(){
		Color current = gameObject.GetComponent<MeshRenderer> ().material.color;
		gameObject.GetComponent<MeshRenderer> ().material.color = Color.green;
		yield return new WaitForSeconds(4f);
		gameObject.GetComponent<MeshRenderer> ().material.color = current;
	}
	

	/// <summary>
	/// Called when a file is received and triggers an action on the receiving player.
	/// Only available if there is no game running.
	/// </summary>
	/// <param name="file">The file which was sent.</param>
	[ClientRpc]
	public void RpcReceiveFile(GameObject file){
		if (isLocalPlayer) {
			_userInterfaceController.ShowIncomingFile (file);
		}
	}


}
