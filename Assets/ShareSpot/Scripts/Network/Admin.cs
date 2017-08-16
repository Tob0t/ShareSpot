using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Adminstration to create connections between Administration panel and PlayerController.
/// </summary>
public class Admin : AManager<Admin> {

	#region [Public fields]
	public GameObject CurrentTrackedPlayer; ///< Current selected Player.
	public Button[] ClientButtons; ///< Array for the buttons for all connected Clients.
	public GameObject[] ConnectedClients; ///< Array for all connected Clients.
	public GameObject ClientButtonPrefab; ///< Prefab of a Client Button for programmatically instantiation.
	public GameObject GameManger; ///< GameManager for controlling a Game.

	public InputField NewNameTextBox;	///< TextBox for the new name.
	public Text CurrentTrackedPlayerText; ///< Text displaying the current tracked player.

	#endregion


	/// <summary>
	/// Preparing Arrays.
	/// </summary>
	public void Start(){
		ClientButtons = new Button[GlobalConfig.MaxClients+1];
		ConnectedClients = new GameObject[GlobalConfig.MaxClients+1];
	}

	/// <summary>
	/// Creates connection between the selected TrackedPlayer obtained from the Tracking-Server with the indicated Client
	/// who is connected to the network
	/// </summary>
	/// <param name="connectionId">Connection identifier of the client.</param>
	public void ConnectToClient(int connectionId)
	{
		if (CurrentTrackedPlayer != null)
		{
			// Enable the old tracked player and mark it available
			if (ConnectedClients [connectionId].GetComponent<PlayerController> ().HasControllingPlayer && ConnectedClients [connectionId].GetComponent<PlayerController> ().ControllingPlayer != null) {
				GameObject previousControllingPlayer = ConnectedClients [connectionId].GetComponent<PlayerController> ().ControllingPlayer;
				previousControllingPlayer.GetComponent<TrackedPlayerNetwork>().HasPlayer = false;
			}

			// Assign the connected client the current tracked player
			ConnectedClients[connectionId].GetComponent<PlayerController>().ControllingPlayer = CurrentTrackedPlayer;
			ConnectedClients[connectionId].GetComponent<PlayerController>().HasControllingPlayer = true;

			// Tell the current tracked player that he has now a connected client assigned to him
			CurrentTrackedPlayer.GetComponent<TrackedPlayerNetwork>().ControlledPlayer = ConnectedClients[connectionId];
			CurrentTrackedPlayer.GetComponent<TrackedPlayerNetwork>().HasPlayer = true;

			// Format the newly connected player
			ConnectedClients[connectionId].GetComponent<MeshRenderer>().material.color = Color.black;

			// Write the name of the current tracked player in the admin panel
			Admin.Instance.CurrentTrackedPlayerText.text = ConnectedClients [connectionId].GetComponent<PlayerController> ().PlayerName;

			// If the game is active, assign the right color as well
			if (GameManger.GetComponent<GameController> ().isGameActive) {
				ClientButtons[connectionId].GetComponent<Image>().color = GlobalConfig.GetColorForPlayerId (connectionId);
				ConnectedClients[connectionId].GetComponent<MeshRenderer> ().material.color = GlobalConfig.GetColorForPlayerId (connectionId);
			}
		}

	}

	/// <summary>
	/// Instantiating one Button per newly connected Client.
	/// </summary>
	/// <param name="connectionId">Connection identifier of the client.</param>
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

	/// <summary>
	/// Changes the name of the player by the server interface.
	/// </summary>
	public void ChangeName(){
		// Update the playername as syncvar
		CurrentTrackedPlayer.GetComponent<PlayerController> ().PlayerName = NewNameTextBox.text;
		// Update the name on the server button
		Admin.Instance.ClientButtons [CurrentTrackedPlayer.GetComponent<PlayerController> ().ConnectionId].GetComponentInChildren<Text> ().text = NewNameTextBox.text;
	}
}
