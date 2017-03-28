using UnityEngine;
using System.Collections;

/// <summary>
/// Class for Computer Input 
/// </summary>
public class InputComputer: IInputMode{

	/// <summary>
	/// Returns Key for Fire.
	/// </summary>
	public bool Fire() {
		return Input.GetKeyDown(KeyCode.Space);
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

