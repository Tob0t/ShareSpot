using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
// MyNetworkManager inherits from NetworManager for customizing some functions
/// </summary>
public class MyNetworkManager : NetworkManager {

	#region [Public fields]
	public static MyNetworkManager Instance = null;	///< static instance of MyNetworkManager which allows it to be accessed by any other script.
	public string ConnectionIP; ///< The IP-Address to connect to
	public int ConnectionPort = 7777; ///< The Port to connect to (Standard is 7777)
	public bool ClientConnected = false; ///< Determine if a client is already connected (Standard is false)
	public bool IsServer;	///< Determine if the caller is a Server
	public bool IsClient; ///< Determine if the caller is a Client
	public Text DebugTextClient; ///< Debugging the actions of the client
	public Text DebugTextServer; ///< Debugging the actions of the server



	// Gameobjects for Userinterfaces Client
	public GameObject UI_Setup;	///< Userinterface for Starting Setup Scene
	public GameObject UI_Wait; ///< Userinterface for Waiting Setup Scene
	public GameObject UI_Panel; ///< Userinterface Panel for Setup
	public GameObject UserInterfaceController; ///< Userinterface Controller for controlling the inputs


	// Gameobjects for Userinterfaces Server
	public GameObject ButtonStartServer; ///< Button to start the server
	public GameObject ButtonStopServer; ///< Button to stop the server
	public GameObject GamePanel; ///< GamePanel for controlling a Game

	#endregion

	#region [Private fields]

	#endregion

	void Awake()
	{
		// Singleton pattern to ensure that there is only one instance of MyNetworkManager
		if (Instance == null) {
			Instance = this;
		}
		else if (Instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	// Manually setting IP Address
	void SetIPAddress()
	{
		networkAddress = ConnectionIP;
	}

	#region Server
	// Procedures when starting the server
	public void StartServerHosting()
	{
		StartServer();
		IsServer = true;
		ButtonStartServer.SetActive (false);
		ButtonStopServer.SetActive (true);
		NetworkServer.SpawnObjects();
	}

	// Procedures when stopping the server
	public void StopServerHosting()
	{
		StopServer();
		GamePanel.SetActive (false);
		ButtonStopServer.SetActive (false);
		ButtonStartServer.SetActive (true);
		NetworkServer.Reset();
	}

	// Override function when the server starts
	public override void OnStartServer()
	{
		base.OnStartServer();
		DebugTextServer.text = "Server started";
	}

	// Override function when the server stopps
	public override void OnStopServer()
	{
		base.OnStopServer();
		// Cleanup and delete all clients
		foreach (GameObject connectedClient in Admin.Instance.ConnectedClients) {
			NetworkServer.Destroy (connectedClient);
		}
		DebugTextServer.text = "Server stopped";
	}

	// Override function when a client connects
	public override void OnServerConnect(NetworkConnection conn)
	{
		base.OnServerConnect(conn);
		// TODO: is it necessary here?
		// only add the connected client and button if there are not more than @param MaxClients
		if (conn.connectionId <= Admin.Instance.MaxClients) {
			Admin.Instance.AddClientButton(conn.connectionId);
		}
		toggleClientPanel (conn, true);
		DebugTextClient.text += "Client " + conn.connectionId + " connected.\n";

	}

	// Override function when a client disconnects
	public override void OnServerDisconnect(NetworkConnection conn)
	{
		// show the template of a tracked player if the client is not connected anymore
		Admin.Instance.ConnectedClients [conn.connectionId].GetComponent<PlayerController> ().ControllingPlayer.SetActive (true);
		base.OnServerDisconnect(conn);
		toggleClientPanel (conn, false);
		DebugTextClient.text += "Client " + conn.connectionId + " disconnected.\n";
	}

	// Toggle the Buttons in ClientPanel depending on the current connection
	private void toggleClientPanel(NetworkConnection conn, bool toggleMode){
		Admin.Instance.ClientButtons [conn.connectionId].gameObject.SetActive (toggleMode);
	}

	#endregion

	#region Client
	// Override function when a client connects (on client side)
	public override void OnClientConnect(NetworkConnection conn){
		base.OnClientConnect (conn);
		UI_Setup.SetActive (false);
		UI_Wait.SetActive (true);
	}

	// Procedure when a Client connects to the Server
	public void JoinServer()
	{
		SetIPAddress();
		StartClient();
		IsClient = true;
	}

	// Procedure to let the client reconnect
	public void ReconnectClient()
	{
		StopClient();
		UserInterfaceController.GetComponent<UserInterfaceController>().DisableAllGamePanels ();
		UI_Wait.SetActive (true);
		UserInterfaceController.GetComponent<UserInterfaceController>().PlayerObject = null;
		StartCoroutine("Reconnect");
	}

	IEnumerator Reconnect()
	{
		yield return new WaitForSeconds(1f);
		JoinServer();
	}

	// Procedure if the client is stopped
	public override void OnStopClient(){
		UserInterfaceController.GetComponent<UserInterfaceController>().DisableAllGamePanels ();
		UI_Wait.SetActive (false);
		UI_Panel.SetActive (true);
		UI_Setup.SetActive (true);
		// TODO Is not necessary right?
		//userInterfaceController.PlayerObject = null;
		base.OnStopClient ();
	}

	#endregion
}