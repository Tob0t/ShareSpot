using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
// TODO: Test Class, can be removed!
/// </summary>
public class Accelerometer : MonoBehaviour {
	
	public float Threshold = 0.7f;
	public Text message;

	// Use this for initialization
	void Start(){
		// Activate the gyroscope
		Input.gyro.enabled = true;
	}

	// Update is called once per frame
	void Update () {
		//transform.Translate (0,0,-Input.acceleration.z*Time.deltaTime*Speed);
		//transform.Rotate (0, 0, -Input.acceleration.x*Speed);

		//Debug.Log ("X: "+Input.acceleration.x);
		//Debug.Log ("Z: "+Input.acceleration.z);
		if (Input.gyro.userAcceleration.magnitude > 0.1) {
			Debug.Log ("UserAcceleration: " + Input.gyro.userAcceleration);
			if (Input.gyro.userAcceleration.z > Threshold) {
				message.text = "Acceleration is " + Input.gyro.userAcceleration.z;
			}
		}
	}

	/*public float smooth = 0.4f;
	public float newRotation;
	public float sensitivity = 6;
	private Vector3 currentAcceleration, initialAcceleration;
	void Start()
	{
		initialAcceleration = Input.acceleration;
		currentAcceleration = Vector3.zero;
	}
	void Update () {
		//pre-processing
		currentAcceleration = Vector3.Lerp(currentAcceleration, Input.acceleration - initialAcceleration, Time.deltaTime/smooth);
		newRotation = Mathf.Clamp(currentAcceleration.x * sensitivity, -1, 1);
		transform.Rotate(0, 0, -newRotation);
	}*/

}
