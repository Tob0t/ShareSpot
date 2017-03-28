using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Class for controlling TouchNChuck Behaviour.
/// </summary>
public class TouchNChuck : MonoBehaviour {

	#region [Public fields]
	public float LongPressTrigger = 0.5f; ///< Threshold for a long press.
	public float ChuckingThresholdZ = 0.7f; ///< Threshold of movment of the device in z-direction.
	public GameObject UserInterface; ///< Reference to the user interface.
	public Text Instructions;	///< Text for giving instructions.
	public string InitInstructions = "Select a player with a longpress"; ///< Initial instructions in textbox.
	public string ChuckInstructions = "Chuck your device if you want to share with "; ///< Chucking instructions in textbox.
	public GameObject FileToShare; ///< Reference to the file to share.

	#endregion

	#region [Private fields]
	private GameObject _hitObject; ///< The hitobject which is marked.
	private float _touchTimeStart; ///< Saving the start time of a touch.
	private bool _camFrozen = false; ///< Current state of the camera stream, set it initially to false.

	// camera variables needed for saving the current camera when freezing it
	private CameraClearFlags _currentFlags; ///< The current flags of the camera.
	private int currentCullingMask; ///< The current culling mask of the camera.

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
			if (!_camFrozen) {

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
							_touchTimeStart = Time.time;
						}

						// If the touch is on a constant point
						if (touch.phase == TouchPhase.Stationary) {

							// Compare the current time with the start time and check if it is a long press
							if (Time.time - _touchTimeStart > LongPressTrigger) {

								// set the hit object ot the pressed one
								_hitObject =  hit.collider.gameObject;

								// Change the instructions text field
								Instructions.text = ChuckInstructions + _hitObject.GetComponent<PlayerController>().PlayerName;

								// Start the coroutine to freeze the camera and set the boolean to true
								StartCoroutine (FreezeCam ());

								// TODO: Move maybe to coroutine? Testing!!!
								_camFrozen = true;
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
					p.CmdReceiveFile (FileToShare, p.ConnectionId ,_hitObject.GetComponent<PlayerController> ().ConnectionId);
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
	/// Enumerator to freeze the camera.
	/// </summary>
	/// <returns>Returns null to jump to next frame.</returns>
	IEnumerator FreezeCam(){
		//yield return null;

		// change color of instructions to black
		GetComponentInChildren<Text>().color = Color.black;

		_currentFlags = Camera.main.clearFlags;
		currentCullingMask = Camera.main.cullingMask;

		Camera.main.clearFlags = CameraClearFlags.Nothing;
		yield return null;
		Camera.main.cullingMask = 0;
	}

	/// <summary>
	/// Enumerator to unfreeze the camera.
	/// </summary>
	/// <returns>Return null to jump to the next frame.</returns>
	IEnumerator UnfreezeCam(){
		Camera.main.clearFlags = _currentFlags;
		yield return null;
		Camera.main.cullingMask = currentCullingMask;

		// change color of instructions back to white
		GetComponentInChildren<Text>().color = Color.white;

		_camFrozen = false;
	}


}
