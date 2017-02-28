using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
// Adminstration to create connections between Administration panel and PlayerController
/// </summary>
public class Admin : AManager<Admin> {

	#region [Public fields]
	public Text PlayerDebug;
	public GameObject CurrentTrackedPlayer;
	public RectTransform PlayersPanel;
	public Button ButtonClientOne;
	public Button ButtonClientTwo;
	public Button ButtonDisconnect;
	public GameObject ClientOne;
	public GameObject ClientTwo;

	#endregion

	// Creates connection between the selected TrackedPlayer obtained from the Tracking-Server with ClientOne
	// who is connected to the network
	public void ChooseClientOne()
	{
		if (CurrentTrackedPlayer != null)
		{
			ClientOne.GetComponent<PlayerController>().ControllingPlayer = CurrentTrackedPlayer;
			ClientOne.GetComponent<PlayerController>().HasControllingPlayer = true;
			CurrentTrackedPlayer.GetComponent<TrackedPlayerNetwork>().ControlledPlayer = ClientOne;
			CurrentTrackedPlayer.GetComponent<TrackedPlayerNetwork>().HasPlayer = true;
			//ButtonClientOne.interactable = false;

		}

	}
	// Creates connection between the selected TrackedPlayer obtained from the Tracking-Server with ClientTwo
	// who is connected to the network
	public void ChooseClientTwo()
	{
		if (CurrentTrackedPlayer != null)
		{
			ClientTwo.GetComponent<PlayerController>().ControllingPlayer = CurrentTrackedPlayer;
			ClientTwo.GetComponent<PlayerController> ().HasControllingPlayer = true;
			CurrentTrackedPlayer.GetComponent<TrackedPlayerNetwork>().ControlledPlayer = ClientTwo;
			CurrentTrackedPlayer.GetComponent<TrackedPlayerNetwork>().HasPlayer = true;
			//ButtonClientTwo.interactable = false;

		}
	}


}
