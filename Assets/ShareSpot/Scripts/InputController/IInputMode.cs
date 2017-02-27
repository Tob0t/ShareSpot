using System;

/// <summary>
// Interface to support multiple devices
/// </summary>

public interface IInputMode{
	bool Fire();
	float Turn();
	float Move();
}


