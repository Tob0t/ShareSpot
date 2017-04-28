using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// UserInterfaceController is responsible for all the interactions between the user and the device.
/// </summary>
public class UserInterfaceController : MonoBehaviour {

	#region [Public fields]
	public GameObject PlayerObject;	///< Associated PlayerObject which gets the instructions.
	public GameObject UI_StartupPanel;	///< Startup Userinterfaces.
	public GameObject UI_Wait;	///< Wait Userinterface.
	public GameObject UI_SharingModePanel;	///< SharingMode Userinterface.
	public GameObject UI_CalibrationModePanel;	///< CalibrationMode Userinterface.
	public GameObject UI_CalibrationTogglePanel;	///< CalibrationToggle Userinterface.
	public GameObject UI_AdjustDevicePositionPanel;	///< UI_AdjustDevicePositionPanel Userinterface.
	public GameObject UI_AdjustDevicePositionTogglePanel;	///< UI_AdjustDevicePositionTogglePanel Userinterface.
	public GameObject UI_DragNDropPanel;	///< DragNDrop Userinterface.
	public GameObject UI_SwipeShotPanel;	///< SwipeShot Userinterface.
	public GameObject UI_TouchNChuckPanel;	///< TouchNChuck Userinterface.
	public GameObject UI_VerifyReceiverPanel;	///< Verify Receiver Userinterface.
	public GameObject UI_FileIncomingPanel;	///< Incoming File Userinterface.
	public GameObject UI_GamePanel;	///< Game Userinterface.
	public GameObject UI_GameStartPanel;	///< GameStart Userinterface.
	public GameObject UI_GamePointsPanel;	///< GamePoints Userinterface.
	public GameObject UI_ErrorPanel;	///< Error Userinterface.
	public GameObject UI_SuccessPanel;	///< Success Userinterface.
	public GUISkin CustomGuiSkin;	///< Custom Skin for designing the Buttons.
	public string PlayerName;	///< The name of the associated player.
	public Text ChallengeDescription;	///< Text for the description of a new challenge.
	public Text VerifyReceiverDescription; ///< Text for verifying the receiver.
	public Text CurrentDevicePosition; ///< Current Position of the device
	public Text PlayerPoints; ///< Current Points of the player

	public GameObject IncomingFile; ///< GameObject of any incoming file.


	public GameObject GameManager;	///< Reference to the GameManger.

	#endregion

	#region [Private fields]
	private GameObject _sendingFile; ///< GameObject of any outcoming file.
	private int _fileReceiverId; ///< The id of the receiver of the sending file.

	#endregion


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	/// <summary>
	/// Changes the name of the player by calling a command on the server.
	/// </summary>
	/// <param name="newName">New name.</param>
	public void ChangeName(string newName){
		if (PlayerObject != null) {
			PlayerObject.GetComponent<PlayerController> ().CmdChangeName (newName);
		}
	}

	/// <summary>
	/// Adds reference to the player controller.
	/// </summary>
	/// <param name="callingPlayerObject">Calling player object.</param>
	public void AddPlayerController(GameObject callingPlayerObject){
		// Setting the ControlledPlayer object
		PlayerObject = callingPlayerObject;
	}

	/// <summary>
	/// Initial setup, only done once.
	/// </summary>
	public void InitialSetup(){

		// Disable previous UI Panels
		UI_Wait.SetActive (false);
		UI_StartupPanel.SetActive (false);

		// Enable SharingMode Panel
		UI_SharingModePanel.SetActive(true);

		// Enable Calibration and Device Toggle Button
		UI_CalibrationTogglePanel.SetActive (true);
		UI_AdjustDevicePositionTogglePanel.SetActive (true);
	}

	/// <summary>
	/// Toggles the SharingMode panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleSharingModePanel(bool newValue){
		UI_SharingModePanel.SetActive (newValue);
	}

	/// <summary>
	/// Toggles the Calibration panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleCalibrationModePanel(bool newValue){
		UI_CalibrationModePanel.SetActive (newValue);
	}

	/// <summary>
	/// Toggles the Device position panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleDevicePositionPanel(bool newValue){
		UI_AdjustDevicePositionPanel.SetActive (newValue);
	}

	/// <summary>
	/// Toggles the DragNDrop panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleDragNDropPanel(bool newValue){
		UI_DragNDropPanel.SetActive (newValue);
	}

	/// <summary>
	/// Toggles the SwipeShot panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleSwipeShotPanel(bool newValue){
		UI_SwipeShotPanel.SetActive(newValue);
	}

	/// <summary>
	/// Toggles the TouchNChuck panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleTouchNChuckPanel(bool newValue){
		UI_TouchNChuckPanel.SetActive(newValue);
	}

	/// <summary>
	/// Toggles the file incoming panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleFileIncomingPanel(bool newValue){
		UI_FileIncomingPanel.SetActive (newValue);
	}

	/// <summary>
	/// Toggles the verify receiver panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleVerifyReceiverPanel(bool newValue){
		UI_VerifyReceiverPanel.SetActive (newValue);
	}

	/// <summary>
	/// Toggles the game points panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleGamePointsPanel(bool newValue){
		UI_GamePointsPanel.SetActive (newValue);
	}

	/// <summary>
	/// Disables all game panels except the Startup ones.
	/// </summary>
	public void DisableAllGamePanels(){
		ToggleSharingModePanel(false);
		ToggleCalibrationModePanel (false);
		ToggleDevicePositionPanel (false);
		ToggleDragNDropPanel(false);
		ToggleSwipeShotPanel(false);
		ToggleTouchNChuckPanel(false);
		ToggleVerifyReceiverPanel(false);
		ToggleFileIncomingPanel(false);
		ToggleGamePanel (false);
		ToggleGamePointsPanel (false);
		//UI_CalibrationTogglePanel.SetActive (false);
		UI_ErrorPanel.SetActive (false);
		UI_SuccessPanel.SetActive (false);
	}

	/// <summary>
	/// Toggles the individual sharing mode panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void TogglePlayersSharingModePanel(bool newValue){
		SharingMode sharingMode = (SharingMode) PlayerObject.GetComponent<PlayerController> ().SharingMode;
		switch (sharingMode) {
		case SharingMode.DragNDrop:
			ToggleDragNDropPanel (newValue);
			break;
		case SharingMode.SwipeShot:
			ToggleSwipeShotPanel (newValue);
			break;
		case SharingMode.TouchNChuck:
			ToggleTouchNChuckPanel (newValue);
			break;
		default:
			break;
		}
	}

	#region[Device Calibration]
	/// <summary>
	/// Recalibrates the device by calling function on player controller.
	/// </summary>
	/// <param name="angle">Angle in degrees.</param>
	public void RecalibrateDevice(int angle){
		PlayerObject.GetComponent<PlayerController> ().RecalibrateDevice (angle);

		// Disable the panel
		ToggleCalibrationModePanel (false);

		// Disable the toggle button programmatically
		UI_CalibrationTogglePanel.GetComponentsInChildren<Image> () [1].color = Color.clear;
		UI_CalibrationTogglePanel.GetComponentInChildren<Toggle> ().isOn = false;
	}


	/// <summary>
	/// Adjusts the height of the device location to the player.
	/// </summary>
	/// <param name="adjustingValue">Adjusting value in y direction.</param>
	public void AdjustDeviceLocationHeight(float adjustingValue){
		adjustDeviceLocation(new Vector3 (0, adjustingValue, 0));
	}

	/// <summary>
	/// Adjusts the device location closeness to the player.
	/// </summary>
	/// <param name="adjustingValue">Adjusting value in z direction.</param>
	public void AdjustDeviceLocationCloseness(float adjustingValue){
		adjustDeviceLocation(new Vector3 (0, 0, adjustingValue));
	}

	/// <summary>
	/// Resets the device location.
	/// </summary>
	public void ResetDeviceLocation(){
		PlayerObject.GetComponent<PlayerController> ().PlayerHand.transform.localPosition = PlayerObject.GetComponent<PlayerController> ().PlayerHandInitialPos;
		showCurrentPosition ();
	}

	/// <summary>
	/// Adjusts the device location.
	/// </summary>
	/// <param name="adjustingVector">Adjusting vector.</param>
	private void adjustDeviceLocation(Vector3 adjustingVector){
		PlayerObject.GetComponent<PlayerController> ().PlayerHand.transform.Translate (adjustingVector,PlayerObject.transform);
		showCurrentPosition ();
	}

	/// <summary>
	/// Shows the current position of the players device relative to the player.
	/// </summary>
	private void showCurrentPosition(){
		// scaleVector needed for calculations to cm (half of the maximum)
		Vector3 scaleVector = new Vector3 (0, 90, 20);

		// Calculate local position in cm
		Vector3 localPositionInCm = Vector3.Scale(PlayerObject.GetComponent<PlayerController> ().PlayerHand.transform.localPosition, scaleVector) + scaleVector;

		CurrentDevicePosition.text = "Current pos " + localPositionInCm.ToString();
		//CurrentDevicePosition.text += "Abs pos " +  PlayerObject.GetComponent<PlayerController> ().PlayerHand.transform.position.ToString();
	}
	#endregion

	/// <summary>
	/// Toggles the game panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ToggleGamePanel(bool newValue){
		UI_GamePanel.SetActive (newValue);
	}

	/// <summary>
	/// Toggles the game start panel.
	/// </summary>
	/// <param name="newValue">Indicates whether it should be activated or deactivated.</param>
	public void ShowGameStartPanel(){
		UI_GameStartPanel.SetActive (true);
		// Show Points panel
		ToggleGamePointsPanel(true);
		StartCoroutine(DisableAfterSomeTime(2f));
	}

	/// <summary>
	/// Displays a new challenge.
	/// </summary>
	/// <param name="description">Description of the new challenge.</param>
	public void ShowNewChallenge(string description){
		// Disable Game points during a challenge
		ToggleGamePointsPanel (false);

		ToggleGamePanel (true);
		ChallengeDescription.text = description;

	}

	/// <summary>
	/// Shows the error panel.
	/// </summary>
	public void ShowErrorPanel(){
		UI_ErrorPanel.SetActive (true);
		StartCoroutine(DisableAfterSomeTime(3f));
	}

	/// <summary>
	/// Shows the success panel.
	/// </summary>
	public void ShowSuccessPanel(){
		UI_SuccessPanel.SetActive (true);
		StartCoroutine(DisableAfterSomeTime(3f));
	}

	/// <summary>
	/// Adapting the panels depending on the result of the challenge.
	/// </summary>
	/// <param name="challengeState">Indicates whether the challenge is successful or failed.</param>
	public void AdaptPanels(bool challengeState){
		if (challengeState) {
			// Show success panel
			ShowSuccessPanel ();

			// Disable the instruction panel
			ToggleGamePanel (false);

			// Disable the sharing mode panel
			TogglePlayersSharingModePanel(false);

			// Show Points panel
			ToggleGamePointsPanel(true);
		} else {
			// Show error panel
			ShowErrorPanel ();
		}
	}

	/// <summary>
	/// Disables the panels after some time.
	/// </summary>
	/// <param name="seconds">The amount in seconds until the panel gets disabled.</param>
	/// <returns>Waiting time in seconds</returns>
	IEnumerator DisableAfterSomeTime(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		UI_ErrorPanel.SetActive (false);
		UI_SuccessPanel.SetActive (false);
		UI_GameStartPanel.SetActive (false);
	}

	/// <summary>
	/// Updates the player points on the UI.
	/// </summary>
	/// <param name="CollectedPickups">Amount of collected pickups.</param>
	public void UpdatePlayerPoints (int CollectedPickups){
		PlayerPoints.text = CollectedPickups.ToString();
	}

	/// <summary>
	/// Raises the GUI event.
	/// </summary>
	void OnGUI(){
		// only call the GUI if it there is a PlayerObject already existing
		if (PlayerObject == null) {
			return;
		}

		GUI.skin = CustomGuiSkin;

		// TODO: Create Panel on UI
		// Only if there is no Mode selected show the UI for changing the name
		if (!UI_DragNDropPanel.activeSelf && !UI_SwipeShotPanel.activeSelf && !UI_TouchNChuckPanel.activeSelf) {
			/*PlayerName = GUI.TextField (new Rect (Screen.width - 425, Screen.height - 250, 400, 200), PlayerName);

			if (GUI.Button (new Rect (Screen.width - 750, Screen.height - 250, 300, 200), "Change")) {
				PlayerObject.GetComponent<PlayerController> ().CmdChangeName (PlayerName);
			}*/
		}
	}

	/// <summary>
	/// Activates a panel to show the incoming file.
	/// </summary>
	/// <param name="file">Incoming file.</param>
	public void ShowIncomingFile(GameObject file){
		Debug.Log ("File incoming");
		ToggleFileIncomingPanel (true);
		IncomingFile = file;
	}

	/// <summary>
	/// Forwarding Swipeshot to server
	/// </summary>
	/// <param name="force">Force as 3D-Vector.</param>
	public void SwipeShot (Vector3 force){
		PlayerObject.GetComponent<PlayerController> ().CmdShootFile (force);	
	}

	/// <summary>
	/// Accepts the incoming file and disables the panel.
	/// </summary>
	public void AcceptIncomingFile(){
		Debug.Log ("Incoming File accepted");
		ToggleFileIncomingPanel (false);
		Debug.Log ("Name: " + IncomingFile.GetComponent<SharedFile> ().name);
		Debug.Log ("Author: " + IncomingFile.GetComponent<SharedFile> ().Author);
		Debug.Log ("Size: " + IncomingFile.GetComponent<SharedFile> ().Size);
	}

	/// <summary>
	/// Declines the incoming file and disables the panel
	/// </summary>
	public void DeclineIncomingFile(){
		Debug.Log ("Incoming File declined");
		ToggleFileIncomingPanel (false);
	}


	/// <summary>
	/// Activates a panel to show the verify receiver panel.
	/// </summary>
	/// <param name="file">File to sent.</param>
	/// <param name="receiverId">Iidentifier of the receiving client.</param>
	/// <param name="playerName">Player name of the receiving client.</param>
	public void ShowVerifyReceiverPanel(GameObject file, int receiverId, string playerName){
		_sendingFile = file;
		_fileReceiverId = receiverId;
		VerifyReceiverDescription.text = "Sharing file with " + playerName + "?";
		ToggleVerifyReceiverPanel (true);
	}

	/// <summary>
	/// Verifies the receiver.
	/// </summary>
	/// <param name="success">If set to <c>true</c> the positive button was clicked.</param>
	public void VerifyReceiver(bool success){
		// Disable verify receiver panel
		ToggleVerifyReceiverPanel (false);
		// sent command to continue with receiving client
		if (success) {
			PlayerObject.GetComponent<PlayerController>().CmdReceiveFile (_sendingFile, _sendingFile.GetComponent<SharedFile> ().SourceId, _fileReceiverId);
		}
	}

}
