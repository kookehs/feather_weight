using UnityEngine;
using System.Collections;

public class TwitchInputs : MonoBehaviour {

	public void Update(){
		GameObject[] twitch = GameObject.FindGameObjectsWithTag ("TwitchData");
		//destroy any extra twitch huds the one with data set has high chances for survival
		if (twitch.Length > 1) {
			for (int i = twitch.Length - 1; i > 0; i--) {
				if (twitch [i].GetComponent<TwitchIRC> ().channel_name.Equals (string.Empty)) {
					Destroy (twitch [i]);
				}
			}
		}
		TwitchIRC twitchData = twitch[0].GetComponent<TwitchIRC> ();
		GameObject chatHud = GameObject.FindGameObjectWithTag ("TwitchHUD");

		if (chatHud != null) {
			TwitchIRC chat = chatHud.GetComponent<TwitchIRC> ();

			chat.channel_name = twitchData.channel_name;
			chat.nickname = twitchData.nickname;
			chat.o_auth_token = twitchData.o_auth_token;

			twitchData.GetComponent<CanvasGroup> ().alpha = 0;
			twitchData.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			twitchData.GetComponent<CanvasGroup> ().interactable = false;
		} else {
			twitchData.GetComponent<CanvasGroup> ().alpha = 1;
			twitchData.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			twitchData.GetComponent<CanvasGroup> ().interactable = true;
		}

		DontDestroyOnLoad (twitch[0]);
	}
}
