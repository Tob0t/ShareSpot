using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// The pickup controller is managing the state of a specific pickup.
/// </summary>
public class PickupController : NetworkBehaviour {

	public GameObject AssociatedNetworkPlayer;	///< TODO: not needed?	The networkplayer who is collecting the pickup.
	[SyncVar]
	public Color ChosenColor;	///< The color of the pickup.
	[SyncVar]
	public int ValidForConnectionId;	///< The connection id of the player for which the pickup should only be displayed.

	// Use this for initialization
	void Start () {

		// Set the color to the chosen color for the specific player
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

	/// <summary>
	/// Sets the color to the chosen color for the specific player.
	/// </summary>
	/// <param name="color">Color.</param>
	private void SetColor(Color color){
		GetComponent<MeshRenderer> ().material.color = ChosenColor;
	}

}
