﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class TwitchController : MonoBehaviour {
    private GameObject hud;
    private TwitchIRC irc;
    private ScenarioController scenario_controller;

    private List<string> captured_messages = new List<string>();
    private float captured_timer = 0.0f;
    public float max_catpured_time = 10.0f;

    public int max_messages = 250;
    private List<GameObject> messages = new List<GameObject>();

    private List<float> display_times = new List<float>();
    public float max_display_time = 3.0f;
    public int max_displayed_messages = 10;

    private DateTime last_write_time;
    public string interpret = "Nomad_Classifier/Interpret.py";
    public string interpret_output = "Nomad_Classifier/guess.txt";
    public string twitch_output = "Nomad_Classifier/twitch_output.txt";

    private void
    Awake() {
        hud = GameObject.Find("TwitchHUD");
        irc = GetComponent<TwitchIRC>();
        // This function will be called for every received message
        irc.irc_message_received_event.AddListener(MessageListener);
        scenario_controller = GameObject.Find("ScenarioController").GetComponent<ScenarioController>();
        last_write_time = File.GetLastWriteTime(interpret_output);
    }

    private void
    CreateMessage(string user, string message) {
        if (scenario_controller.IsInScenario()) {
            captured_messages.Add(message);
        }

        // Create a GameObject for every message, so we can display it
        GameObject twitch_message = new GameObject("TwitchMessage");
        twitch_message.SetActive(false);
        twitch_message.transform.SetParent(hud.transform);
        // TODO(bill): Figure out x,y based on message
        float x = 200.0f;
        float y = 200.0f;
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
        // Split string after the index of the command
        int message_start = message.IndexOf("PRIVMSG #");
        string text = message.Substring(message_start + irc.channel_name.Length + 11);
        string user = message.Substring(1, message.IndexOf('!') - 1);

        // Free up message GameObjects so we don't run out of memory
        if (messages.Count > max_messages) {
            Destroy(messages[0]);
            messages.RemoveAt(0);
            display_times.RemoveAt(0);
        }

        CreateMessage(user, text);
    }

    private void
    Update() {
        if (scenario_controller.IsInScenario()) {
            if (captured_timer >= max_catpured_time) {
                using (StreamWriter stream = new StreamWriter(twitch_output, false)) {
                    foreach (string line in captured_messages) {
                        stream.WriteLine(line);
                    }
                }

                // Create process for calling python code
                ProcessStartInfo process_info = new ProcessStartInfo();
                process_info.Arguments = interpret + " " + scenario_controller.GetCurrentScenarioName() + " " + twitch_output;
                process_info.FileName = "python.exe";
                process_info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(process_info);
                captured_timer = 0.0f;
                captured_messages.Clear();
            } else {
                captured_timer += Time.deltaTime;
            }
        }

        // Check if the python result file has updated
        DateTime write_time = File.GetLastWriteTime(interpret_output);

        if (last_write_time.Equals(write_time) == false) {
                last_write_time = write_time;
                string function_name = File.ReadAllText(interpret_output);
                scenario_controller.UpdateTwitchCommand(function_name);
        }

        // Queue a limited number of messages for display
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
