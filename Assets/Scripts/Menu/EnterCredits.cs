using UnityEngine;
using System.Collections;

public class EnterCredits : MonoBehaviour {

	public int isGameOver = 0;//0 - entered credits from menu, 1 - twitch wins (streamer death), 2 - streamer wins (twitch fail)
	public string lineFeed = "___________________________";
	public string extraGameOverDialogT = "The Test Subject Has Escaped!";
	public string extraGameOverDialogB = "Sorry Twitch Maybe Next Time";
	public string twitchGameOverDialogT = "Congrats Everyone\nThe System Works Perfectly.";
	public string twitchGameOverDialogB = "Sorry Streamer Maybe Next Time.";

}
