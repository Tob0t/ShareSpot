using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// GameController is responsible for managing the Gameplay and mainly executed on the Server.
/// </summary>
public class GameController : NetworkBehaviour {

	#region [Public fields]
	[SyncVar]
	public bool isGameActive = false;	///< Indicates if there is a game currently active.

	public GameObject PickupPrefab;	///< The prefab of a pickup.
	public Challenge[] currentChallenges;	///< Array of the current challenges of all players.

	// TODO: use maxRepetitions in class
	public int MaxRepetitions = 10;	///< Amount of repetions.

	public GameObject ButtonStartGame;	///< Button to start the game.
	public GameObject ButtonSaveData;	///< Button to persist the data to local storage.
	public GameObject ButtonDragNDrop;	///< Button for the sharing mode DragNDrop.
	public GameObject ButtonSwipeShot;	///< Button for the sharing mode SwipeShot.
	public GameObject ButtonTouchNChuck;	///< Button for the sharing mode TouchNChuck.

	public Text DebugGame; ///< Textbox for debugging the actions during the game.
	#endregion

	#region [Private fields]
	private List<Challenge>[] _allChallenges;	///< Array of all challenges saved in a list per player connection .

	#endregion
	// Use this for initialization
	void Start () {

		// only run on server
		if (!isServer)
			return;
		// disable Start Game Button first
		//ButtonStartGame.SetActive (false);

		// init the arrays
		currentChallenges = new Challenge[Admin.Instance.MaxClients+1];
		_allChallenges = new List<Challenge>[Admin.Instance.MaxClients+1];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	/// <summary>
	/// Helper function to toggle all the sharing mode buttons.
	/// </summary>
	/// <param name="id">Identifier of the button.</param>
	/// <param name="newValue">Indicates whether showing or hiding the button.</param>
	private void ToggleButton(int id, bool newValue){
		switch (id) {
		case 0:
			ButtonDragNDrop.SetActive (newValue);
			break;
		case 1:
			ButtonSwipeShot.SetActive (newValue);
			break;
		case 2:
			ButtonTouchNChuck.SetActive (newValue);
			break;
		default:
			break;
		}
	}
		

	/// <summary>
	/// Assign Sharing Mode to selected player object.
	/// </summary>
	/// <param name="sharingMode">Indicates the type of sharing mode.</param>
	public void AssignSharingMode(int sharingMode){
		// OPTIONAL
		// Get sharing method which is selected first end enable button
		/*int oldSharingMode = Admin.Instance.CurrentTrackedPlayer.GetComponent<PlayerController> ().SharingMode;
		if (oldSharingMode != null) {
			ToggleButton (oldSharingMode, true);	
		}
		// Disable current sharing method
		ToggleButton (sharingMode, false);
		*/

		// Assign sharing mode to the current selected player
		Admin.Instance.CurrentTrackedPlayer.GetComponent<PlayerController> ().SharingMode = sharingMode;
		DebugGame.text += "Assigned SharingMode " + (SharingMode) sharingMode + " to " + Admin.Instance.CurrentTrackedPlayer.GetComponent<PlayerController> ().PlayerName +"\n";

		// Remove Reference
		Admin.Instance.CurrentTrackedPlayer = null;
	}

	/// <summary>
	/// Starting the game and initializing the pickups.
	/// </summary>
	public void StartGame(){
		// Debugging on the server
		DebugGame.text += "Start Game\n";
		DebugGame.text += "Connected Players: "+MyNetworkManager.Instance.numPlayers +"\n";

		// Set the game state to true
		isGameActive = true;

		for (int j = 0; j <= MyNetworkManager.Instance.numPlayers; j++) {
			_allChallenges [j] = new List<Challenge> ();
		}

		// Loop to produce the Pickup prefabs
		int i=0;
		foreach (GameObject connectedClient in Admin.Instance.ConnectedClients) {
			if (connectedClient != null) {
				// Get valid area for finding random location
				Vector3 randomLocation = new Vector3 (Random.Range (GlobalHelper.GetPickupAreaMinValues().x, GlobalHelper.GetPickupAreaMaxValues().x), Random.Range (GlobalHelper.GetPickupAreaMinValues().y, GlobalHelper.GetPickupAreaMaxValues().y), Random.Range (GlobalHelper.GetPickupAreaMinValues().z, GlobalHelper.GetPickupAreaMaxValues().z));
				// Create the pickup from the file Prefab
				GameObject pickup = (GameObject)Instantiate (PickupPrefab, randomLocation, Quaternion.identity);
				// Add the color of the pickup to the Controller
				pickup.GetComponent<PickupController> ().ChosenColor = GlobalHelper.GetColorForPlayerId (i);
				// Set who is allowed to see the Pickup
				pickup.GetComponent<PickupController>().ValidForConnectionId = i;
				// Spawn the file on the Clients
				NetworkServer.Spawn (pickup);
			}
			i++;
		}

		// Start the game on each client
		foreach (GameObject connectedClient in Admin.Instance.ConnectedClients) {
			if (connectedClient != null) {
				connectedClient.GetComponent<PlayerController>().RpcStartGame ();	
			}
		}
	}


	/// <summary>
	/// Create a challenge for the current player object.
	/// </summary>
	/// <returns>The challenge for the player object.</returns>
	/// <param name="currentPlayer">Current player object.</param>
	public string CreateChallenge(GameObject currentPlayer){
		// Initially set ID to 1
		int sharingToPlayerId = 1;
		// Get own ID
		int currentId = currentPlayer.GetComponent<PlayerController> ().ConnectionId;
		// Check if there are more than one people connected otherwise the Challenge will be shared with itself
		// (to avoid infinity loop)
		if (MyNetworkManager.Instance.numPlayers > 1) {
			// Assign random Id to share challenge to
			do {
				sharingToPlayerId = Random.Range (1, MyNetworkManager.Instance.numPlayers + 1);	
			} while (sharingToPlayerId == currentId);
		}

		// Create new Challenge and store it in the current Challenges
		currentChallenges[currentId] = new Challenge ((SharingMode) currentPlayer.GetComponent<PlayerController> ().SharingMode, sharingToPlayerId);

		// Return description of current challenge
		return currentChallenges [currentId].GetDescription ();
	}

	/// <summary>
	/// Called when the file is received to verify if challenge is successful.
	/// <param name="senderId">The player id of the sender.</param>
	/// <param name="receiverId">The player id of the receiver.</param>
	/// <returns>State whether the challenge is successful.</returns>
	/// </summary>
	public bool VerifyChallenge(int senderId, int receiverId){
		// Get Challenge from the stored ones
		Challenge c = currentChallenges [senderId];

		// Check if the receiverId is the same with the receiverId of the challenge
		if (receiverId == c.GetReceiverId()) {
			// Save the endtime and calculate the reaction time
			c.SetEndTime (Time.time);
			DebugGame.text += "Reaction time was: " + c.CalculateReactionTime ()+"\n";

			// Store/Persist Challenge
			_allChallenges[senderId].Add(c);

			// Challenge finished successfully and return true
			return true;
		}
		// Otherwise increase the error rate and return false
		Debug.Log ("Increasing Error rate now");
		c.IncreaseError ();
		return false;
	}

	/// <summary>
	/// Persist the data of all challenges on the local file system.
	/// </summary>
	public void SaveData(){

		// Get data path
		string currentDataPath = Application.persistentDataPath +"/Challenges_"+System.DateTime.Now.ToString("yyyyMMdd_HHmmss")+".json";
		Debug.Log ("Saving file to " + currentDataPath);
		DebugGame.text = "Saving file to " + currentDataPath;

		// Write data from the allChallenges List to file
		FileStream file = File.Create (currentDataPath);
		for (int i = 0; i <= MyNetworkManager.Instance.numPlayers; i++) {
			string challenges =JsonHelper.ToJson (_allChallenges[i].ToArray(), true);
			file.Write ( System.Text.Encoding.UTF8.GetBytes(challenges), 0,  System.Text.Encoding.UTF8.GetBytes(challenges).Length);
		}

		// Close file
		file.Close ();
	}
}
