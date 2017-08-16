using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
// Class for a shared file which can be "shot" to other players
/// </summary>
public class SharedFile : NetworkBehaviour {

	[SyncVar]
	public int SourceId;	///< ID of the sending player of the file.
	public string Name = "ExampleFile";	///< Example name of the file.
	public string Author = "Max Mustermann";	///< Example author of the file.
	public int Size = 100;	///< Example size of the file.

	/// <summary>
	/// As soon as the sharedFile determines a collision it calls a function on the PlayerObject and destroys the object.
	/// </summary>
	/// <param name="collision">Collision componente of the other game object.</param>
	void OnCollisionEnter(Collision collision)
    {
		//Debug.Log ("Collision detected with "+collision.gameObject.ToString());
        var hit = collision.gameObject;
		if (hit.CompareTag ("Player")) {
			// Only work if the file is not hit with the sender
			if (hit.GetComponent<PlayerController> ().ConnectionId != SourceId) {
				hit.GetComponent<PlayerController> ().ReceiveFile (this.gameObject);
				gameObject.SetActive (false);
			}
		} else {
			// Increasing error rate if the hit is a miss (on server).
			if (isServer) {
				Admin.Instance.GameManger.GetComponent<GameController> ().IncreaseErrorRate (SourceId);
			}
			// Destroy the game object
			Destroy (gameObject);
		}
    }
}
