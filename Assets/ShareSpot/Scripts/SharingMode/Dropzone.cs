using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Dropzone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	//public Draggable.Slot typeOfItem = Draggable.Slot.INVENTORY;

	public void OnPointerEnter(PointerEventData eventData){
		//Debug.Log ("OnPointerEnter to " + gameObject.name);
		if(eventData.pointerDrag == null)
			return;

		Draggable d = eventData.pointerDrag.GetComponent<Draggable> ();
		if (d != null) {
			//if (typeOfItem == d.typeOfItem || typeOfItem == Draggable.Slot.INVENTORY) {
			d.placeholderParent = this.transform;
			//}
		}
	}

	public void OnPointerExit(PointerEventData eventData){
		//Debug.Log ("OnPointerExit to " + gameObject.name);

		if(eventData.pointerDrag == null)
			return;

		Draggable d = eventData.pointerDrag.GetComponent<Draggable> ();
		if (d != null && d.placeholderParent == this.transform) {
			//if (typeOfItem == d.typeOfItem || typeOfItem == Draggable.Slot.INVENTORY) {
			d.placeholderParent = d.parentToReturnTo;
			//}
		}
	}

	public void OnDrop(PointerEventData eventData){
		Debug.Log (eventData.pointerDrag + "was dropped on " + gameObject.name);

		Draggable d = eventData.pointerDrag.GetComponent<Draggable> ();
		if (d != null) {
			//if (typeOfItem == d.typeOfItem || typeOfItem == Draggable.Slot.INVENTORY) {
				d.parentToReturnTo = this.transform;
			//}
		}
	}
}
