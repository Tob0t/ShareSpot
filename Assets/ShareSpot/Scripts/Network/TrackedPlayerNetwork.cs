using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
// TrackedPlayerNetwork is using some network-related commands
/// </summary>
public class TrackedPlayerNetwork : NetworkBehaviour {

	#region [Public fields]
	[SyncVar]
	public bool HasPlayer;
	public GameObject ControlledPlayer;

	#endregion

	#region [Private fields]
	private Transform _body;

	#endregion

	// Use this for initialization
	void Start () {
		_body = transform.FindChild ("Body");
	}
	
	// Update is called once per frame
	void Update () {
		if (MyNetworkManager.Instance.IsServer) {
			NetworkServer.Spawn (gameObject);
		}
		if (HasPlayer) {
			gameObject.SetActive (false);
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
