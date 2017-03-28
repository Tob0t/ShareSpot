using UnityEngine;
using System.Collections;

/// <summary>
/// A Challenge is describing a task what a player needs to do.
/// </summary>
[System.Serializable]
public class Challenge {
	[SerializeField]
	private SharingMode sharingMode;	///< The sharing mode which is used in the challenge.
	private float startTime;	///< The start time of the challenge.
	private float endTime;	///< The end time of the challenge.
	[SerializeField]
	private float reactionTime;	///< The reaction time derive by comparing start with end time.
	[SerializeField]
	private int errors;	///< The amount of errors occured during performing the challenge.
	[SerializeField]
	private int receiverId;	///< The id of the receiver.
	[SerializeField]
	private string description;	///< The description of the challenge.

	/// <summary>
	/// Initializes a new instance of the <see cref="Challenge"/> class.
	/// </summary>
	/// <param name="sharingMode">The sharing mode.</param>
	/// <param name="receiverId">The id of the expected receiver.</param>
	public Challenge(SharingMode sharingMode, int receiverId){
		this.sharingMode = sharingMode;
		this.startTime = Time.time;
		this.errors = 0;
		this.receiverId = receiverId;
		this.description = "Share with " + Admin.Instance.ConnectedClients [receiverId].GetComponent<PlayerController> ().PlayerName;
	}

	/// <summary>
	/// Gets the description of the challenge.
	/// </summary>
	/// <returns>The description.</returns>
	public string GetDescription(){
		return this.description;
	}

	/// <summary>
	/// Gets the receiver identifier.
	/// </summary>
	/// <returns>The receiver identifier.</returns>
	public int GetReceiverId(){
		return this.receiverId;
	}

	/// <summary>
	/// Gets the start time.
	/// </summary>
	/// <returns>The start time.</returns>
	public float GetStartTime(){
		return this.startTime;
	}

	/// <summary>
	/// Sets the end time.
	/// </summary>
	/// <param name="endTime">End time.</param>
	public void SetEndTime(float endTime){
		this.endTime = endTime;
	}

	/// <summary>
	/// Increases the error counter.
	/// </summary>
	public void IncreaseError(){
		this.errors++;
	}

	/// <summary>
	/// Calculates the reaction time by comparing start and end time
	/// </summary>
	/// <returns>The reaction time.</returns>
	public float CalculateReactionTime(){
		this.reactionTime = Mathf.Round((endTime - startTime) * 100f) / 100f;
		return this.reactionTime;
	}

}
