using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
// This Class is detecting Swipes based on the touch phases
/// </summary>
public class SwipeDetector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public float MinSwipeDistY = 200; ///< minimal movement distance in Y direction to detect a Swipe
	public float MinSwipeDistX = 200; ///< minimal movement distance in X direction to detect a Swipe
	private Vector2 startPos; ///< Starting position

	private Vector2 startPosDrag; ///< Starting position

	void Update(){
		//#if UNITY_ANDROID
		if (Input.touchCount > 0) {
			Touch touch = Input.touches[0];
			switch (touch.phase) {
				case TouchPhase.Began:{
					startPos = touch.position;
					break;
				} 
				case TouchPhase.Ended:{
					float swipeDistVertical = (new Vector3 (0, touch.position.y, 0) - new Vector3 (0, startPos.y, 0)).magnitude;
					if (swipeDistVertical > MinSwipeDistY) {
						//GetComponent<UserInterfaceController> ().ShootFile (touch.position-startPos);
						float swipeValue = Mathf.Sign (touch.position.y - startPos.y);
						if (swipeValue > 0) {	// up swipe
							Debug.Log("Up swipe "+ swipeDistVertical);
							//GetComponent<UserInterfaceController> ().ShootFile (startPos - touch.position);
						} else if (swipeValue < 0) {	//down swipe
							Debug.Log("Down swipe "+ swipeDistVertical);
						}
					}
					float swipeDistHorizontal = (new Vector3 (touch.position.x, 0, 0) - new Vector3 (startPos.x, 0, 0)).magnitude;
					if (swipeDistHorizontal > MinSwipeDistX) {
						float swipeValue = Mathf.Sign (touch.position.x - startPos.x);
						if (swipeValue > 0) {	//right swipe
							Debug.Log("Righ swipe " +swipeDistHorizontal);
						} else if (swipeValue < 0) {	//left swipe
							Debug.Log("Left swipe "+swipeDistHorizontal);
						}
					}
					break;
				}
			}
		}
	}

	public void OnBeginDrag(PointerEventData eventData){
		Debug.Log ("OnBeginDrag");
		startPosDrag = eventData.position;
	}

	public void OnDrag(PointerEventData eventData){
		//Debug.Log ("OnDrag " +eventData.position);
	}

	public void OnEndDrag(PointerEventData eventData){
		Debug.Log ("OnEndDrag");
		Debug.Log ("StartPos: "+startPosDrag.ToString());
		Debug.Log ("StartPos: "+eventData.position.ToString());
		#if UNITY_EDITOR_WIN
			//GetComponent<UserInterfaceController> ().ShootFile (startPosDrag - eventData.position);
		#endif
	}
}
