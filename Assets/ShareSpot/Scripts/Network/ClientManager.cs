using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
// ClientManager helps to client to continously connect to the server on startup
/// </summary>
public class ClientManager : MonoBehaviour {

	#region [Public fields]
	public InputField ServerIpAdressInput;
	public Text ServerIpAdressText;

	#endregion

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ServerIpAdressText.text = GetComponent<MyNetworkManager> ().ConnectionIP;
		if (!MyNetworkManager.Instance.isNetworkActive) {
			GetComponent<MyNetworkManager> ().JoinServer ();
		}
	}

	// Set the IP-Adress received from the Input and try to join the server
	public void SetIPAdress(){
		GetComponent<MyNetworkManager> ().ConnectionIP = ServerIpAdressInput.text;
		GetComponent<MyNetworkManager> ().JoinServer ();
	}

}