using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
// PlayerController is responsible for updating the player position and handle all the actions used by the player
/// </summary>
public class PlayerController : NetworkBehaviour{
	#region [Public fields]
	[SyncVar]
	public Quaternion Rotation;
	[SyncVar]
	public Vector3 Position;
	[SyncVar]
	public bool HasControllingPlayer;
	[SyncVar]
	public string PlayerName;
	[SyncVar]
	public int ConnectionId;
	[SyncVar]
	public int CollectedPickups;
	[SyncVar]
	public int SharingMode;
	public int ShowChallengesRate = 3;
	public GameObject ControllingPlayer;
	public GameObject FilePrefab;
	public Transform FileSpawn;
	public GameObject CubePrefab;
	public GameObject CylinderPrefab;
	public Transform ItemSpawn;
	public float ShootingSpeed = 0.01f;
	public int PlayerSpeed;

	public float minX = 100f;
	public float maxX = 1820f;
	public float minY = 80f;
	public float maxY = 120f;
	public float minZ = 100f;
	public float maxZ = 980f;

	//public Color CollectingColor;
	public GameObject GameManager;


	#endregion

	#region [Private fields]
	private IInputMode inputMode;
	private Vector3 _previousPos;
	private float distance;
	private float _timePassed;
	private bool _staticPos;
	// Bool for initial setup
	private bool initialSetup;
	private GameObject PlayerUserInterface;

	private GameObject ARCamera;


	#endregion

	// Use this for initialization
	void Start () {
		CollectedPickups = 0;
		if (isServer) {
			GameManager = GameObject.FindGameObjectWithTag ("GameManager");
		}
	}

	// Function which is only called for the local player
	public override void OnStartLocalPlayer(){
		// init Userinterfaces
		PlayerUserInterface = GameObject.FindGameObjectWithTag("PlayerUserInterface");
		ARCamera = GameObject.FindGameObjectWithTag ("ARCamera");

		#if UNITY_ANDROID
			inputMode = new InputMobile ();
		#else
			inputMode = new InputComputer ();
		#endif
		//transform.GetComponent<MeshRenderer>().material.color = Color.blue;
		transform.GetComponent<MeshRenderer>().enabled = false;
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
					// Connect to local UserInterface
					PlayerUserInterface.GetComponent<UserInterfaceController>().InitialSetup(this.gameObject);

/*****************************************************************************************************************************/
					// Temp set User to 0,0,0
					transform.position = new Vector3(500,185,500);
/*****************************************************************************************************************************/

					// set to true
					initialSetup = true;
				}
				// Loop through all pickups and only activate the right ones
				// TODO: move to appropiate place to avoid looping every frame
				/*foreach (GameObject pickup  in GameObject.FindGameObjectsWithTag("Pickup")) {
					if(pickup.GetComponent<PickupController>().ValidForConnectionId == ConnectionId){
						pickup.SetActive(true);
					} else{
						pickup.SetActive(false);
					}
				}*/

				// Update position and rotation

				// allow rotation by keyboard for non android runtimes
				if (Application.platform != RuntimePlatform.Android) {
					
					float x = inputMode.Turn () * Time.deltaTime * 150.0f;
					transform.Rotate (0, x, 0);
					ARCamera.transform.rotation = transform.rotation;
/*****************************************************************************************************************************/					
					// TEMP to be able to move
					float z = inputMode.Move() * Time.deltaTime * PlayerSpeed;
					transform.Translate(0,0,z);
/*****************************************************************************************************************************/
				} else{
					// update ARCamera and player position from the syncVar obtained by the trackingClient from the server
					transform.position = Position;
				}
				ARCamera.transform.position = transform.Find("Hand").position;

				// update rotation obtained from the gyroclient of the ARCamera
				transform.rotation = ARCamera.transform.rotation;
			}
			#endregion
		}


		#region Server
		// only called if server instance
		if (isServer) {

			if(!initialSetup){
				transform.name = "Client #" + connectionToClient.connectionId;

				// only add the connected client and button if there are not more than @param MaxClients
				if (connectionToClient.connectionId <= Admin.Instance.MaxClients) {
					Admin.Instance.ConnectedClients [connectionToClient.connectionId] = gameObject;
				}
				initialSetup = true;
			}
				
			// update position of the player object (obtained by the tracking system)
			if (ControllingPlayer != null) {
				transform.position = ControllingPlayer.transform.position;
				Position = transform.position;
			}
		}
		#endregion

	}

	// Command executed on the server for creating virtual sample objects
	[Command]
	void CmdCreateItem(int itemType){
		GameObject item = null;
		if (itemType == 1) {
			item = (GameObject)Instantiate (CubePrefab, ItemSpawn.position, ItemSpawn.rotation);
		} else if (itemType == 2) {
			item = (GameObject)Instantiate (CylinderPrefab, ItemSpawn.position, ItemSpawn.rotation);
		}
		if (item != null) {
			NetworkServer.Spawn (item);
		}
	}

	// Command executed on the server for shooting a SharedFile
	[Command]
	public void CmdShootFile(Vector3 force){
		// Create the SharedFile from the file Prefab
		var sharedFile = (GameObject)Instantiate(
			FilePrefab,
			FileSpawn.position,
			FileSpawn.rotation);

		//sharedFile.GetComponent<Rigidbody>().velocity = sharedFile.transform.forward * ShootingSpeed;
		Debug.Log("Force Vector: "+force.ToString());

		// Transform the force in a direction first and set is as velocity
		sharedFile.GetComponent<Rigidbody> ().velocity = transform.TransformDirection (force*ShootingSpeed);
		//sharedFile.GetComponent<Rigidbody>().AddRelativeForce(force * ShootingSpeed);
		sharedFile.GetComponent<SharedFile> ().SourceId = connectionToClient.connectionId;

		// Spawn the file on the Clients
		NetworkServer.Spawn(sharedFile);

		// Destroy the file after 30 seconds
		Destroy(sharedFile, 30.0f);
	}

	// Command executed on the server for changing the name
	[Command]
	public void CmdChangeName(string newName){
		PlayerName = newName;
		Admin.Instance.ClientButtons [ConnectionId].GetComponentInChildren<Text> ().text = newName;
	}

	// Command executed on the server for moving the pickup to a new random location
	[Command]
	public void CmdPickupCollected(GameObject pickup){
		Debug.Log ("Move Pickup");
		CollectedPickups++;
		if (CollectedPickups % ShowChallengesRate == 0) {
			string description = GameManager.GetComponent<GameController> ().CreateChallenge (gameObject);
			RpcShowChallenge (description);
		}
		// Create random Location
		Vector3 randomLocation = new Vector3(Random.Range (minX, maxX), Random.Range (minY, maxY), Random.Range (minZ, maxZ));
		pickup.transform.position = randomLocation;
	}

	/// <summary>
	/// Command executed on the server for receiving a file
	/// </summary>
	// TODO Show file (null)
	[Command]
	public void CmdReceiveFile(GameObject file, int senderId, int receiverId){
		Debug.Log ("Sender ID :" + senderId);
		Debug.Log ("ReceiverId ID :" + receiverId);
		bool challengeState = GameManager.GetComponent<GameController> ().VerifyChallenge (senderId, receiverId);

		// Forward to sending client that the file was sent
		Admin.Instance.ConnectedClients[senderId].GetComponent<PlayerController>().RpcFileSent(challengeState);

		// only needed if I want the the receiver to show trigger
		//Admin.Instance.ConnectedClients[receiverId].GetComponent<PlayerController>().GetComponent<PlayerController>().RpcReceiveFile (file);
	}
		
	/// <summary>
	/// ClientRpc executed on the client for feedback of a sent file
	/// </summary>
	[ClientRpc]
	public void RpcFileSent(bool challengeState){
		if (isLocalPlayer) {
			if (challengeState) {
				// TODO: move instructions to one function in UserInterfaceController and show successfull message for x seconds
				// Disable the instruction canvas
				PlayerUserInterface.GetComponent<UserInterfaceController> ().ToggleGameCanvas (false);

				// Disable the sharing mode canvas
				PlayerUserInterface.GetComponent<UserInterfaceController> ().ToggleSharingModeCanvas(false);
			} else {
				// Show error canvas
				PlayerUserInterface.GetComponent<UserInterfaceController> ().ShowErrorCanvas ();
			}
		}
	}

	/// <summary>
	/// ClientRpc executed on the client for displaying the new challenge
	/// </summary>
	[ClientRpc]
	public void RpcShowChallenge(string description){
		if (isLocalPlayer) {
			// Display the new challenge on the clients UI
			PlayerUserInterface.GetComponent<UserInterfaceController> ().ShowNewChallenge (description);

			// Enable the sharing mode canvas which is assigned to the client
			PlayerUserInterface.GetComponent<UserInterfaceController> ().ToggleSharingModeCanvas(true);
		}
	}

	/// <summary>
	/// ClientRpc to Show only the own pickups when collecting (called only once when game is started by GameController)
	/// </summary>
	[ClientRpc]
	public void RpcShowOnlyOwnPickups(){
		if (isLocalPlayer) {
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
	/// Show trigger when local player receives a File
	/// For game not needed
	/// </summary>
	public void ReceiveFile(GameObject file){
		if (isLocalPlayer) {

			CmdReceiveFile(file, file.GetComponent<SharedFile>().SourceId, ConnectionId);

			// Show Incoming file on local client 
			// For GameMode not needed
			//PlayerUserInterface.GetComponent<UserInterfaceController> ().ShowIncomingFile (file);
		}
	}
		
	/// <summary>
	/// When the player object is colliding with another object
	/// </summary>
	void OnTriggerEnter(Collider other){
		if (isLocalPlayer) {
			Debug.Log ("OnTriggerEnter");
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
	// Called when the GameObject is selected
	/// </summary>
	void OnMouseDown(){
		Debug.Log ("Mouse Down on " + gameObject);
		if (isServer) {
			Admin.Instance.CurrentTrackedPlayer = gameObject;
			StartCoroutine (SelectPlayer());
		}
	}

	/// <summary>
	/// Coroutine to change the color for 4 seconds
	/// </summary>
	IEnumerator SelectPlayer(){
		Color current = gameObject.GetComponent<MeshRenderer> ().material.color;
		gameObject.GetComponent<MeshRenderer> ().material.color = Color.green;
		yield return new WaitForSeconds(4f);
		gameObject.GetComponent<MeshRenderer> ().material.color = current;
	}


	/*
	/// FOR GAME MODE NOT NEEDED
	/// TODO: Check if game is running or not
	/// <summary>
	/// ClientRpc executed on the client for receiving the file and showing incoming file
	/// </summary>
	[ClientRpc]
	public void RpcReceiveFile(GameObject file){
		if (isLocalPlayer) {
			PlayerUserInterface.GetComponent<UserInterfaceController> ().ShowIncomingFile (file);
		}
	}


	*/
}
