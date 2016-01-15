using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TwitchController : MonoBehaviour {
	private GameObject hud;
	private TwitchIRC irc;

	private int max_messages = 250;
	private LinkedList<GameObject> messages = new LinkedList<GameObject>();

	private float display_timer = 0.0f;
	private float max_display_time = 3.0f;
	private int max_displayed_messages = 10;

	private void
	Awake() {
		hud = GameObject.Find("TwitchHUD");
		irc = GetComponent<TwitchIRC>();
	}

	private void
	DrawMessage(float x, float y, string user, string message) {
		GameObject twitch_message = new GameObject("TwitchMessage");
		Text twitch_text = twitch_message.AddComponent<Text>();
		twitch_text.alignment = TextAnchor.UpperCenter;
		twitch_text.color = Color.black;
		twitch_text.fontSize = 18;
		twitch_text.horizontalOverflow = HorizontalWrapMode.Overflow;
		twitch_text.SetActive(false);
		twitch_text.text = user + ": " + message;
		twitch_text.transform.position = new Vector3(x, y, 0.0f);
		twitch_text.transform.SetParent(hud.transform);

		messages.AddLast(twitch_message);
	}

	private void
	MessageListener(string message) {
		int message_start = message.IndexOf("PRIVMSG #");
		string text = message.Substring(message_start + irc.channel_name.Length + 11);
		string user = message.Substring(1, message.IndexOf('!') - 1);

		if (messages.Count > max_messages) {
			Destroy(messages.First.Value);
			messages.RemoveFirst();
		}

		float x = 100.0f;
		float y = 100.0f;

		DrawMessage(x, y, user, text);
	}

	private void
	Start() {
		irc.irc_message_received_event.AddListener(MessageListener);
	}

	private void
	Update() {
		if ()
	}
}
