using UnityEngine;
using System.Collections;
/// <summary>
// Test Class, can be removed!
/// </summary>
public class Accelerometer : MonoBehaviour {
	public int Speed = 20;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (Speed*Input.acceleration.x, 0, Speed*(-Input.acceleration.z));
		Debug.Log ("X: "+Input.acceleration.x);
		Debug.Log ("Z: "+Input.acceleration.z);
	}
}
