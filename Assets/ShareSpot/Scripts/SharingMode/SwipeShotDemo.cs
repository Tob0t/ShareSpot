using UnityEngine;
using System.Collections;

public class SwipeShotDemo : MonoBehaviour {

	public GameObject File;

	public void SwipeShot(Vector3 force){
		// Create the SharedFile from the file Prefab
		File.SetActive(true);

		//sharedFile.GetComponent<Rigidbody>().velocity = sharedFile.transform.forward * ShootingSpeed;
		Debug.Log("Force Vector: "+force.ToString());

		// Transform the force in a direction first and set is as velocity
		File.GetComponent<Rigidbody> ().velocity = transform.TransformDirection (force*0.03f);
		//sharedFile.GetComponent<Rigidbody>().AddRelativeForce(force * ShootingSpeed);

		// Destroy the file after 30 seconds
		//Destroy(sharedFile, 30.0f);
		StartCoroutine("ReturnFile");
	}

	IEnumerator ReturnFile(){
		yield return new WaitForSeconds(5f);
		File.SetActive (false);
		File.GetComponent<Rigidbody> ().velocity = new Vector3(0,0,0);
		File.transform.localPosition = new Vector3(0,0,15);

	}
}
