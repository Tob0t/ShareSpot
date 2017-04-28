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
        var hit = collision.gameObject;
		if (hit.CompareTag ("Player")) {
			hit.GetComponent<PlayerController> ().ReceiveFile (this.gameObject);
			gameObject.SetActive (false);
		} else {
			Destroy (gameObject);
		}
    }
}
