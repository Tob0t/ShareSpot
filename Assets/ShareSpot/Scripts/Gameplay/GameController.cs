using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.IO;

/// <summary>
// GameController is responsible for managing the Gameplay and only executed on the Server
/// </summary>
public class GameController : NetworkBehaviour {

	public GameObject PickupPrefab;
	public float minX = 100f;
	public float maxX = 1820f;
	public float minY = 80f;
	public float maxY = 120f;
	public float minZ = 100f;
	public float maxZ = 980f;

	public Color[] collectingColor;
	public Challenge[] currentChallenges;

	public int MaxRepetitions = 10;

	public GameObject ButtonStartGame;
	public GameObject ButtonSaveData;
	public GameObject ButtonDragNDrop;
	public GameObject ButtonSwipeShot;
	public GameObject ButtonTouchNChuck;

	public Text DebugGame; ///< Debugging the actions during the game

	//private Challenge[] allChallenges;
	private List<Challenge>[] allChallenges;

	// Use this for initialization
	void Start () {
		// disable Start Game Button first
		//ButtonStartGame.SetActive (false);

		currentChallenges = new Challenge[Admin.Instance.MaxClients+1];
		allChallenges = new List<Challenge>[Admin.Instance.MaxClients+1];

		collectingColor = new Color[Admin.Instance.MaxClients];
		collectingColor [0] = Color.red;
		collectingColor [1] = Color.green;
		collectingColor [2] = Color.blue;
		collectingColor [3] = Color.yellow;
		collectingColor [4] = Color.magenta;
		collectingColor [5] = Color.cyan;
		collectingColor [6] = Color.white;
		collectingColor [7] = Color.black;
		collectingColor [8] = Color.gray;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	// Helper function to toggle all the sharing mode buttons
	/// </summary>
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
	// Assign Sharing Mode to selected Player object
	/// </summary>
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
		Admin.Instance.CurrentTrackedPlayer.GetComponent<PlayerController> ().SharingMode = sharingMode;
		DebugGame.text += "Assigned SharingMode " + (SharingMode) sharingMode + " to " + Admin.Instance.CurrentTrackedPlayer.GetComponent<PlayerController> ().PlayerName +"\n";

		// Remove Reference
		Admin.Instance.CurrentTrackedPlayer = null;
	}

	/// <summary>
	/// Starting the game and initializing the pickups
	/// </summary>
	public void StartGame(){
		// Debugging on the server
		DebugGame.text += "Start Game\n";
		DebugGame.text += "Connected Players: "+MyNetworkManager.Instance.numPlayers +"\n";

		for (int j = 0; j <= MyNetworkManager.Instance.numPlayers; j++) {
			allChallenges [j] = new List<Challenge> ();
		}

		// Loop to produce the Pickup prefabs
		int i=0;
		foreach (GameObject connectedClient in Admin.Instance.ConnectedClients) {
			if (connectedClient != null) {
				// Create the pickup from the file Prefab
				Vector3 randomLocation = new Vector3 (Random.Range (minX, maxX), Random.Range (minY, maxY), Random.Range (minZ, maxZ));
				GameObject pickup = (GameObject)Instantiate (PickupPrefab, randomLocation, Quaternion.identity);
				// Add the color of the pickup to the Controller
				pickup.GetComponent<PickupController> ().ChosenColor = collectingColor [i];
				// Set who is allowed to see the Pickup
				pickup.GetComponent<PickupController>().ValidForConnectionId = i;
				// Spawn the file on the Clients
				NetworkServer.Spawn (pickup);
			}
			i++;
		}

		// Loop to disable the Pickups of other players
		foreach (GameObject connectedClient in Admin.Instance.ConnectedClients) {
			if (connectedClient != null) {
				connectedClient.GetComponent<PlayerController>().RpcShowOnlyOwnPickups ();	
			}
		}
	}


	/// <summary>
	/// Create a challenge for the current player object
	/// </summary>
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
	/// Called when the file is received to verify if challenge is successful
	/// </summary>
	public bool VerifyChallenge(int senderId, int receiverId){
		// Get Challenge from the stored ones
		Challenge c = currentChallenges [senderId];

		// Check if the receiverId is the same with the receiverId of the challenge
		if (receiverId == c.GetReceiverId()) {
			// Save the endtime and calculate the reaction time
			c.SetEndTime (Time.time);
			Debug.Log ("Reaction time was: "+c.CalculateReactionTime());
			DebugGame.text += "Reaction time was: " + c.CalculateReactionTime ()+"\n";

			// Store/Persist Challenge
			allChallenges[senderId].Add(c);

			// Challenge finished successfully and return true
			return true;
		}
		// Otherwise increase the error rate and return false
		Debug.Log ("Increasing Error rate now");
		c.IncreaseError ();
		return false;
	}

	/// <summary>
	/// Persist the data of all challenges on the local file system
	/// </summary>
	public void SaveData(){

		string currentDataPath = Application.persistentDataPath +"/Challenges_"+System.DateTime.Now.ToString("yyyymmdd_HHmm")+".json";
		Debug.Log ("Saving file to " + currentDataPath);
		DebugGame.text = "Saving file to " + currentDataPath;

		FileStream file = File.Create (currentDataPath);
		for (int i = 0; i <= MyNetworkManager.Instance.numPlayers; i++) {
			string challenges =JsonHelper.ToJson (allChallenges[i].ToArray(), true);
			file.Write ( System.Text.Encoding.UTF8.GetBytes(challenges), 0,  System.Text.Encoding.UTF8.GetBytes(challenges).Length);
		}
		file.Close ();
	}
}
