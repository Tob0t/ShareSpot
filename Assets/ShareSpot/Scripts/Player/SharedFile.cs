﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
// Class for a shared file which can be "shot" to other players
/// </summary>
public class SharedFile : NetworkBehaviour {

	[SyncVar]
	public int SourceId;
	public string Name = "ExampleFile";
	public string Author = "Max Mustermann";
	public int size = 100;

	// As soon as the sharedFile determines a collision it calls a function on the PlayerObject and destroys the object
    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
		if (hit.CompareTag ("Player")) {
			hit.GetComponent<PlayerController> ().ReceiveFile (this.gameObject);
			gameObject.SetActive (false);
		} else {
			Destroy (gameObject);
		}
    }
}
