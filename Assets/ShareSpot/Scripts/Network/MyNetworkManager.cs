using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// MyNetworkManager inherits from NetworManager for customizing some functions.
/// </summary>
public class MyNetworkManager : NetworkManager {

	#region [Public fields]
	public static MyNetworkManager Instance = null;	///< static instance of MyNetworkManager which allows it to be accessed by any other script.
	public string ConnectionIP; ///< The IP-Address to connect to.
	public int ConnectionPort = 7777; ///< The Port to connect to (Standard is 7777).
	public bool IsServer;	///< Determine if the caller is a Server.
	public bool IsClient; ///< Determine if the caller is a Client.
	public Text DebugTextClient; ///< Debugging the actions of the client.
	public Text DebugTextServer; ///< Debugging the actions of the server.

	/// Gameobjects for Userinterfaces Client
	public GameObject UI_Setup;	///< Userinterface for Starting Setup Scene.
	public GameObject UI_Wait; ///< Userinterface for Waiting Setup Scene.
	public GameObject UI_Panel; ///< Userinterface Panel for Setup.
	public GameObject UserInterfaceController; ///< Userinterface Controller for controlling the inputs.

	/// Gameobjects for Userinterfaces Server
	public GameObject ButtonStartServer; ///< Button to start the server.
	public GameObject ButtonStopServer; ///< Button to stop the server.
	public GameObject GameControlPanel; ///< GamePanel for controlling a Game.
	public GameObject GameDebugPanel; ///< GamePanel for debugging a Game.

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



	#region Server
	/// <summary>
	/// Procedures when starting the server.
	/// </summary>
	public void StartServerHosting()
	{
		StartServer();
		GameControlPanel.SetActive (true);
		GameDebugPanel.SetActive (true);
		IsServer = true;
		ButtonStartServer.SetActive (false);
		ButtonStopServer.SetActive (true);
		NetworkServer.SpawnObjects();
	}

	/// <summary>
	/// Procedures when stopping the server.
	/// </summary>
	public void StopServerHosting()
	{
		StopServer();
		GameControlPanel.SetActive (false);
		ButtonStopServer.SetActive (false);
		ButtonStartServer.SetActive (true);
		NetworkServer.Reset();
	}

	/// <summary>
	/// Override function when the server starts.
	/// </summary>
	public override void OnStartServer()
	{
		base.OnStartServer();
		DebugTextServer.text = "Server started";
	}

	/// <summary>
	/// Override function when the server stopps.
	/// </summary>
	public override void OnStopServer()
	{
		base.OnStopServer();
		// Cleanup and delete all clients
		foreach (GameObject connectedClient in Admin.Instance.ConnectedClients) {
			// show template of a tracked player if the client is not connected anymore
			if (connectedClient != null && connectedClient.GetComponent<PlayerController> ().ControllingPlayer != null) {
				connectedClient.GetComponent<PlayerController> ().ControllingPlayer.GetComponent<TrackedPlayerNetwork> ().HasPlayer = false;
			}
			// destroy connected client
			NetworkServer.Destroy (connectedClient);

		}
		// Cleanup and delete all pickups
		foreach (GameObject pickup in GameObject.FindGameObjectsWithTag("Pickup")){
			NetworkServer.Destroy (pickup);
		}
		DebugTextServer.text = "Server stopped";
	}

	/// <summary>
	/// Override function when a client connects.
	/// </summary>
	/// <param name="conn">Connection id of the client.</param>
	public override void OnServerConnect(NetworkConnection conn)
	{
		base.OnServerConnect(conn);
		// only add the connected client and button if there are not more than @param MaxClients
		if (conn.connectionId <= GlobalConfig.MaxClients) {
			Admin.Instance.AddClientButton(conn.connectionId);
		}
		DebugTextClient.text += "Client " + conn.connectionId + " connected.\n";

	}

	/// <summary>
	/// Override function when a client disconnects.
	/// </summary>
	/// <param name="conn">Connection id of the client.</param>
	public override void OnServerDisconnect(NetworkConnection conn)
	{
		// show the template of a tracked player if the client is not connected anymore
		if (Admin.Instance.ConnectedClients [conn.connectionId].GetComponent<PlayerController> ().HasControllingPlayer) {
			Admin.Instance.ConnectedClients [conn.connectionId].GetComponent<PlayerController> ().ControllingPlayer.GetComponent<TrackedPlayerNetwork>().HasPlayer = false;
		}
		//toggleClientPanel (conn, false);
		Destroy(Admin.Instance.ClientButtons [conn.connectionId].gameObject);
		DebugTextClient.text += "Client " + conn.connectionId + " disconnected.\n";

		base.OnServerDisconnect(conn);

	}

	/// <summary>
	/// Toggle the Buttons in ClientPanel depending on the current connection.
	/// </summary>
	/// <param name="conn">Connection id of the client.</param>
	/// <param name="toggleMode">Specifies if it should be shown or hidden.</param>
	private void toggleClientPanel(NetworkConnection conn, bool toggleMode){
		Admin.Instance.ClientButtons [conn.connectionId].gameObject.SetActive (toggleMode);
	}

	#endregion

	#region Client

	/// <summary>
	/// Manually setting IP Address.
	/// </summary>
	void SetIPAddress()
	{
		networkAddress = ConnectionIP;
	}

	/// <summary>
	/// Override function when a client connects (on client side).
	/// </summary>
	/// <param name="conn">Conn.</param>
	public override void OnClientConnect(NetworkConnection conn){
		base.OnClientConnect (conn);
		UI_Setup.SetActive (false);
		UI_Wait.SetActive (true);
	}

	/// <summary>
	/// Procedure when a Client connects to the Server.
	/// </summary>
	public void JoinServer()
	{
		SetIPAddress();
		StartClient();
		IsClient = true;
	}

	/// <summary>
	/// Procedure to let the client reconnect.
	/// </summary>
	public void ReconnectClient()
	{
		StopClient();
		UserInterfaceController.GetComponent<UserInterfaceController>().DisableAllGamePanels ();
		UI_Wait.SetActive (true);
		UserInterfaceController.GetComponent<UserInterfaceController>().PlayerObject = null;
		StartCoroutine("Reconnect");
	}

	/// <summary>
	/// Reconnect to the server after waiting for 1 second.
	/// </summary>
	IEnumerator Reconnect()
	{
		yield return new WaitForSeconds(1f);
		JoinServer();
	}

	/// <summary>
	/// Procedure if the client is stopped.
	/// </summary>
	public override void OnStopClient(){
		UserInterfaceController.GetComponent<UserInterfaceController>().DisableAllGamePanels ();
		UserInterfaceController.GetComponent<UserInterfaceController> ().UI_Wait.GetComponentInChildren<InputField> ().text = "";
		UI_Wait.SetActive (false);
		UI_Panel.SetActive (true);
		UI_Setup.SetActive (true);
		base.OnStopClient ();
	}

	#endregion
}