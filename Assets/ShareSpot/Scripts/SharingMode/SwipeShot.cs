using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Class for controlling SwipeShot behaviour.
/// </summary>
public class SwipeShot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	#region [Public fields]
	public float ForceFactor = 1f;	///< Force factor for the shot file
	public GameObject UserInterface;	///< Reference to the user interface of the player

	public Color NormalStartColor = Color.white;	///< Color of the normal state of the panel
	public Color BeginDragColor = Color.yellow;	///< Color of the begin drag state of the panel
	public Color EndDragColor = Color.green;	///< Color of the end drag state of the panel

	#endregion

	#region [Private fields]
	private float _startTime;	///< Time when the drag is started
	private Vector2 _startPos;	///< Position when the drag is started

	#endregion

	/// <summary>
	/// Called when user starts to drag something.
	/// </summary>
	/// <param name="eventData">Event data of the current pointer.</param>
	public void OnBeginDrag(PointerEventData eventData){
		GetComponent<Image> ().color = BeginDragColor;
		_startTime = Time.time;
		_startPos = eventData.position;
		Debug.Log ("startPos: " + _startPos.ToString());
	}
		
	/// <summary>
	/// Called during dragging.
	/// </summary>
	/// <param name="eventData">Event data of the current pointer.</param>
	public void OnDrag(PointerEventData eventData){
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

		// TODO: Show confirmation log when hiting an object.
	}
		
	/// <summary>
	/// Coroutine to wait for 4 seconds.
	/// </summary>
	/// <returns>The seconds to wait for.</returns>
	IEnumerator WaitSeconds(){
		yield return new WaitForSeconds(4f);
		GetComponent<Image> ().color = NormalStartColor;
	}
}

