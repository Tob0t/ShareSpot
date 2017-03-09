using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class SwipeShot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public float ForceFactor = 0.1f;
	public GameObject UserInterface;

	public Color NormalStartColor = Color.white;
	public Color BeginDragColor = Color.yellow;
	public Color EndDragColor = Color.green;

	private float startTime;
	private Vector2 startPos;

	// Called when user starts to drag something
	public void OnBeginDrag(PointerEventData eventData){
		GetComponent<Image> ().color = BeginDragColor;
		startTime = Time.time;
		startPos = eventData.position;
		Debug.Log ("startPos: " + startPos.ToString());
	}

	// Called during dragging
	public void OnDrag(PointerEventData eventData){
	}

	// Called when user is finished with dragging
	public void OnEndDrag(PointerEventData eventData){
		GetComponent<Image> ().color = EndDragColor;
		Vector2 endPos = eventData.position;
		Vector2 diff = endPos - startPos;
		Debug.Log ("diff: "+diff.ToString());
		Vector3 force = new Vector3 (diff.x, 0, diff.magnitude);
		Debug.Log ("force: "+force.ToString());	
		force /= (Time.time - startTime);
		UserInterface.GetComponent<UserInterfaceController>().SwipeShot(force * ForceFactor);
		StartCoroutine ("WaitSeconds");
	}

	// Coroutine to wait for 4 seconds
	IEnumerator WaitSeconds(){
		yield return new WaitForSeconds(4f);
		GetComponent<Image> ().color = NormalStartColor;
	}

	/*
	void OnMouseDown() {
		startTime = Time.time;
		startPos = Input.mousePosition;
		Debug.Log ("startPos: " + startPos.ToString());
	}

	void OnMouseUp() {
		Vector2 endPos = Input.mousePosition;
		Vector2 diff = endPos - startPos;
		Debug.Log ("diff: "+diff.ToString());
		Vector3 force = new Vector3 (diff.x, 0, diff.magnitude);
		Debug.Log ("force: "+force.ToString());	
		force /= (Time.time - startTime);
		//GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotation;
		//GetComponent<Rigidbody> ().AddRelativeForce (force * ForceFactor);
		GetComponentInParent<PlayerController> ().CmdShootFile (force * ForceFactor);
		ReturnBall();
	}
	*/


}

