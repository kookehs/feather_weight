using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TwitchController : MonoBehaviour {
    private GameObject hud;
    private TwitchIRC irc;

    private int max_messages = 250;
    private List<GameObject> messages = new List<GameObject>();

    private List<float> display_times = new List<float>();
    private float max_display_time = 3.0f;
    private int max_displayed_messages = 10;

    private void
    Awake() {
        hud = GameObject.Find("TwitchHUD");
        irc = GetComponent<TwitchIRC>();
        irc.channel_name = "kookehs";
        irc.nickname = "kookehs";
        irc.o_auth_token = "oauth:crj1dlsj8839qripdhwbj04cr7gec9";
        irc.irc_message_received_event.AddListener(MessageListener);
    }

    private void
    CreateMessage(float x, float y, string user, string message) {
       GameObject twitch_message = new GameObject("TwitchMessage");
       twitch_message.SetActive(false);
       twitch_message.transform.SetParent(hud.transform);
       twitch_message.transform.position = new Vector3(x, y, 0.0f);

       LayoutElement layout = twitch_message.AddComponent<LayoutElement>();
       layout.minHeight = 20.0f;

       Text twitch_text = twitch_message.AddComponent<Text>();
       twitch_text.alignment = TextAnchor.MiddleCenter;
       twitch_text.color = Color.black;
       twitch_text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
       twitch_text.fontSize = 18;
       twitch_text.horizontalOverflow = HorizontalWrapMode.Overflow;
       twitch_text.text = user + ": " + message;

       messages.Add(twitch_message);
       display_times.Add(0.0f);
    }

    private void
    MessageListener(string message) {
        Debug.Log(message);
        int message_start = message.IndexOf("PRIVMSG #");
        string text = message.Substring(message_start + irc.channel_name.Length + 11);
        string user = message.Substring(1, message.IndexOf('!') - 1);

        if (messages.Count > max_messages) {
            Destroy(messages[0]);
            messages.RemoveAt(0);
            display_times.RemoveAt(0);
        }

        float x = 200.0f;
        float y = 200.0f;

        CreateMessage(x, y, user, text);
    }

    private void
    Update() {
        for (int i = 0; i < max_displayed_messages && i < messages.Count; ++i) {
            if (display_times[i] >= max_display_time) {
                Destroy(messages[i]);
                messages.RemoveAt(i);
                display_times.RemoveAt(i);
            } else {
                display_times[i] += Time.deltaTime;

                if (messages[i].activeSelf == false)
                        messages[i].SetActive(true);
            }
        }
    }
}
