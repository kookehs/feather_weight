using UnityEngine;
using System.Collections;

public class TwitchInputs : MonoBehaviour {

        public void Awake () {
            DontDestroyOnLoad (gameObject);
        }

	public void Update(){
		GameObject[] twitch = GameObject.FindGameObjectsWithTag ("TwitchData");
		//destroy any extra twitch huds the one with data set has high chances for survival
		if (twitch.Length > 1) {
			for (int i = twitch.Length - 1; i > 0; i--) {
				if (twitch [i].GetComponent<SaveTwitchData> ().channel_name.Equals (string.Empty)) {
					Destroy (twitch [i]);
				}
			}
			if (twitch.Length > 1) {
				for (int i = twitch.Length - 1; i > 0; i--) {
					Destroy (twitch [i]);
					if (twitch.Length <= 1)
						break;
				}
			}
		}
		GameObject twitchData = twitch[0];
		GameObject chatHud = GameObject.FindGameObjectWithTag ("TwitchHUD");
		if (twitchData == null) {
			twitchData = Resources.Load ("TwitchData") as GameObject;
		}

		if (chatHud != null) {
			TwitchIRC chat = chatHud.GetComponent<TwitchIRC> ();

			chat.channel_name = twitchData.GetComponent<SaveTwitchData> ().channel_name;
			chat.nickname = twitchData.GetComponent<SaveTwitchData> ().nickname;
			chat.o_auth_token = twitchData.GetComponent<SaveTwitchData> ().o_auth_token;

			twitchData.GetComponent<CanvasGroup> ().alpha = 0;
			twitchData.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			twitchData.GetComponent<CanvasGroup> ().interactable = false;
		} else {
			twitchData.GetComponent<CanvasGroup> ().alpha = 1;
			twitchData.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			twitchData.GetComponent<CanvasGroup> ().interactable = true;
		}
	}
}
