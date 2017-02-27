using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
// MyNetworkManager inherits from NetworManager for customizing some functions
/// </summary>
public class MyNetworkManager : NetworkManager {

	#region [Public fields]
	public string ConnectionIP;
	public int ConnectionPort = 7777;
	public bool ClientConnected = false;
	public Text DebugTextClient;
	public static MyNetworkManager Instance = null;              // static instance of MyNetworkManager which allows it to be accessed by any other script.
	public bool IsServer;
	public bool IsClient;
	public Text DebugTextServer;

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
		Admin.Instance.PlayersPanel.gameObject.SetActive (false);
		StartServer();
		IsServer = true;
		NetworkServer.SpawnObjects();
	}

	// Procedures when stopping the server
	public void StopServerHosting()
	{
		StopServer();
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
		DebugTextServer.text = "Server stopped";
	}

	// Override function when a client connects
	public override void OnServerConnect(NetworkConnection conn)
	{
		base.OnServerConnect(conn);
		Admin.Instance.PlayersPanel.gameObject.SetActive (true);
		DebugTextClient.text += "Client " + conn.connectionId + " connected.\n";
	}

	// Override function when a client disconnects
	public override void OnServerDisconnect(NetworkConnection conn)
	{
		base.OnServerDisconnect(conn);
		switch (conn.connectionId) {
		case 1:
			//Admin.Instance.ButtonPlayerOne.gameObject.SetActive (false);
			break;
		case 2:
			//Admin.Instance.ButtonPlayerOne.gameObject.SetActive (false);
			break;
		default:
			break;
		}

		DebugTextClient.text += "Client " + conn.connectionId + " disconnected.\n";
	}
	#endregion

	#region Client
	// Override function when a client connects (on client side)
	public override void OnClientConnect(NetworkConnection conn){
		base.OnClientConnect (conn);
		GameObject.FindGameObjectWithTag ("UI_Setup").SetActive (false);
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
		StartCoroutine("Reconnect");
	}

	IEnumerator Reconnect()
	{
		yield return new WaitForSeconds(1f);
		JoinServer();
	}

	#endregion
}