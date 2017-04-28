using UnityEngine;
using System.Collections;

/// <summary>
/// TrackedPlayer is inherited from ATrackingEntity which is receiving the position from the PlayerManager.
/// </summary>
public class TrackedPlayer : ATrackingEntity {

	#region [Public fields]
	//public float PlayerHeight = 185*3;	///< Individually setting the height of the player
	public float PlayerHeight = 270f;	///< Individually setting the height of the player

	#endregion

	/// <summary>
	/// Overriding SetPosition Function:
	/// Transforming input x/y values to x/y/z values where y is defined as a constant height.
	/// <param name="coords">The 2D-vector of the coordinates.</param>
	/// </summary>
	public override void SetPosition(Vector2 coords)
	{
		transform.position = new Vector3 (coords.x, PlayerHeight, coords.y);
	}
}
