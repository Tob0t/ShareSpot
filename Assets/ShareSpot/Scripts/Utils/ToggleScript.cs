using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This class is extending the toggle button to improve the behaviour.
/// </summary>
public class ToggleScript : Toggle{

	public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData) {
		base.OnPointerClick(eventData);
		if (isOn) {
			// Disable all other toggle fields programmatically
			foreach(Image img in this.transform.parent.gameObject.GetComponentsInChildren<Image> ()){
				// exclude the parent object (unity bug)
				if (img.gameObject.GetInstanceID () != this.transform.parent.gameObject.GetInstanceID ()) {
					img.color = Color.clear;
				}
			}
			image.color = this.colors.pressedColor;
		} else {
			image.color = Color.clear;
		}
	}
}
