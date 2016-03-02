using UnityEngine;
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

    private List<KeyValuePair<string, string>> captured_messages = new List<KeyValuePair<string, string>>();
    // private List<string> captured_messages = new List<string>();
    private float captured_timer = 0.0f;
    public float max_catpured_time = 10.0f;

    public int max_messages = 250;
    private List<GameObject> messages = new List<GameObject>();
    public int max_displayed_messages = 10;

    private DateTime last_write_time;
    public string interpret = "Nomad_Classifier/Interpret.py";
    public string interpret_output = "Nomad_Classifier/guess.txt";
    public string interpret_output_copy = "Nomad_Classifier/guess_copy.txt";
    public string twitch_output = "Nomad_Classifier/twitch_output.txt";

    public float influence_amount = 0.1f;
    private float influence_timer = 0.0f;
    public float max_influence_time = 60.0f;
    private Dictionary<string, float> twitch_users = new Dictionary<string, float>();

    private void
    AddUser(string user, float influence) {
        twitch_users.Add(user, influence);
    }

    private void
    Awake() {
        hud = GameObject.Find("ChatHUD");
        irc = GetComponent<TwitchIRC>();
        // This function will be called for every received message
        irc.irc_message_received_event.AddListener(MessageListener);
        scenario_controller = GameObject.Find("ScenarioController").GetComponent<ScenarioController>();
        last_write_time = File.GetLastWriteTime(interpret_output);
    }

    private bool
    CheckIfRepeated(string user) {
        foreach (KeyValuePair<string, string> line in captured_messages) {
            if (line.Key == user)
                return true;
        }

        return false;
    }

    private void
    CreateMessage(string user, float influence, string message) {
        // Prevent repeated answers
        if (CheckIfRepeated(user) == true)
            return;

        // Capture messages to send off to Python
        captured_messages.Add(new KeyValuePair<string, string>(user, influence + " " + message));

        // Create a GameObject for every message, so we can display it
        GameObject twitch_message = new GameObject("TwitchMessage");
        twitch_message.SetActive(false);
        twitch_message.transform.SetParent(hud.transform);
        twitch_message.transform.position = hud.transform.position;

        LayoutElement layout = twitch_message.AddComponent<LayoutElement>();
        layout.minHeight = 20.0f;

        Text twitch_text = twitch_message.AddComponent<Text>();
        twitch_text.alignment = TextAnchor.MiddleLeft;
        twitch_text.color = Color.white;
        twitch_text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        twitch_text.fontSize = 18;
        twitch_text.horizontalOverflow = HorizontalWrapMode.Overflow;
        twitch_text.text = user + ": " + message;

        messages.Add(twitch_message);
    }

    private void
    MessageListener(string message) {
        if (message.StartsWith("PING ")) {
            irc.IRCPutCommand(message.Replace("PING", "PONG"));
        } else if (message.Split(' ')[1] == "001") {
            // 001 command is received after successful connection
            irc.IRCPutCommand("JOIN #" + irc.channel_name);
        } else if (message.Contains("PRIVMSG #")) {
            // Split string after the index of the command
            int message_start = message.IndexOf("PRIVMSG #");
            string text = message.Substring(message_start + irc.channel_name.Length + 11);
            string user = message.Substring(1, message.IndexOf('!') - 1);

            // Free up message GameObjects so we don't run out of memory
            if (messages.Count > max_messages) {
                Destroy(messages[0]);
                messages.RemoveAt(0);
            }

            float influence = 0;

            if (twitch_users.ContainsKey(user) == false)
                AddUser(user, 0.1f);

            influence = twitch_users[user];
            CreateMessage(user, influence, text);
        }
    }

    private void
    SendFeedback(string feedback) {
        for (int i = 0; i < feedback.Length; ++i) {
            if (feedback[i] == 0)
                irc.WhisperPutMessage(captured_messages[i].Key, "This feature is not currently implemented.");
        }
    }

    private void
    Update() {
        if (Input.GetKeyDown("m")) {
            irc.IRCPutCommand("NAMES #kookehs");
        }

        if (influence_timer >= max_influence_time) {
            influence_timer = 0.0f;

           List<string> keys = new List<string>(twitch_users.Keys);

            foreach (string key in keys) {
                twitch_users[key] +=  influence_amount;
            }
        } else {
            influence_timer += Time.deltaTime;
        }

        if (captured_timer >= max_catpured_time) {
            captured_timer = 0.0f;

            if (captured_messages.Count > 0) {
                using (StreamWriter stream = new StreamWriter(twitch_output, false)) {
                    foreach (KeyValuePair<string, string> line in captured_messages) {
                        stream.WriteLine(line.Value);
                    }
                }

                // Create process for calling python code
                ProcessStartInfo process_info = new ProcessStartInfo();
                UnityEngine.Debug.Log(scenario_controller.GetCurrentScenarioName());
                process_info.Arguments = interpret + " " + scenario_controller.GetCurrentScenarioName() + " " + twitch_output;
                process_info.FileName = "python.exe";
                process_info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(process_info);
                captured_messages.Clear();
                UnityEngine.Debug.Log("Sending");
            }
        } else {
            captured_timer += Time.deltaTime;
        }

        // Check if the python result file has updated
        DateTime write_time = new DateTime();

        if (File.Exists(interpret_output)) {
            write_time = File.GetLastWriteTime(interpret_output);

            if (last_write_time.Equals(write_time) == false) {
                UnityEngine.Debug.Log("Reading");
                File.Copy(interpret_output, interpret_output_copy, true);
                string function_name = string.Empty;
                string feedback = string.Empty;

                using (StreamReader stream = new StreamReader(interpret_output_copy)) {
                    function_name = stream.ReadLine();
                    feedback = stream.ReadLine();
                }

                UnityEngine.Debug.Log(function_name);
                scenario_controller.UpdateTwitchCommand(function_name);
                SendFeedback(feedback);
                last_write_time = write_time;
            }
        }

        // Queue a limited number of messages for display
        for (int i = 0; i < messages.Count; ++i) {
            if (i >= max_displayed_messages) {
                Destroy(messages[0]);
                messages.RemoveAt(0);
            } else {
                Vector3 position = new Vector3(100.0f, 200.0f - i * 20.0f, 0.0f);
                messages[i].transform.position = position;

                if (messages[i].activeSelf == false)
                    messages[i].SetActive(true);
            }
        }
    }
}
