using UnityEngine;
using System.Collections;

[System.Serializable]
public class Challenge {
	[SerializeField]
	private SharingMode sharingMode;
	private float startTime;
	private float endTime;
	[SerializeField]
	private float reactionTime;
	[SerializeField]
	private int errors;
	[SerializeField]
	private int receiverId;
	[SerializeField]
	private string description;

	public Challenge(SharingMode sharingMode, int receiverId){
		this.sharingMode = sharingMode;
		this.startTime = Time.time;
		this.errors = 0;
		this.receiverId = receiverId;
		this.description = "Share with " + Admin.Instance.ConnectedClients [receiverId].GetComponent<PlayerController> ().PlayerName;
	}

	public string GetDescription(){
		return this.description;
	}

	public int GetReceiverId(){
		return this.receiverId;
	}

	public float GetStartTime(){
		return this.startTime;
	}

	public void SetEndTime(float endTime){
		this.endTime = endTime;
	}

	public void IncreaseError(){
		this.errors++;
	}

	public float CalculateReactionTime(){
		this.reactionTime = Mathf.Round((endTime - startTime) * 100f) / 100f;
		return this.reactionTime;
	}

}
