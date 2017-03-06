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
	[SyncVar]
	public string PlayerName;
	public GameObject ControllingPlayer;
//	public GameObject ARCamera;
	public GameObject FilePrefab;
	public Transform FileSpawn;
	public GameObject CubePrefab;
	public GameObject CylinderPrefab;
	public Transform ItemSpawn;
	public int ShootingSpeed;
	public int PlayerSpeed;


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

					// set to true
					initialSetup = true;
				}

				// Update position and rotation

				// allow rotation by keyboard for non android runtimes
				if (Application.platform != RuntimePlatform.Android) {
					//float z = inputMode.Move() * Time.deltaTime * PlayerSpeed;
					float x = inputMode.Turn () * Time.deltaTime * 150.0f;
					transform.Rotate (0, x, 0);
					ARCamera.transform.rotation = transform.rotation;
				}
				// update ARCamera and player position from the syncVar obtained by the trackingClient from the server
				ARCamera.transform.position = transform.position = Position;

				// update rotation obtained from the gyroclient of the ARCamera
				transform.rotation = ARCamera.transform.rotation;
			}
			#endregion
		}


		#region Server
		// only called if server instance
		if (isServer) {
			transform.name = "Client #" + connectionToClient.connectionId;

			// only add the connected client and button if there are not more than @param MaxClients
			if (connectionToClient.connectionId <= Admin.Instance.MaxClients) {
				Admin.Instance.ConnectedClients [connectionToClient.connectionId] = gameObject;
				Admin.Instance.AddClientButton(connectionToClient.connectionId);
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
	public void CmdShootFile(Vector2 direction){
		// Create the SharedFile from the file Prefab
		var sharedFile = (GameObject)Instantiate(
			FilePrefab,
			FileSpawn.position,
			FileSpawn.rotation);

		// TODO: Add velocity to the file
		//sharedFile.GetComponent<Rigidbody>().velocity = sharedFile.transform.forward * ShootingSpeed;
		Debug.Log("Direction Vector: "+direction.ToString());
		Debug.Log("Forward Vector: "+sharedFile.transform.forward.ToString());
		//sharedFile.GetComponent<Rigidbody>().velocity = direction * ShootingSpeed;
		sharedFile.GetComponent<Rigidbody>().AddForce(new Vector3(direction.x,0,direction.y));

		// Spawn the file on the Clients
		NetworkServer.Spawn(sharedFile);

		// Destroy the file after 4 seconds
		Destroy(sharedFile, 4.0f);
	}

	// Command executed on the server for changing the name
	[Command]
	public void CmdChangeName(string newName){
		PlayerName = newName;
	}

	// Show trigger when local player receives a File
	public void ReceiveFile(GameObject file){
		if (isLocalPlayer) {
			PlayerUserInterface.GetComponent<UserInterfaceController> ().ShowIncomingFile (file);
		}
	}
}
