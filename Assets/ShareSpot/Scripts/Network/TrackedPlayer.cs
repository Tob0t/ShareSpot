using UnityEngine;
using System.Collections;

/// <summary>
// TrackedPlayer is inherited from ATrackingEntity which is receiving the position from the PlayerManager
/// </summary>
public class TrackedPlayer : ATrackingEntity {

	#region [Public fields]
	public float Height = 185;

	#endregion

	/// <summary>
	/// Overriding SetPosition Function
	/// Transforming input x/y values to x/y/z values where y is defined as a constant height
	/// </summary>
	public override void SetPosition(Vector2 coords)
	{
		transform.position = new Vector3 (coords.x, Height, coords.y);
	}
}
