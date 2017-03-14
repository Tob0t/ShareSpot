using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
// UserInterfaceController is responsible for all the interactions between the user and the device
/// </summary>
public class UserInterfaceController : MonoBehaviour {

	public GameObject PlayerObject;	///< Associated PlayerObject which gets the instructions
	public GameObject UI_StartupPanel;	///< Startup Userinterfaces
	public GameObject UI_Wait;	///< Wait Userinterface
	public GameObject UI_SharingModePanel;	///< SharingMode Userinterface
	public GameObject UI_DragNDropPanel;	///< DragNDrop Userinterface
	public GameObject UI_SwipeShotPanel;	///< SwipeShot Userinterface
	public GameObject UI_TouchNChuckPanel;	///< TouchNChuck Userinterface
	public GameObject UI_FileIncomingPanel;	///< Incoming File Userinterface
	public GameObject UI_GamePanel;	///< Game Userinterface
	public GameObject UI_ErrorPanel;	///< Error Userinterface
	public GameObject UI_SuccessPanel;	///< Success Userinterface
	public GUISkin CustomGuiSkin;	///< Custom Skin for designing the Buttons
	public string PlayerName;	///< The name of the associated player
	public Text ChallengeDescription;	///< Text for the description of a new challenge

	public GameObject IncomingFile; ///< GameObject of any incoming file



	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Initial setup only done once
	public void InitialSetup(GameObject CallingGameObject){
		// Setting the ControlledPlayer object
		PlayerObject = CallingGameObject;

		// Disable previous UI Panels
		UI_Wait.SetActive (false);
		UI_StartupPanel.SetActive (false);

		// Enable SharingMode Panel
		UI_SharingModePanel.SetActive(true);
	}

	// Called when the Toggle DragNDrop is selected
	public void ToggleDragNDropPanel(bool newValue){
		UI_DragNDropPanel.SetActive (newValue);
	}

	// Called when the Toggle SwipeShot is selected
	public void ToggleSwipeShotPanel(bool newValue){
		UI_SwipeShotPanel.SetActive(newValue);
	}

	// Called when the Toggle TouchNChuck is selected
	public void ToggleTouchNChuckPanel(bool newValue){
		UI_TouchNChuckPanel.SetActive(newValue);
	}

	// Called when the Toggle DragNDrop is selected
	public void ToggleFileIncomingPanel(bool newValue){
		UI_FileIncomingPanel.SetActive (newValue);
	}

	// Disabel all Panels except the Startup ones
	public void DisableAllGamePanels(){
		UI_SharingModePanel.SetActive(false);
		ToggleDragNDropPanel(false);
		ToggleSwipeShotPanel(false);
		ToggleTouchNChuckPanel(false);
		ToggleFileIncomingPanel(false);
		ToggleGamePanel (false);
		UI_ErrorPanel.SetActive (false);
		UI_SuccessPanel.SetActive (false);
	}

	public void ToggleSharingModePanel(bool newValue){
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
		

	// Called when the Toggle Game is selected
	public void ToggleGamePanel(bool newValue){
		UI_GamePanel.SetActive (newValue);
	}

	// Display a new challenge
	public void ShowNewChallenge(string description){
		ToggleGamePanel (true);
		ChallengeDescription.text = description;
	}

	// Show Error
	public void ShowErrorPanel(){
		UI_ErrorPanel.SetActive (true);
		StartCoroutine(DisableAfterSomeTime(3f));
	}

	// Show Error
	public void ShowSuccessPanel(){
		UI_SuccessPanel.SetActive (true);
		StartCoroutine(DisableAfterSomeTime(3f));
	}

	// Disable the Error Panel after 3 seconds
	IEnumerator DisableAfterSomeTime(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		UI_ErrorPanel.SetActive (false);
		UI_SuccessPanel.SetActive (false);
	}

	// Called once per frame
	void OnGUI(){
		// only call the GUI if it there is a PlayerObject already existing
		if (PlayerObject == null) {
			return;
		}

		GUI.skin = CustomGuiSkin;

		// TODO: Create Panel on UI
		// Only if there is no Mode selected show the UI for changing the name
		if (!UI_DragNDropPanel.activeSelf && !UI_SwipeShotPanel.activeSelf && !UI_TouchNChuckPanel.activeSelf) {
			PlayerName = GUI.TextField (new Rect (Screen.width - 425, Screen.height - 250, 400, 200), PlayerName);

			if (GUI.Button (new Rect (Screen.width - 750, Screen.height - 250, 300, 200), "Change")) {
				PlayerObject.GetComponent<PlayerController> ().CmdChangeName (PlayerName);
			}
		}
	}

	// Show trigger when local player receives a File
	public void ShowIncomingFile(GameObject file){
		Debug.Log ("File incoming");
		ToggleFileIncomingPanel (true);
		IncomingFile = file;
	}

	// Forwarding Shooting a File
	public void SwipeShot (Vector3 force){
		PlayerObject.GetComponent<PlayerController> ().CmdShootFile (force);	
	}

	// Method for accepting incoming file
	public void AcceptIncomingFile(){
		Debug.Log ("Incoming File accepted");
		ToggleFileIncomingPanel (false);
		Debug.Log ("Name: " + IncomingFile.GetComponent<SharedFile> ().name);
		Debug.Log ("Author: " + IncomingFile.GetComponent<SharedFile> ().Author);
		Debug.Log ("Size: " + IncomingFile.GetComponent<SharedFile> ().size);
	}

	// Method for declining incoming file
	public void DeclineIncomingFile(){
		Debug.Log ("Incoming File declined");
		ToggleFileIncomingPanel (false);
	}

}
