using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PickupController : NetworkBehaviour {

	public GameObject AssociatedNetworkPlayer;
	[SyncVar]
	public Color ChosenColor;
	[SyncVar]
	public int ValidForConnectionId;

	// Use this for initialization
	void Start () {
		SetColor (ChosenColor);
	}
	
	// Update is called once per frame
	void Update () {
		//if(AssociatedNetworkPlayer
		//if (AssociatedNetworkPlayer.GetComponent<NetworkPlayer> ().Equals (Network.player)) {
			// Rotate pickup every frame
			transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
		/*} else {
			gameObject.SetActive (false);
		}*/
	}

	private void SetColor(Color color){
		GetComponent<MeshRenderer> ().material.color = ChosenColor;
	}

}
