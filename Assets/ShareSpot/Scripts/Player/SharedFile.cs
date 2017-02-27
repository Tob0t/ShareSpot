using UnityEngine;
using System.Collections;

/// <summary>
// Class for a shared file which can be "shot" to other players
/// </summary>
public class SharedFile : MonoBehaviour {

	// As soon as the sharedFile determines a collision it calls a function on the PlayerObject and destroys the object
    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
		if(hit.CompareTag("Player")){
			hit.GetComponent<PlayerController> ().ReceiveFile ();
		}

        Destroy(gameObject);
    }
}
