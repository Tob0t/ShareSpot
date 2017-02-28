using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

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
	public GameObject ControllingPlayer;
	public Camera ARCamera;
	public GameObject FilePrefab;
	public Transform FileSpawn;
	public GameObject CubePrefab;
	public GameObject CylinderPrefab;
	public Transform ItemSpawn;
	public int ShootingSpeed;
	public int PlayerSpeed;
	public GUISkin CustomGuiSkin;

	#endregion

	#region [Private fields]
	private IInputMode inputMode;
	private Vector3 _previousPos;
	private float distance;
	private float _timePassed;
	private bool _staticPos;

	#endregion

	// Use this for initialization
	void Start () {
	}

	// Function which is only called for the local player
	public override void OnStartLocalPlayer(){
		if (Application.platform == RuntimePlatform.Android) {
			inputMode = new InputMobile ();
		} else {
			inputMode = new InputComputer ();
		}
		transform.GetComponent<MeshRenderer>().material.color = Color.blue;

	}

	// Update is called once per frame
	void Update () {
		// only called if localPlayer and if there is already a connection to a tracked player (set by admin)
		if (isLocalPlayer && HasControllingPlayer) {
			// Activate ARCamera for that player
			transform.Find ("ARCamera").gameObject.SetActive (true);

			// Update position and rotation

			// allow rotation by keyboard for non android runtimes
			if (Application.platform != RuntimePlatform.Android) {
				//float z = inputMode.Move() * Time.deltaTime * PlayerSpeed;
				float x = inputMode.Turn () * Time.deltaTime * 150.0f;
				transform.Rotate (0, x, 0);
			}
			// update player position to the syncVar obtained by the trackingClient from the server
			transform.position = Position;

			// Call function for firing an object
			if (inputMode.Fire ()) {
				CmdShootFile ();
			}
		}
		// only called if server instance
		if (isServer) {
			transform.name = "Client #" + connectionToClient.connectionId;

			// set the GameObjects depending on their connectionId
			if (connectionToClient.connectionId == 1) {
				Admin.Instance.ClientOne = gameObject;
			}
			if (connectionToClient.connectionId == 2) {
				Admin.Instance.ClientTwo = gameObject;
			}
			// update position of the player object (obtained by the tracking system)
			if (ControllingPlayer != null) {
				transform.position = ControllingPlayer.transform.position;
				Position = transform.position;
			}
		}

	}
	void OnGUI(){
		// only call the GUI if it is a local player and connected to a tracking player
		if (!isLocalPlayer || !HasControllingPlayer){
			return;
		}
		GUI.skin = CustomGuiSkin;

		if (GUI.Button(new Rect(10, 100, 500, 200), "Create Cube")){
			Debug.Log ("Creating Cube now by Command");
			CmdCreateItem(1);
		}
		if (GUI.Button(new Rect(10, 350, 500, 200), "Create Cylinder")){
			CmdCreateItem(2);
		}
		if (GUI.Button (new Rect (Screen.width - 500, 100, 500, 200), "Shoot File")) {
			CmdShootFile();
		}
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
	void CmdShootFile(){
		// Create the SharedFile from the file Prefab
		var sharedFile = (GameObject)Instantiate(
			FilePrefab,
			FileSpawn.position,
			FileSpawn.rotation);

		// Add velocity to the file
		sharedFile.GetComponent<Rigidbody>().velocity = sharedFile.transform.forward * ShootingSpeed;

		// Spawn the file on the Clients
		NetworkServer.Spawn(sharedFile);

		// Destroy the file after 4 seconds
		Destroy(sharedFile, 4.0f);
	}

	// Show message when Player receives a File
	public void ReceiveFile(){
		if (isLocalPlayer) {
			transform.Find ("CanvasTrigger").gameObject.SetActive(true);
		}
	}
}
