using UnityEngine;
using System.Collections;

/// <summary>
// UserInterfaceController is responsible for all the interactions between the user and the device
/// </summary>
public class UserInterfaceController : MonoBehaviour {

	public GameObject PlayerObject;	///< Associated PlayerObject which gets the instructions
	public GameObject UI_StartupCanvas;	///< Startup Userinterfaces
	public GameObject UI_Wait;	///< Wait Userinterface
	public GameObject UI_SharingModeCanvas;	///< SharingMode Userinterface
	public GameObject UI_DragNDropCanvas;	///< DragNDrop Userinterface
	public GUISkin CustomGuiSkin;	///< Custom Skin for designing the Buttons
	public string PlayerName;	///< The name of the associated player



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
		UI_StartupCanvas.SetActive (false);

		// Enable SharingMode Canvas
		UI_SharingModeCanvas.SetActive(true);
	}

	// Called when the Toggle DragNDrop is selected
	public void ToggleDragNDropCanvas(bool newValue){
		UI_DragNDropCanvas.SetActive (newValue);
	}

	// Called when the Toggle SwipeShot is selected
	public void ToggleSwipeShot(bool newValue){
		GetComponent<SwipeDetector> ().enabled = newValue;
	}

	// Called once per frame
	void OnGUI(){
		// only call the GUI if it there is a PlayerObject already existing
		if (PlayerObject == null) {
			return;
		}

		GUI.skin = CustomGuiSkin;

		// TODO: Create Canvas on UI
		// Only if there is no Mode selected show the UI for changing the name
		if (!UI_DragNDropCanvas.GetComponent<Canvas> ().enabled && !GetComponent<SwipeDetector> ().enabled) {
			PlayerName = GUI.TextField (new Rect (Screen.width - 425, Screen.height - 250, 400, 200), PlayerName);

			if (GUI.Button (new Rect (Screen.width - 750, Screen.height - 250, 300, 200), "Change")) {
				PlayerObject.GetComponent<PlayerController> ().CmdChangeName (PlayerName);
			}

		}

		if (GUI.Button (new Rect (Screen.width - 750, Screen.height - 550, 300, 200), "Shoot")) {
			PlayerObject.GetComponent<PlayerController> ().CmdShootFile (Vector3.forward);
		}
	}

	// Show trigger when local player receives a File
	public void ShowIncomingFile(GameObject file){
		Debug.Log ("File received");
	}

	// Forwarding Shooting a File
	public void ShootFile (Vector2 direction){
		PlayerObject.GetComponent<PlayerController> ().CmdShootFile (direction);	
	}

}
