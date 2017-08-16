using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// PlayerController is responsible for updating the player position and handle all the actions performed by the player.
/// </summary>
public class PlayerController : NetworkBehaviour{
	#region [Public fields]
	[SyncVar]
	public Quaternion PlayerHandRotation;	///< SyncVar for the Rotation of the player object.
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
	public int PlayerSpeed;	///< The speed of the player when moving manually (only for debugging reasons).

	public GameObject GameManager; ///< Reference to the GameManger.

	public Transform DeviceLocation; ///< The location of the device relative to the player object.
	public Transform PlayerHand;	///< The current location of the player hand.
	public Vector3 PlayerHandInitialPos; ///< The initial position of the hand.

	public Text TestCompass;

	#endregion

	#region [Private fields]
	private IInputMode _inputMode;	///< The input mode depending wheter its mobile or computer
	// TODO implement LERP ?
	//private Vector3 _previousPos;	///< The previous position of the object.
	//private float distance;	///< The distance traveled since the last frame.
	//private float _timePassed;	///< The time passed since the last frame.
	//private bool _staticPos;
	private bool _initialSetup;	///< Boolean indicating whether the initial setup has already been done.
	private UserInterfaceController _userInterfaceController;	///< A reference to the player user interface
	private GameObject _ARCamera;	///< A reference to the ARCamera object
	private GameObject _localPickup; ///< Reference to the local pickup to collect for game mode

	#endregion

	// Use this for initialization
	void Start () {
		CollectedPickups = 0;
		if (isServer) {
			// Disable the object as long as it is not assigned
			transform.GetComponent<MeshRenderer>().enabled = false;

			// create Reference to the game manager
			GameManager = GameObject.FindGameObjectWithTag ("GameManager");

			// Set the name
			transform.name = "Client #" + connectionToClient.connectionId;
			PlayerName = "Client " + connectionToClient.connectionId;

			// only add the connected client and button if there are not more than @param MaxClients
			if (connectionToClient.connectionId <= GlobalConfig.MaxClients) {
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
		_ARCamera = GameObject.FindGameObjectWithTag ("ARCamera");

		#if UNITY_ANDROID
			_inputMode = new InputMobile ();
		#else
			_inputMode = new InputComputer ();
		#endif
		// Disable the MeshRenderer
		transform.GetComponent<MeshRenderer>().enabled = false;

		// Create reference to Userinterface controller
		_userInterfaceController.AddPlayerController(this.gameObject);

		// Get initial position of PlayerHand
		PlayerHandInitialPos = PlayerHand.transform.localPosition;
	}

	// Update is called once per frame
	void Update () {
		

		// only called if there is already a connection to a tracked player (set by admin)
		if (HasControllingPlayer) {
			// show player layover
			transform.GetComponent<MeshRenderer>().enabled = true;

			// Set PlayerName from Textfield
			this.GetComponentInChildren<TextMesh>().text = PlayerName;

			#region Client
			// only called if its the localPlayer (Client)
			if (isLocalPlayer) {

				// Check if initial setup is already done
				if (!_initialSetup) {
					RecalibrateDevice(0);

					// Create Initial Setup
					_userInterfaceController.InitialSetup();

/*****************************************************************************************************************************/
					if (Application.platform != RuntimePlatform.Android) {
						// Temp set User to 0,0,0
						transform.position = new Vector3(500,270,500);
					}
/*****************************************************************************************************************************/
					// set to true
					_initialSetup = true;
				}

				// Update position and rotation

				// allow rotation by keyboard for non android runtimes
				if (Application.platform != RuntimePlatform.Android) {
					
					float x = _inputMode.Turn () * Time.deltaTime * 150.0f;
					// TODO: Delete?
					transform.Rotate (0, x, 0);
					//DeviceLocation.transform.Rotate (0, x, 0);
					_ARCamera.transform.rotation = transform.rotation;
/*****************************************************************************************************************************/					
					// TEMP to be able to move
					//float z = _inputMode.Move() * Time.deltaTime * PlayerSpeed;
					//transform.Translate(0,0,z);
/*****************************************************************************************************************************/
				} //else{
					// update player position from the syncVar obtained by the trackingClient from the server
					transform.position = Position;
				//}
				// Set position of AR Camera to device location
				_ARCamera.transform.position = DeviceLocation.transform.position;

/*****************************************************************************************************************************/
				// Alternative approach
				// update rotation obtained from the gyroclient of the ARCamera
				//transform.rotation = _ARCamera.transform.rotation;
/*****************************************************************************************************************************/

				// Update the position of the device depending on the type of rotation
				Vector3 _ARCameraRotationEuler = _ARCamera.transform.rotation.eulerAngles;

				// Movements in x and z direction are changing the device orientation
				DeviceLocation.transform.rotation = Quaternion.Euler(_ARCameraRotationEuler.x,0,_ARCameraRotationEuler.z);

				// Movements in y direction are changing the players orientation (x and z direction are not changed)
				transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,_ARCameraRotationEuler.y,transform.rotation.eulerAngles.z);

				// Send the new rotation of the device to the server 
				// (Not necessary to avoid too much network traffic)
				//CmdSetDeviceRotation(DeviceLocation.transform.rotation);

			}
			#endregion
		}

		#region Server
		// only called if server instance
		if (isServer) {

			// update position of the player object (obtained by the tracking system from TrackedPlayer)
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
	/// <param name="angle">Angle in degrees.</param>
	public void RecalibrateDevice(int angle){
		_ARCamera.GetComponent<GyroController> ().Recalibrate (angle);
		Vibration.Vibrate (500);
	}


	#region Server Commands

	/// <summary>
	/// Command executed on the server to update the rotation of the device
	/// </summary>
	/// <param name="newRotation">New rotation.</param>
	[Command]
	public void CmdSetDeviceRotation(Quaternion newRotation){
		//DeviceLocation.transform.rotation = newRotation;
		// only update the local rotation
		DeviceLocation.transform.localRotation = newRotation;
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

		//Debug.Log("Force Vector: "+force.ToString());

		// Transform the force in a direction first and set is as velocity
		sharedFile.GetComponent<Rigidbody> ().velocity = FileSpawn.transform.TransformDirection (force*GlobalConfig.ShootingSpeed);
		sharedFile.GetComponent<SharedFile> ().SourceId = connectionToClient.connectionId;

		// Spawn the file on the Clients
		NetworkServer.Spawn(sharedFile);

		// Destroy the file after 20 seconds
		Destroy(sharedFile, 20.0f);
	}

	/// <summary>
	/// Command executed on the server for changing the name.
	/// </summary>
	/// <param name="newName">A new name.</param>
	[Command]
	public void CmdChangeName(string newName){
		PlayerName = newName;
		Admin.Instance.ClientButtons [ConnectionId].GetComponentInChildren<Text> ().text = newName;
	}

	/// <summary>
	/// Command executed on the server for moving the pickup to a new random location.
	/// </summary>
	/// <param name="pickup">The collected pickup.</param>
	[Command]
	public void CmdPickupCollected(GameObject pickup){
		Debug.Log ("Pickup Collected");

		// Increase collected pickups
		CollectedPickups++;

		// Update game scores and only show new pickup if game is not finished yet
		if (GameManager.GetComponent<GameController> ().UpdateGameScores ()) {

			// Call rpc to update the players points
			RpcUpdatePlayerPoints (CollectedPickups, pickup);

			// Get valid area for finding random location
			Vector3 randomLocation = new Vector3 (Random.Range (GlobalConfig.GetPickupAreaMinValues ().x, GlobalConfig.GetPickupAreaMaxValues ().x), Random.Range (GlobalConfig.GetPickupAreaMinValues ().y, GlobalConfig.GetPickupAreaMaxValues ().y), Random.Range (GlobalConfig.GetPickupAreaMinValues ().z, GlobalConfig.GetPickupAreaMaxValues ().z));
			pickup.transform.position = randomLocation;

			if (CollectedPickups % GlobalConfig.ShowChallengesRate == 0) {
				string description = GameManager.GetComponent<GameController> ().CreateChallenge (gameObject);
				RpcShowChallenge (description, pickup);
				_localPickup = pickup;
				_localPickup.SetActive (false);
			}
		}
	}

	/// <summary>
	/// Command executed on the server for receiving a file.
	/// </summary>
	/// <param name="file">The received file.</param>
	/// <param name="senderId">The player id of the sender.</param>
	/// <param name="receiverId">The player id of the receiver.</param>
	// TODO Show file (change to SharedFile instead of GameObject?)
	[Command]
	public void CmdReceiveFile(GameObject file, int senderId, int receiverId){
		bool challengeState = false;

		// Check if there is a game currently running
		if(GameManager.GetComponent<GameController> ().isGameActive){
			challengeState = GameManager.GetComponent<GameController> ().VerifyChallenge (senderId, receiverId);
			if (_localPickup != null) {
				_localPickup.SetActive (challengeState);
			}
		} else{
			// if the game is not active
			// Call the rpc receiving file on the receiving player
			Admin.Instance.ConnectedClients [receiverId].GetComponent<PlayerController> ().RpcReceiveFile (file);
		}
		// Forward to sending client that the file was sent
		Admin.Instance.ConnectedClients [senderId].GetComponent<PlayerController> ().RpcFileSent (challengeState);
	}

	/// <summary>
	/// Command to verify if the right client is receiving the file.
	/// </summary>
	/// <param name="file">File which was sent.</param>
	/// <param name="receiverId">Identifier of the receiving client.</param>
	[Command]
	public void CmdVerifyReceiver(GameObject file, int receiverId){
		// extract the name of the receiving client
		string name = Admin.Instance.ConnectedClients [receiverId].GetComponent<PlayerController> ().PlayerName;

		// Call the rpc on the sender verifying if it is the right receiver
		Admin.Instance.ConnectedClients [file.GetComponent<SharedFile>().SourceId].GetComponent<PlayerController> ().RpcVerifyReceiver (file, receiverId, name);
	}

	/// <summary>
	/// Command to increase the error rate for the local sharing mode.
	/// </summary>
	/// <param name="senderId">Identifier of the sending client.</param>
	[Command]
	public void CmdIncreaseError(int senderId){
		GameManager.GetComponent<GameController> ().IncreaseErrorRate (senderId);
	}

	#endregion

	#region Client Rpcs
		
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
				// Set Pickup active again
				_localPickup.SetActive (challengeState);
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
	public void RpcShowChallenge(string description, GameObject pickup){
		if (isLocalPlayer) {
			// Display the new challenge on the clients UI
			_userInterfaceController.ShowNewChallenge (description);

			// Enable the sharing mode panel which is assigned to the client
			_userInterfaceController.TogglePlayersSharingModePanel(true);

			// assign local pickup and set it to inactive
			_localPickup = pickup;
			_localPickup.SetActive (false);
		}
	}

	/// <summary>
	/// ClientRpc to to show the game sharing mode on the client.
	/// </summary>
	/// <param name="sharingMode">Sharing mode.</param>
	[ClientRpc]
	public void RpcAssignGameSharingMode(int sharingMode){
		if (isLocalPlayer) {
			_userInterfaceController.ShowGameSharingMode (sharingMode);
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

			// Enable game start panel
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
	/// ClientTpc to Stop the Game and reset the parameters
	/// </summary>
	[ClientRpc]
	public void RpcStopGame(){
		if (isLocalPlayer) {
			// Disable all current panels
			_userInterfaceController.DisableAllGamePanels ();

			// Enable game stop panel
			_userInterfaceController.ShowGameStopPanel();

			// Show sharing mode panel
			_userInterfaceController.ToggleSharingModePanel (true);

			// Reset player points to 0
			_userInterfaceController.UpdatePlayerPoints (0);
		}
	}

	/// <summary>
	/// ClientRpc to verify the receiver.
	/// </summary>
	/// <param name="file">File which was sent.</param>
	/// <param name="receiverId">Identifier of the receiving client.</param>
	/// <param name="playerName">Player name of the receiving client.</param>
	[ClientRpc]
	public void RpcVerifyReceiver (GameObject file, int receiverId, string playerName){
		// Show the verify Panel on the local Player
		if(isLocalPlayer){
			_userInterfaceController.ShowVerifyReceiverPanel (file, receiverId, playerName);
		}
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

	/// <summary>
	/// Called to update the player points.
	/// </summary>
	/// <param name="CollectedPickups">Collected pickups.</param>
	/// <param name="pickup">The collected pickup.</param>
	[ClientRpc]
	public void RpcUpdatePlayerPoints (int CollectedPickups, GameObject pickup){
		if (isLocalPlayer) {
			_userInterfaceController.UpdatePlayerPoints (CollectedPickups);
			pickup.GetComponent<AudioSource> ().Play ();
		}
	}

	#endregion

	/// <summary>
	/// Show trigger when local player receives physically a file (called by localPlayer).
	/// <param name="file">The file to receive.</param>
	/// </summary>
	public void ReceiveFile(GameObject file){
		if (isLocalPlayer) {
			CmdVerifyReceiver(file, ConnectionId);
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

				// Vibrate Device to give feedback that the pickup is collected
				Vibration.Vibrate (200);

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
			Admin.Instance.CurrentTrackedPlayerText.text = PlayerName;
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

}
