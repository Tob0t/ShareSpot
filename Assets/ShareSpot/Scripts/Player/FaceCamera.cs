using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
// Rotate and face the camera everytime.
/// </summary>
public class FaceCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.current != null) {
			this.transform.LookAt (Camera.main.transform.position);
			// Rotate for 180° otherwise it is displayed upside down
			this.transform.Rotate (new Vector3 (0, 180, 0));
		}
	}
}
