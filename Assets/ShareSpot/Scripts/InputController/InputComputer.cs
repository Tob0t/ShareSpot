using UnityEngine;
using System.Collections;

/// <summary>
// Class for Computer Input 
/// </summary>
public class InputComputer: IInputMode{

	// Returns Key for Fire
	public bool Fire() {
		return Input.GetKeyDown(KeyCode.Space);
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

