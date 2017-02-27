using UnityEngine;
using System.Collections;

/// <summary>
// Class for Mobile Input 
/// </summary>
public class InputMobile: IInputMode{

	// Returns Key for Fire
	public bool Fire() {
		return Input.GetMouseButtonDown (0);
	}

	// Returns Keys for Turning
	public float Turn ()
	{
		return Input.GetAxis ("Horizontal");
	}

	// Returns Keys for Moving
	public float Move ()
	{
		return Input.GetAxis ("Vertical");
	}
}

