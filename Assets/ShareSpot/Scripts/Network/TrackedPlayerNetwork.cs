using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
// TrackedPlayerNetwork is using some network-related commands
/// </summary>
public class TrackedPlayerNetwork : NetworkBehaviour {

	#region [Public fields]
	[SyncVar]
	public bool HasPlayer; ///< Indicates if the TrackedPlayer is already connected to a device
	public GameObject ControlledPlayer; ///< Gameobject which is controlling the player

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
		if (NetworkServer.active) {
			if (MyNetworkManager.Instance.IsServer) {
				NetworkServer.Spawn (gameObject);
			}
		}
		if (HasPlayer) {
			gameObject.SetActive (false);
		} else {
			gameObject.SetActive (true);
		}
	}

	// Called when the GameObject is selected
	void OnMouseDown(){
		if (isServer) {
			Admin.Instance.CurrentTrackedPlayer = gameObject;
		}
		StartCoroutine ("SelectPlayer");
	}

	// Coroutine to change the color for 4 seconds
	IEnumerator SelectPlayer(){
		Color current = _body.GetComponent<MeshRenderer> ().material.color;
		_body.GetComponent<MeshRenderer> ().material.color = Color.red;
		yield return new WaitForSeconds(4f);
		_body.GetComponent<MeshRenderer> ().material.color = current;
	}


}
