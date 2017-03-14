using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
// Class for controlling TouchNChuck Behaviour
/// </summary>
public class TouchNChuck : MonoBehaviour {
	public float LongPressTrigger = 0.5f; ///< Threshold for a long press
	public float ChuckingThresholdZ = 0.7f; ///< Threshold of movment in z-direction
	public GameObject UserInterface; ///< Reference to user interface
	public Text Instructions;	///< Text for the giving instructions


	private float TouchTimeStart; ///< Saving the start of a touch
	private bool camFrozen; ///< current state of the camera stream

	// private vars needed for saving the current camera when freezing it
	private CameraClearFlags currentFlags; ///< current Flags of the camera
	private int currentCullingMask; ///< current culling Mask of the camera

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0) {
			Touch touch = Input.touches [0];
			if (!camFrozen) {
				Ray ray = Camera.main.ScreenPointToRay (touch.position);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					if (hit.collider.gameObject.CompareTag ("Player")) {
						if (touch.phase == TouchPhase.Began) {
							TouchTimeStart = Time.time;
						}
						if (touch.phase == TouchPhase.Stationary) {
							if (Time.time - TouchTimeStart > LongPressTrigger) {
								Instructions.text = "Chuck your device if you want to share with XX";
								StartCoroutine (FreezeCam ());
								camFrozen = true;
							}
						}
					}
				}
			}
			if (camFrozen) {
				if (Input.gyro.userAcceleration.z > ChuckingThresholdZ) {
					StartCoroutine (UnfreezeCam ());
					Instructions.text = "Select a player with a longpress";
					UserInterface.GetComponent<UserInterfaceController> ().ShowSuccessPanel ();
				}
				if (touch.phase == TouchPhase.Ended) {
					StartCoroutine (UnfreezeCam ());
					Instructions.text = "Select a player with a longpress";
				}
			}

		}
	}

	IEnumerator FreezeCam(){
		//yield return null;
		currentFlags = Camera.main.clearFlags;
		currentCullingMask = Camera.main.cullingMask;

		Camera.main.clearFlags = CameraClearFlags.Nothing;
		yield return null;
		Camera.main.cullingMask = 0;
	}


	IEnumerator UnfreezeCam(){
		Camera.main.clearFlags = currentFlags;
		yield return null;
		Camera.main.cullingMask = currentCullingMask;
		camFrozen = false;
	}


}
