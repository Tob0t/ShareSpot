using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Vuforia;

/// <summary>
/// Class for controlling TouchNChuck Behaviour.
/// </summary>
public class TouchNChuck : MonoBehaviour {

	#region [Public fields]
	public float LongPressTrigger = 0.5f; ///< Threshold for a long press.
	public float ChuckingThresholdZ = 0.7f; ///< Threshold of movment of the device in z-direction.
	public float ChuckingMinimumErrorThresholdZ = 0.2f; ///< Threshold of movment of the device in z-direction to detect an error.
	public Text Instructions;	///< Text for giving instructions.
	public string InitInstructions = "Select a player with a longpress"; ///< Initial instructions in textbox.
	public string ChuckInstructions = "Chuck your device if you want to share with "; ///< Chucking instructions in textbox.
	public GameObject ChallengeInstructions; ///< Reference to the text of the game panel
	public GameObject FileToShare; ///< Reference to the file to share. // TODO: use the file
	public GameObject UserInterface; ///< Reference to the user interface.


	#endregion

	#region [Private fields]
	private GameObject _hitObject; ///< The hitobject which is marked.
	private float _touchTimeStart; ///< Saving the start time of a touch.
	private bool _weakChuckingDetectorLocked = false; ///< mutex to lock the weak chucking for some time to avoid multiple errors
	private bool _camFrozen = false; ///< Current state of the camera stream, set it initially to false.

	// camera variables needed for saving the current camera when freezing it
	private CameraClearFlags _currentFlags; ///< The current flags of the camera.
	private int _currentCullingMask; ///< The current culling mask of the camera.

	#endregion


	// Use this for initialization
	void Start () {
		//Instructions.text = InitInstructions;
	}
	
	// Update is called once per frame
	void Update () {
		
		// Check if Challenge is active
		if(UserInterface != null && UserInterface.GetComponent<UserInterfaceController> ().TouchNChuckEnabled){
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

						// Find out the current touch phase
						// If the touch phase just started save the current time
						if (touch.phase == TouchPhase.Began) {
							_touchTimeStart = Time.time;
						}

						// If the touch is on a constant point
						if (touch.phase == TouchPhase.Stationary) {

							// Compare the current time with the start time and check if it is a long press
							if (Time.time - _touchTimeStart > LongPressTrigger) {

								// Check if a player object was touched
								if (hit.collider.gameObject.CompareTag ("Player")) {
									
									// set the hit object ot the pressed one
									_hitObject = hit.collider.gameObject;

									// Change the instructions text field
									Instructions.text = ChuckInstructions + _hitObject.GetComponent<PlayerController> ().PlayerName;

									// Start the coroutine to freeze the camera and set the boolean to true
									StartCoroutine (FreezeCam ());

									// Freeze cam
									_camFrozen = true;
								} 
							}
						}
						if (touch.phase == TouchPhase.Ended) {
							// Compare the current time with the start time and check if it is a long press
							if (Time.time - _touchTimeStart > LongPressTrigger) {
								// Increase error rate if long press on empty space
								IncreaseErrorRate();
							}
						}



					}
				} else {	// If camera is currently frozen
					// if the touch phase is ended unfreeze the camera and reset the instructions text
					if (touch.phase == TouchPhase.Ended) {

						// Start the coroutine to unfreeze the camera
						StartCoroutine (UnfreezeCam ());

						// Unfreeze cam
						_camFrozen = false;

						// Reset the instructions text and show the successpanel
						Instructions.text = InitInstructions;

						// Increase error rate if long press is interrupted
						IncreaseErrorRate();

					} else {
						// Check if there is a chucking performed -> Successfull
						if (Input.gyro.userAcceleration.z > ChuckingThresholdZ) {

							// Start the coroutine to unfreeze the camera
							StartCoroutine (UnfreezeCam ());

							// Unfreeze cam
							_camFrozen = false;

							// Reset the instructions text and show the successpanel
							Instructions.text = InitInstructions;
							//UserInterface.GetComponent<UserInterfaceController> ().ShowSuccessPanel ();

							// Call Command on the local Player
							PlayerController p = UserInterface.GetComponent<UserInterfaceController> ().PlayerObject.GetComponent <PlayerController> ();
							p.CmdReceiveFile (FileToShare, p.ConnectionId, _hitObject.GetComponent<PlayerController> ().ConnectionId);

							// set TouchNChuck to false to avoid any more wrong errors
							UserInterface.GetComponent<UserInterfaceController> ().TouchNChuckEnabled = false;

						} else if (Input.gyro.userAcceleration.z > ChuckingMinimumErrorThresholdZ) {
							if (!_weakChuckingDetectorLocked) {
								// Start coroutine to unlock a weak chucking detection after 2 seconds
								StartCoroutine (UnlockWeakChuckingDetectorAfterSeconds (2f));

								// Increase error rate if chucking is too weak
								IncreaseErrorRate();

								// Lock chucking detector
								_weakChuckingDetectorLocked = true;
							}
						}
					}
				}
			}

		}
	}
		
	/// <summary>
	/// Enumerator to freeze the camera.
	/// </summary>
	/// <returns>Returns null to jump to next frame.</returns>
	IEnumerator FreezeCam(){
		yield return new WaitForEndOfFrame ();

		// change color of instructions to black
		GetComponentInChildren<Text>().color = Color.black;
		ChallengeInstructions.GetComponentInChildren<Text> ().color = Color.black;

		// Get width
		int width = Screen.width;
		int height = Screen.height;

		// Create new texture
		Texture2D overlayTexture = new Texture2D( width, height, TextureFormat.RGB24, false );
		overlayTexture.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		overlayTexture.Apply();

		// Create new render texture and apply it to current camera
		RenderTexture rt = new RenderTexture (width, height, 24);
		Camera.main.targetTexture = rt;
		Camera.current.Render ();

		///  Old method what is sometimes buggy in game mode
		/*
		_currentFlags = Camera.main.clearFlags;
		_currentCullingMask = Camera.main.cullingMask;

		Camera.main.clearFlags = CameraClearFlags.Nothing;
		yield return null;
		Camera.main.cullingMask = 0;*/
	}

	/// <summary>
	/// Enumerator to unfreeze the camera.
	/// </summary>
	/// <returns>Return null to jump to the next frame.</returns>
	IEnumerator UnfreezeCam(){

		yield return null;
		// (Re)set target Texture to null
		Camera.current.targetTexture = null;

		// change color of instructions back to white
		GetComponentInChildren<Text>().color = Color.white;
		ChallengeInstructions.GetComponentInChildren<Text> ().color = Color.white;

		///  Old method what is sometimes buggy in game mode
		/*
		if(_currentFlags != null)
			Camera.main.clearFlags = _currentFlags;
		yield return null;
		if(_currentCullingMask != null)
			Camera.main.cullingMask = _currentCullingMask;
		yield return null;

		*/

		_camFrozen = false;
	}

	/// <summary>
	/// Unlocks the detector of some seconds
	/// </summary>
	IEnumerator UnlockWeakChuckingDetectorAfterSeconds(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		_weakChuckingDetectorLocked = false;
	}

	/// <summary>
	/// Increases the error rate.
	/// </summary>
	private void IncreaseErrorRate(){
		if (UserInterface.GetComponent<UserInterfaceController> ().TouchNChuckEnabled) {
			PlayerController p = UserInterface.GetComponent<UserInterfaceController> ().PlayerObject.GetComponent <PlayerController> ();
			p.CmdIncreaseError (p.ConnectionId);
		}
	}

}
