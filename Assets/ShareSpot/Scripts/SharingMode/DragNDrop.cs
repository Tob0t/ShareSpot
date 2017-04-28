using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
// Class for controlling DragNDrop Behaviour
/// </summary>

public class DragNDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	#region [Public fields]
	public Color MarkAvailable = Color.yellow;	///< Color to show all available dropable objects when start dragging
	public Color MarkHover = Color.green;	///< Color to show when an object is hovered
	public Color MarkDeselected = Color.black;	///< Color to show when dragging is finished
	public GameObject FileToShare; ///< Reference to the file to share
	public GameObject UserInterface; ///< Reference to user interface

	#endregion

	#region [Private fields]
	private Transform _parentToReturnTo = null;	///< Parent of the currently clicked object
	private Transform _placeholderParent = null;	///< Placedholder for parent object
	private GameObject _placeholder = null;	///< Placeholder object which acts as "available space" between the files
	private GameObject _hitObject = null;	///< GameObject which is hit during onDrag

	#endregion


	// Called when user starts to drag something
	public void OnBeginDrag(PointerEventData eventData){
		Debug.Log ("OnBeginDrag");

		// Create placeholder object and set the same parent as the dragged object
		_placeholder = new GameObject ();
		_placeholder.transform.SetParent (this.transform.parent);

		// Set the same layout measurements
		LayoutElement le = _placeholder.AddComponent<LayoutElement> ();
		le.preferredWidth = this.GetComponent<LayoutElement> ().preferredWidth;
		le.preferredHeight = this.GetComponent<LayoutElement> ().preferredHeight;

		// Set the placeholder to the same position
		_placeholder.transform.SetSiblingIndex (this.transform.GetSiblingIndex());

		// Set the return parent to the current one
		_parentToReturnTo = this.transform.parent;

		// Set also the placeholderParent to the current parent
		_placeholderParent = _parentToReturnTo;

		// Set parent of the current object one level up
		this.transform.SetParent (this.transform.parent.parent);

		// Allow that the file can act as trigger (=raycast)
		GetComponent<CanvasGroup> ().blocksRaycasts = false;

		// Highlight availabe Dropzones
		//Dropzone[] zones = GameObject.FindObjectsOfType<Dropzone> ();

		// Mark all droppable Objects
		foreach (GameObject dropable in GameObject.FindGameObjectsWithTag("Player")) {
			// TODO: save original color
			//Color current = droppable.GetComponent<MeshRenderer> ().material.color;
			dropable.GetComponent<MeshRenderer> ().material.color = MarkAvailable;
		}
	}

	// Called during dragging
	public void OnDrag(PointerEventData eventData){
		//Debug.Log ("OnDrag " +eventData.position);

		// Set the position of the dragged object to position of the cursors
		this.transform.position = eventData.position;

		//  Adjust the placeholder position depending on their x position
		for (int i = 0; i < _placeholderParent.childCount; i++) {
			if(this.transform.position.x < _placeholderParent.GetChild(i).position.x){
				_placeholder.transform.SetSiblingIndex (i);
				break; ///< break if it is set once per drag
			}
		}

		// Raycast of the current marked object
		//Ray ray = GameObject.FindGameObjectWithTag ("ARCameraStream").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		// if the cursor is over a droppable mark the object
		if (Physics.Raycast(ray, out hit)) {
				if (hit.collider.gameObject.CompareTag ("Player")) {
					// MarkObject
					_hitObject = hit.collider.gameObject;
					MarkObject(_hitObject);
				} else{
				// ClearSelection
				if (_hitObject != null) {
					ClearSelection ();
				}
			}
		}
	}

	// Mark Object when the Drag is hovering it
	void MarkObject(GameObject obj) {
		if(obj != null) {
			obj.GetComponent<MeshRenderer> ().material.color = MarkHover;
		}
	}

	// Reset Object when Drag is not hovering anymore
	void ClearSelection() {
		_hitObject.GetComponent<MeshRenderer> ().material.color = MarkAvailable;
		_hitObject = null;
	}

	// Called when user is finished with dragging
	public void OnEndDrag(PointerEventData eventData){
		Debug.Log ("OnEndDrag");

		// TODO Think about it
		// set parent of the object to the variable parentToReturnTo
		this.transform.SetParent (_parentToReturnTo);

		// Set the position to the one from the placeholder
		this.transform.SetSiblingIndex (_placeholder.transform.GetSiblingIndex ());

		// Block raycasts of the object again
		GetComponent<CanvasGroup> ().blocksRaycasts = true;

		// Whats is currently under me, loop through it
		//EventSystem.current.RaycastAll (eventData);

		// Remove the placeholder object
		Destroy (_placeholder);

		// Reset all droppable Objects back
		foreach (GameObject droppable in GameObject.FindGameObjectsWithTag("Player")) {
			//Color current = droppable.GetComponent<MeshRenderer> ().material.color;
			droppable.GetComponent<MeshRenderer> ().material.color = MarkDeselected;
		}

		// If the object is dropped on a valid hitObject
		if (_hitObject != null) {
			Debug.Log ("HitObject " + _hitObject);
			// TODO: GameObject must have a network identity to be able to work with it
			// Call Command on the local Player
			PlayerController p = UserInterface.GetComponent<UserInterfaceController> ().PlayerObject.GetComponent <PlayerController> ();
			p.CmdReceiveFile (this.gameObject, p.ConnectionId ,_hitObject.GetComponent<PlayerController> ().ConnectionId);
			// remove gameobject
			//gameObject.SetActive (false);
		}
	}
	
}
