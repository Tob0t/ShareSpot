using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// TrackedPlayerNetwork is using some network-related commands.
/// </summary>
public class TrackedPlayerNetwork : NetworkBehaviour {

	#region [Public fields]
	[SyncVar]
	public bool HasPlayer; ///< Indicates if the TrackedPlayer is already connected to a device.
	public GameObject ControlledPlayer; ///< Gameobject which is controlling the player.

	#endregion

	#region [Private fields]
	private Transform _body; ///< Body of the Gameobject

	#endregion

	// Use this for initialization
	void Start () {
		_body = transform.FindChild ("Body");
	}
	
	// Update is called once per frame
	void Update () {
		// If the network is active and the server, spawn the tracked player.
		if (NetworkServer.active) {
			if (MyNetworkManager.Instance.IsServer) {
				NetworkServer.Spawn (gameObject);
			}
		}

		// If a device is already connected to it disable it 
		if (HasPlayer) {
			//gameObject.SetActive (false);
			gameObject.GetComponentInChildren<MeshRenderer> ().enabled = false;
		} else { // otherwise enable
			//gameObject.SetActive (true);
			gameObject.GetComponentInChildren<MeshRenderer> ().enabled = true;
		}
	}

	/// <summary>
	/// Called when the GameObject is selected.
	/// </summary>
	void OnMouseDown(){
		if (isServer) {
			Admin.Instance.CurrentTrackedPlayer = gameObject;
		}
		StartCoroutine ("SelectPlayer");
	}

	/// <summary>
	///	Coroutine to change the color for 4 seconds.
	/// </summary>
	/// <returns>The seconds to wait for.</returns>
	IEnumerator SelectPlayer(){
		Color current = _body.GetComponent<MeshRenderer> ().material.color;
		_body.GetComponent<MeshRenderer> ().material.color = Color.red;
		yield return new WaitForSeconds(4f);
		_body.GetComponent<MeshRenderer> ().material.color = current;
	}


}
