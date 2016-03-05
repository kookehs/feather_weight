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

    string instructions;
    public float max_slow_time = 30.0f;
    private bool slow_on = false;
    private float slow_timer = 0.0f;

    private void
    AddUser(string user, float influence) {
        twitch_users.Add(user, influence);
    }

    private void
    Awake() {
        hud = GameObject.Find("ChatHUD");
		irc = GameObject.Find("PlayerUIClean").GetComponentInChildren<TwitchIRC>();
        // This function will be called for every received message
        irc.irc_message_received_event.AddListener(MessageListener);
        scenario_controller = GameObject.Find("WorldContainer").GetComponent<ScenarioController>();
        last_write_time = File.GetLastWriteTime(interpret_output);
        instructions = "Welcome to Panopticon! Type statements to stop the nomad's progress! Ex. \"that bear attacks you\". If we aren't able to parse your statement, we will let you know. Collaboration between chatters is encouraged. To hide your chat prefix your statements with \"OOC:\" Happy Panopticonning!";
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
            // Requests must come before joining a channel
            // This allows us to receive JOIN and PART
            irc.IRCPutCommand("CAP REQ :twitch.tv/membership");
            irc.IRCPutCommand("JOIN #" + irc.channel_name);
            SendInstructions();
        } else if (message.Contains("JOIN #" + irc.channel_name)) {
            int user_end = message.IndexOf("!");
            string user = message.Substring(1, user_end - 1);

            if (user != irc.channel_name)
               SendInstructions(user);
        } else if (message.Contains("PRIVMSG #")) {
            // Split string after the index of the command
            int message_start = message.IndexOf("PRIVMSG #");
            string text = message.Substring(message_start + irc.channel_name.Length + 11);

            if (text.StartsWith("OOC:"))
                return;

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
    SendInstructions() {
        // Put the room in slow mode so we can have instructions displayed
        irc.IRCPutMessage("/slow " + max_slow_time);
        slow_on = true;
        irc.IRCPutMessage(instructions);
    }

    private void
    SendInstructions(string user) {
        irc.WhisperPutMessage(user, instructions);
    }

    private void
    Update() {
        if (slow_on == true) {
            if (slow_timer >= max_slow_time) {
                slow_on = false;
                slow_timer = 0.0f;
                irc.IRCPutMessage("/slowoff");
            } else {
                slow_timer += Time.deltaTime;
                UnityEngine.Debug.Log(slow_timer);
            }
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

                // Create process for calling Python code
                ProcessStartInfo process_info = new ProcessStartInfo();
                UnityEngine.Debug.Log(scenario_controller.GetCurrentScenarioName());
                process_info.Arguments = interpret + " " + scenario_controller.GetCurrentScenarioName() + " " + twitch_output;
                process_info.FileName = "C:/Program Files (x86)/Python 3.5/python.exe";
                process_info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(process_info);
                captured_messages.Clear();
                UnityEngine.Debug.Log("Sending");
            }
        } else {
            captured_timer += Time.deltaTime;
        }

        // Check if the Python result file has updated
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
