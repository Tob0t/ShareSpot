using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class ARController : MonoBehaviour {
	private TrackerManager trackerManager;
	private RotationalDeviceTracker deviceTracker;

	// Use this for initialization
	void Start () {
		trackerManager = TrackerManager.Instance;
		deviceTracker = trackerManager.InitTracker<RotationalDeviceTracker> ();
		deviceTracker.SetModelCorrectionMode (RotationalDeviceTracker.MODEL_CORRECTION_MODE.HANDHELD);
		deviceTracker.Start ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateCameraBaseRotation(){
		deviceTracker.Stop ();
		deviceTracker.RecenterPose ();
		deviceTracker.Start ();
	}
}
