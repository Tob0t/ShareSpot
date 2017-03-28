using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
/// <summary>
/// Helper Class to make a smooth sync by interpolations.
/// </summary>
public class NetworkSyncTransform : NetworkBehaviour
{
	// TODO: Adjust values for minimum threshold and so on
	[SerializeField]
	private float _posLerpRate = 15;	///< The rate for interpolating over the position.
	[SerializeField]
	private float _rotLerpRate = 15;	///< The rate for interpolating over the rotation.
	[SerializeField]
	private float _posThreshold = 0.1f;	///< The minimum threshold for a position change detection.
	[SerializeField]
	private float _rotThreshold = 1f;	///< The minimum threshold for a rotation change detection.

	[SyncVar]
	private Vector3 _lastPosition;	///< Saving the last position of the player.

	[SyncVar]
	private Vector3 _lastRotation;	///< Saving the last rotation of the player.

	void Update()
	{
		// Only interpolate if it is not the local player
		// TODO: Maybe interpolate position also for local player?!?
		if (isLocalPlayer)
			return;

		InterpolatePosition();
		InterpolateRotation();
	}

	void FixedUpdate()
	{
		// only send position and rotation update if it is not the local player
		if (!isLocalPlayer)
			return;

		var posChanged = IsPositionChanged();

		// send position update if it is changed
		if (posChanged)
		{
			CmdSendPosition(transform.position);
			_lastPosition = transform.position;
		}

		var rotChanged = IsRotationChanged();

		// send rotation update if it is changed
		if (rotChanged)
		{
			CmdSendRotation(transform.localEulerAngles);
			_lastRotation = transform.localEulerAngles;
		}
	}

	/// <summary>
	/// Linear Interpolation between two vectors for the position.
	/// </summary>
	private void InterpolatePosition()
	{
		transform.position = Vector3.Lerp(transform.position, _lastPosition, Time.deltaTime * _posLerpRate);
	}

	/// <summary>
	/// Linear Interpolation between two vectors for the rotation.
	/// </summary>
	private void InterpolateRotation()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_lastRotation), Time.deltaTime * _rotLerpRate);
	}

	/// <summary>
	/// Send position to server.
	/// </summary>
	/// <param name="pos">Position of the client.</param>
	[Command(channel = Channels.DefaultUnreliable)]
	private void CmdSendPosition(Vector3 pos)
	{
		_lastPosition = pos;
	}

	/// <summary>
	/// Send rotation to server.
	/// </summary>
	/// <param name="rot">Rotation of the client.</param>
	[Command(channel = Channels.DefaultUnreliable)]
	private void CmdSendRotation(Vector3 rot)
	{
		_lastRotation = rot;
	}

	/// <summary>
	/// Check if the position is changed.
	/// </summary>
	/// <returns><c>true</c> if position changed (bigger than threshold); otherwise, <c>false</c>.</returns>
	private bool IsPositionChanged()
	{
		return Vector3.Distance(transform.position, _lastPosition) > _posThreshold;
	}
		
	/// <summary>
	/// Check if the rotation is changed.
	/// </summary>
	/// <returns><c>true</c> if rotation changed (bigger than threshold); otherwise, <c>false</c>.</returns>
	private bool IsRotationChanged()
	{
		return Vector3.Distance(transform.localEulerAngles, _lastRotation) > _rotThreshold;
	}
		
	/// <summary>
	/// Get the right network channel.
	/// </summary>
	/// <returns>The network channel.</returns>
	public override int GetNetworkChannel()
	{
		return Channels.DefaultUnreliable;
	}
		
	/// <summary>
	/// Network sending interval.
	/// </summary>
	/// <returns>The network send interval.</returns>
	public override float GetNetworkSendInterval()
	{
		return 0.01f;
	}
}
