using UnityEngine;
using System.Collections;

/// <summary>
// Returns the clicked object for debugging reasons
/// </summary>
public class RaycastDebug : MonoBehaviour {

	void Update () {
		
		if (Input.GetMouseButtonDown (0)) {
			//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);
			Debug.Log (Camera.allCameras[1].gameObject);
			Ray ray = Camera.allCameras[1].ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				Debug.Log ("Name = " + hit.collider.name);
				Debug.Log ("Tag = " + hit.collider.tag);
				Debug.Log ("Hit Point = " + hit.point);
				Debug.Log ("Object position = " + hit.collider.gameObject.transform.position);
				Debug.Log ("--------------");
			}
		}
	}
}