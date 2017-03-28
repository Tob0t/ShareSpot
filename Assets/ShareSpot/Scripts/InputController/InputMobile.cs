using UnityEngine;
using System.Collections;

/// <summary>
// Class for Mobile Input 
/// </summary>
public class InputMobile: IInputMode{

	/// <summary>
	/// Returns Key for Fire.
	/// </summary>
	public bool Fire() {
		return Input.GetMouseButtonDown (0);
	}

	/// <summary>
	/// Returns Keys for Turning.
	/// </summary>
	public float Turn ()
	{
		return Input.GetAxis ("Horizontal");
	}

	/// <summary>
	/// Returns Keys for Moving.
	/// </summary>
	public float Move ()
	{
		return Input.GetAxis ("Vertical");
	}
}

