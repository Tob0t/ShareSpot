using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class MarkerFound : MonoBehaviour, ITrackableEventHandler {

	#region PRIVATE_MEMBER_VARIABLES

	private TrackableBehaviour mTrackableBehaviour;
	private Quaternion mCurrentRotation;

	#endregion // PRIVATE_MEMBER_VARIABLES

	// Use this for initialization
	void Start()
	{
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//mCurrentRotation = Camera.main.transform.rotation;
	}

	public void OnTrackableStateChanged(
		Vuforia.TrackableBehaviour.Status previousStatus,
		Vuforia.TrackableBehaviour.Status newStatus){

		Handheld.Vibrate ();

		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			OnTrackingFound();
		}
		else
		{
			OnTrackingLost();
		}
	}

	private void OnTrackingFound(){
		//Camera.main.transform.rotation = mTrackableBehaviour.transform.rotation;
		//Input.gyro.enabled = false;
		//Camera.main.GetComponent<GyroController> ().AttachGyro ();

	}

	private void OnTrackingLost(){
		//Input.gyro.enabled = true;
		//Camera.main.transform.rotation = mCurrentRotation;
		Camera.main.GetComponent<GyroController> ().AttachGyro ();
	}
}
