using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
// Rotata and face the camera everytime
/// </summary>
public class FaceCamera : NetworkBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Camera.current != null) {
			this.transform.LookAt (Camera.current.transform.position);
			//this.transform.LookAt (GameObject.FindGameObjectWithTag("ARCamera").transform.position);
			this.transform.Rotate (new Vector3 (0, 180, 0));
		}
	}
}
