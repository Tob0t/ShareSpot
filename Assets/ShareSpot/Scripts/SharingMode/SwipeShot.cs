using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Class for controlling SwipeShot behaviour.
/// TODO: Adding FileToShare?
/// </summary>
public class SwipeShot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	#region [Public fields]
	public float ForceFactor = 1f;	///< Force factor for the shot file.
	public GameObject UserInterface;	///< Reference to the user interface of the player.

	public Color NormalStartColor = Color.white;	///< Color of the normal state of the panel.
	public Color BeginDragColor = Color.yellow;	///< Color of the begin drag state of the panel.
	public Color EndDragColor = Color.green;	///< Color of the end drag state of the panel.

	// For graphical representation of the swipe
	public RectTransform SwipeLine;	///< The swipe line itself.
	public float SwipeLineWidth = 25f;	///< The width of the swipe line.

	#endregion

	#region [Private fields]
	private float _startTime;	///< Time when the drag is started.
	private Vector2 _startPos;	///< Position when the drag is started.

	#endregion

	/// <summary>
	/// Called when user starts to drag something.
	/// </summary>
	/// <param name="eventData">Event data of the current pointer.</param>
	public void OnBeginDrag(PointerEventData eventData){
		GetComponent<Image> ().color = BeginDragColor;
		_startTime = Time.time;
		_startPos = eventData.position;
		//Debug.Log ("startPos: " + _startPos.ToString());
		SwipeLine.gameObject.SetActive(true);
		SwipeLine.position = _startPos;
	}
		
	/// <summary>
	/// Called during dragging.
	/// </summary>
	/// <param name="eventData">Event data of the current pointer.</param>
	public void OnDrag(PointerEventData eventData){

		// calculate and draw a line for the swiped distance
		Vector3 differenceVector = eventData.position - _startPos;
		SwipeLine.sizeDelta = new Vector2( differenceVector.magnitude, SwipeLineWidth);
		SwipeLine.pivot = new Vector2(0, 0.5f);
		float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
		SwipeLine.rotation = Quaternion.Euler(0,0, angle);

	}
		
	/// <summary>
	/// Called when user is finished with dragging.
	/// </summary>
	/// <param name="eventData">Event data of the current pointer.</param>
	public void OnEndDrag(PointerEventData eventData){
		// Get position and calculate vector between start and end position
		Vector2 endPos = eventData.position;
		Vector2 diff = endPos - _startPos;

		// Change color of the panel to endDragColor
		GetComponent<Image> ().color = EndDragColor;

		// Create force determined from the difference vector
		Vector3 force = new Vector3 (diff.x, 0, diff.magnitude);
		//Debug.Log ("force: "+force.ToString());	

		// Cut the force depending on the time needed for the drag
		force /= (Time.time - _startTime);

		// Start the SwipeShot on the user interface
		UserInterface.GetComponent<UserInterfaceController>().SwipeShot(force * ForceFactor);

		// Start coroutine to wait for some seconds until changing back the color of the panel
		StartCoroutine ("WaitSeconds");

	}
		
	/// <summary>
	/// Coroutine to wait for 1 second1.
	/// </summary>
	/// <returns>The seconds to wait for.</returns>
	IEnumerator WaitSeconds(){
		yield return new WaitForSeconds(1f);
		GetComponent<Image> ().color = NormalStartColor;
		SwipeLine.gameObject.SetActive(false);
	}
}

