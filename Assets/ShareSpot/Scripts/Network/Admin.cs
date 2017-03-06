using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
// Adminstration to create connections between Administration panel and PlayerController
/// </summary>
public class Admin : AManager<Admin> {

	#region [Public fields]
	public GameObject CurrentTrackedPlayer; ///< Current selected Player
	public Button ButtonClientOne;	///< Temporarily manually created ButtonTwo
	public Button ButtonClientTwo; ///< Temporarily manually created ButtonTwo
	public Button[] ClientButtons; ///< Array for the buttons for all connected Clients
	public GameObject[] ConnectedClients; ///< Array for all connected Clients
	public GameObject ClientButtonPrefab; ///< Prefab of a Client Button for programmatically instantiation

	// Global Constants
	public int MaxClients = 10;  ///< Maximal allowed connected clients
	#endregion

	// Preparing Arrays
	public void Start(){
		ClientButtons = new Button[MaxClients+1];
		ConnectedClients = new GameObject[MaxClients+1];
	}

	// Creates connection between the selected TrackedPlayer obtained from the Tracking-Server with the indicated Client
	// who is connected to the network
	public void ConnectToClient(int connectionId)
	{
		if (CurrentTrackedPlayer != null)
		{
			// Assign the connected client the current tracked player
			ConnectedClients[connectionId].GetComponent<PlayerController>().ControllingPlayer = CurrentTrackedPlayer;
			ConnectedClients[connectionId].GetComponent<PlayerController>().HasControllingPlayer = true;

			// Tell the current tracked player that he has now a connected client assigned to him
			CurrentTrackedPlayer.GetComponent<TrackedPlayerNetwork>().ControlledPlayer = ConnectedClients[connectionId];
			CurrentTrackedPlayer.GetComponent<TrackedPlayerNetwork>().HasPlayer = true;

			// Format the newly connected player
			//ButtonClientOne.interactable = false;
			ConnectedClients[connectionId].GetComponent<MeshRenderer>().material.color = Color.black;
			ConnectedClients[connectionId].GetComponent<PlayerController> ().PlayerName = "Client "+connectionId;
		}

	}

	// Instantiating one Button per newly connected Client
	public void AddClientButton(int connectionId){
		// only create the Button if it is not already existing
		if (ClientButtons [connectionId] == null) {
			// Instantiate Button from prefab
			GameObject ClientButton = (GameObject)Instantiate (ClientButtonPrefab);

			// Set name of the Button
			ClientButton.GetComponentInChildren<Text> ().text += connectionId;

			// Add OnClickListener which calls ConnectToClient with @param connectionId
			ClientButton.GetComponent<Button> ().onClick.AddListener (delegate {
				ConnectToClient (connectionId);
			});

			// Set the right parent of the GameObject
			ClientButton.transform.SetParent (GameObject.FindGameObjectWithTag ("PlayersPanel").transform, false);

			// Add Buttons to ClientButtons Array
			ClientButtons [connectionId] = ClientButton.GetComponent<Button> ();
		}
	}
}
