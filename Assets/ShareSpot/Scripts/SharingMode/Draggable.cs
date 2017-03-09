using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
// Class for Controlling DragNDrop Behaviour
/// </summary>

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public Transform parentToReturnTo = null;	///< Parent of the currently clicked object
	public Transform placeholderParent = null;	///< Placedholder for parent object

	public GameObject placeholder = null;	///< Placeholder object which acts as "available space" between the files
	public GameObject hitObject = null;	///< GameObject which is hit during onDrag

	public Color markAvailable = Color.yellow;	///< Color to show all available dropable objects when start dragging
	public Color markHover = Color.green;	///< Color to show when an object is hovered
	public Color markDeselected = Color.black;	///< Color to show when dragging is finished

	public GameObject UserInterface;


	//public enum Slot {WEAPON, HEAD, CHEST, LEGS, FEET, INVENTORY};
	//public Slot typeOfItem = Slot.WEAPON;

	// Called when user starts to drag something
	public void OnBeginDrag(PointerEventData eventData){
		Debug.Log ("OnBeginDrag");

		// Create placeholder object and set the same parent as the dragged object
		placeholder = new GameObject ();
		placeholder.transform.SetParent (this.transform.parent);

		// Set the same layout measurements
		LayoutElement le = placeholder.AddComponent<LayoutElement> ();
		le.preferredWidth = this.GetComponent<LayoutElement> ().preferredWidth;
		le.preferredHeight = this.GetComponent<LayoutElement> ().preferredHeight;

		// Set the placeholder to the same position
		placeholder.transform.SetSiblingIndex (this.transform.GetSiblingIndex());

		// Set the return parent to the current one
		parentToReturnTo = this.transform.parent;

		// Set also the placeholderParent to the current parent
		placeholderParent = parentToReturnTo;

		// Set parent of the current object one level up
		this.transform.SetParent (this.transform.parent.parent);

		// Allow that the file can act as trigger (=raycast)
		GetComponent<CanvasGroup> ().blocksRaycasts = false;

		// Highlight availabe Dropzones
		//Dropzone[] zones = GameObject.FindObjectsOfType<Dropzone> ();

		// Mark all droppable Objects green
		foreach (GameObject dropable in GameObject.FindGameObjectsWithTag("Player")) {
			// TODO: save original color
			//Color current = droppable.GetComponent<MeshRenderer> ().material.color;
			dropable.GetComponent<MeshRenderer> ().material.color = markAvailable;
		}
	}

	// Called during dragging
	public void OnDrag(PointerEventData eventData){
		//Debug.Log ("OnDrag " +eventData.position);

		// Set the position of the dragged object to position of the cursors
		this.transform.position = eventData.position;

		//  Adjust the placeholder position depending on their x position
		for (int i = 0; i < placeholderParent.childCount; i++) {
			if(this.transform.position.x < placeholderParent.GetChild(i).position.x){
				placeholder.transform.SetSiblingIndex (i);
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
					hitObject = hit.collider.gameObject;
					MarkObject(hitObject);
				} else{
				// ClearSelection
				if (hitObject != null) {
					ClearSelection ();
				}
			}
		}
	}

	// Mark Object when the Drag is hovering it
	void MarkObject(GameObject obj) {
		if(obj != null) {
			obj.GetComponent<MeshRenderer> ().material.color = markHover;
		}
	}

	// Reset Object when Drag is not hovering anymore
	void ClearSelection() {
		hitObject.GetComponent<MeshRenderer> ().material.color = markAvailable;
		hitObject = null;
	}

	// Called when user is finished with dragging
	public void OnEndDrag(PointerEventData eventData){
		Debug.Log ("OnEndDrag");

		// TODO Think about it
		// set parent of the object to the variable parentToReturnTo
		this.transform.SetParent (parentToReturnTo);

		// Set the position to the one from the placeholder
		this.transform.SetSiblingIndex (placeholder.transform.GetSiblingIndex ());

		// Block raycasts of the object again
		GetComponent<CanvasGroup> ().blocksRaycasts = true;

		// Whats is currently under me, loop through it
		//EventSystem.current.RaycastAll (eventData);

		// Remove the placeholder object
		Destroy (placeholder);

		// Reset all droppable Objects back
		foreach (GameObject droppable in GameObject.FindGameObjectsWithTag("Player")) {
			//Color current = droppable.GetComponent<MeshRenderer> ().material.color;
			droppable.GetComponent<MeshRenderer> ().material.color = markDeselected;
		}

		// If the object is dropped on a valid hitObject
		if (hitObject != null) {
			// TODO: Work with hitObject
			Debug.Log ("HitObject " + hitObject);
			//hitObject.GetComponent<PlayerController> ().ReceiveFile (this.gameObject);
			//hitObject.GetComponent<PlayerController> ().CmdReceiveFile(this.gameObject);
			// Call Command on the local Player
			UserInterface.GetComponent<UserInterfaceController> ().PlayerObject.GetComponent <PlayerController> ().CmdReceiveFile (this.gameObject, hitObject);
			gameObject.SetActive (false);
		}
	}
	
}
