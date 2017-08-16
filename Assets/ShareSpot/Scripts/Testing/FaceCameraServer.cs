using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FaceCameraServer : MonoBehaviour {

	public GameObject PlayerName;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<TextMesh> ().text = PlayerName.GetComponent<TextMesh> ().text;

		this.transform.LookAt (Camera.main.transform.position);
		// Rotate for 180° otherwise it is displayed upside down
		this.transform.Rotate (new Vector3 (0, 180, 0));
	}
}
