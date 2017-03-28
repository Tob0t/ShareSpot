using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// ClientManager helps to client to continously connect to the server on startup.
/// </summary>
public class ClientManager : MonoBehaviour {

	#region [Public fields]
	public InputField ServerIpAdressInput; ///< IP-Adress of the Input field.
	public Text ServerIpAdressTextSetup; ///< IP-Adress TextBox in Setup-Userinterface.
	public Text ServerIpAdressTextSetupWait; ///< IP-Address TextBox in Wait-Userinterface.

	#endregion

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ServerIpAdressTextSetup.text = GetComponent<MyNetworkManager> ().ConnectionIP;
		if (!MyNetworkManager.Instance.isNetworkActive) {
			GetComponent<MyNetworkManager> ().JoinServer ();
		}
	}

	/// <summary>
	/// Set the IP-Adress received from the Input and try to join the server.
	/// </summary>
	public void ConnectToServer(){
		ServerIpAdressTextSetupWait.text = ServerIpAdressInput.text;
		GetComponent<MyNetworkManager> ().ConnectionIP = ServerIpAdressInput.text;
		GetComponent<MyNetworkManager> ().JoinServer ();
	}

	// 
	/// <summary>
	/// Insert the IP Address from the selected Button into the TextView and tries to connect.
	/// </summary>
	/// <param name="IPAddress">IP address of the TextView.</param>
	public void InsertIPAdress(string IPAddress){
		ServerIpAdressInput.text = IPAddress;
		ConnectToServer ();
	}

}