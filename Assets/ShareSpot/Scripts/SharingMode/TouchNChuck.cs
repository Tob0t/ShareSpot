using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Class for controlling TouchNChuck Behaviour
/// </summary>
public class TouchNChuck : MonoBehaviour {

	#region [Public fields]
	public float LongPressTrigger = 0.5f; ///< Threshold for a long press
	public float ChuckingThresholdZ = 0.7f; ///< Threshold of movment in z-direction
	public GameObject UserInterface; ///< Reference to user interface
	public Text Instructions;	///< Text for the giving instructions
	public string InitInstructions = "Select a player with a longpress"; ///< initial instructions in textbox
	public string ChuckInstructions = "Chuck your device if you want to share with "; ///< chucking instructions in textbox
	public GameObject FilteToShare; ///< Reference to the file to share

	#endregion

	#region [Private fields]
	private GameObject hitObject; ///< the hitobject which is marked
	private float TouchTimeStart; ///< Saving the start of a touch
	private bool camFrozen = false; ///< current state of the camera stream, set it initially to false

	// private vars needed for saving the current camera when freezing it
	private CameraClearFlags currentFlags; ///< current Flags of the camera
	private int currentCullingMask; ///< current culling Mask of the camera

	#endregion


	// Use this for initialization
	void Start () {
		//Instructions.text = InitInstructions;
	}
	
	// Update is called once per frame
	void Update () {
		// Check if there was a touch detected
		if (Input.touchCount > 0) {

			// Only investigate the first touch
			Touch touch = Input.touches [0];

			// check if cam is already frozen
			if (!camFrozen) {

				// Convert touched point on device screen to the virtual environment
				Ray ray = Camera.main.ScreenPointToRay (touch.position);
				RaycastHit hit;

				// Check if there is a collision
				if (Physics.Raycast (ray, out hit)) {

					// Check if a player object was touched
					if (hit.collider.gameObject.CompareTag ("Player")) {

						// Find out the current touch phase
						// If the touch phase just started save the current time
						if (touch.phase == TouchPhase.Began) {
							TouchTimeStart = Time.time;
						}

						// If the touch is on a constant point
						if (touch.phase == TouchPhase.Stationary) {

							// Compare the current time with the start time and check if its greater than a long press
							if (Time.time - TouchTimeStart > LongPressTrigger) {

								// set the hit object ot the pressed one
								hitObject =  hit.collider.gameObject;

								// Change the instructions text field
								Instructions.text = ChuckInstructions + hitObject.GetComponent<PlayerController>().PlayerName;

								// Start the coroutine to freeze the camera and set the boolean to true
								StartCoroutine (FreezeCam ());
								camFrozen = true;
							}
						}
					}
				}
			} else{
				// If camera is currently frozen
				// Check if there is a chucking performed -> Successfull
				if (Input.gyro.userAcceleration.z > ChuckingThresholdZ) {

					// Start the coroutine to unfreeze the camera
					StartCoroutine (UnfreezeCam ());

					// Reset the instructions text and show the successpanel
					Instructions.text = InitInstructions;
					//UserInterface.GetComponent<UserInterfaceController> ().ShowSuccessPanel ();

					// Call Command on the local Player
					PlayerController p = UserInterface.GetComponent<UserInterfaceController> ().PlayerObject.GetComponent <PlayerController> ();
					p.CmdReceiveFile (FilteToShare, p.ConnectionId ,hitObject.GetComponent<PlayerController> ().ConnectionId);
				}
				// if the touch phase is ended also unfreeze the camera and reset the instructions text
				if (touch.phase == TouchPhase.Ended) {
					StartCoroutine (UnfreezeCam ());
					Instructions.text = InitInstructions;
				}
			}

		}
	}

	/// <summary>
	/// Enumerator to freeze the camera
	/// </summary>
	IEnumerator FreezeCam(){
		//yield return null;
		currentFlags = Camera.main.clearFlags;
		currentCullingMask = Camera.main.cullingMask;

		Camera.main.clearFlags = CameraClearFlags.Nothing;
		yield return null;
		Camera.main.cullingMask = 0;
	}

	/// <summary>
	/// Enumerator to unfreeze the camera
	/// </summary>
	IEnumerator UnfreezeCam(){
		Camera.main.clearFlags = currentFlags;
		yield return null;
		Camera.main.cullingMask = currentCullingMask;
		camFrozen = false;
	}


}
