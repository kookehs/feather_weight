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
    private Text displayed_messages;

    private DateTime last_write_time;
    public string interpret = "Nomad_Classifier/Interpret.py";
    public string interpret_output = "Nomad_Classifier/guess.txt";
    public string interpret_output_copy = "Nomad_Classifier/guess_copy.txt";
    public string twitch_output = "Nomad_Classifier/twitch_output.txt";
    public string twitch_influence_output = "Data/twitch_influence.txt";

    public float influence_amount = 0.1f;
    private float influence_timer = 0.0f;
    public float max_influence_time = 60.0f;
    private Dictionary<string, float> twitch_users = new Dictionary<string, float>();

    string instructions;
    public float max_slow_time = 30.0f;
    private bool slow_on = false;
    private float slow_timer = 0.0f;

    public float max_poll_major_time = 15.0f;
    public bool poll_major_choice = false;
    private List<KeyValuePair<string, int>> poll_results = new List<KeyValuePair<string, int>>();
    private float poll_major_timer = 0.0f;
    private List<string> poll_users = new List<string>();
    public List<List<string>> poll_choices = new List<List<string>>();

    public float max_poll_boss_time = 10.0f;
    public bool poll_boss_choice = false;
    private float poll_boss_timer = 0.0f;

    public float max_save_time = 30.0f;
    private float save_timer = 0.0f;

    private GameObject the_world;

    private GameObject twitch_banner_gui;
    public float max_banner_time = 5.0f;
    private float banner_timer = 0.0f;
    private List<string> banner_queue = new List<string>();

    private void
    AddUser(string user, float influence) {
        twitch_users.Add(user, influence);
    }

    private void
    Awake() {
        hud = GameObject.Find("ChatHUD");
        the_world = GameObject.Find("WorldContainer");
        twitch_banner_gui = GameObject.FindGameObjectWithTag("TwitchCommand");
        twitch_banner_gui.SetActive(false);

        if (GameObject.Find("TwitchContents") != null)
            displayed_messages = GameObject.Find("TwitchContents").GetComponent<Text>();

        if (GameObject.Find("PlayerUIClean") != null) {
            irc = GameObject.Find("PlayerUIClean").GetComponentInChildren<TwitchIRC>();

            // This function will be called for every received message
            irc.irc_message_received_event.AddListener(MessageListener);
        }

        scenario_controller = the_world.GetComponent<ScenarioController>();
        last_write_time = File.GetLastWriteTime(interpret_output);
        instructions = "Welcome to Panopticon! Type statements to stop the nomad's progress! Ex. \"that bear attacks you\". If we aren't able to parse your statement, we will let you know. All actions cost 100 influence. Collaboration between chatters is encouraged. To hide your chat prefix your statements with \"ooc\" Happy Panopticonning!";

        string[] set_one = {"Permanent Day", "2x Night Speed", "Always Killer Bunnies"};
        string[] set_two = {"Permanent Night", "2x Day Speed", "All Bears Give Birth"};
        string[] set_three = {"Intense Sun", "Shatter Ladders", "Chase Player"};
        // string[] set_four = {"Fire Starter", "Famine", "Shatter Bridges"};
        poll_choices.Add(new List<string>(set_one));
        poll_choices.Add(new List<string>(set_two));
        poll_choices.Add(new List<string>(set_three));
        // poll_choices.Add(new List<string>(set_four));
        LoadUsers();
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
        // Prevent repeated answers and capture messages to send off to Python
        bool capture = (influence > 0) ? true : false;

        if (capture && CheckIfRepeated(user) == false) {
                captured_messages.Add(new KeyValuePair<string, string>(user, influence + " " + message));
        }

        string offset = (capture == true) ? ": " : string.Empty;
        displayed_messages.text += user + offset + message + "\n";
        hud.transform.FindChild("Scrollbar").GetComponent<Scrollbar>().value = 0.0f;
    }

    private void
    LoadUsers() {
        if (File.Exists(twitch_influence_output) == false)
            return;

        using (StreamReader stream = new StreamReader(twitch_influence_output)) {
            string line = string.Empty;

            while ((line = stream.ReadLine()) != null) {
                string[] keyvalue = line.Split(',');
                twitch_users.Add(keyvalue[0], float.Parse(keyvalue[1]));
            }
        }
    }

    private void
    MessageListener(string message) {
        if (message.StartsWith("ping ")) {
            irc.IRCPutCommand(message.Replace("ping", "PONG"));
        } else if (message.Split(' ')[1] == "001") {
            // 001 command is received after successful connection
            // Requests must come before joining a channel
            // This allows us to receive JOIN and PART
            irc.IRCPutCommand("CAP REQ :twitch.tv/membership");
            irc.IRCPutCommand("JOIN #" + irc.channel_name);
            SendInstructions();
        } else if (message.Contains("join #" + irc.channel_name)) {
            int user_end = message.IndexOf("!");
            string user = message.Substring(1, user_end - 1);

            if (user != irc.channel_name) {
               SendInstructions(user);
            }
        } else if (message.Contains("privmsg #")) {
            // Split string after the index of the command
            int message_start = message.IndexOf("privmsg #");
            string text = message.Substring(message_start + irc.channel_name.Length + 11);
            string user = message.Substring(1, message.IndexOf('!') - 1);

            if (user == irc.channel_name)
                return;

            if (text.StartsWith("@panopticonthegame")) {
                UnityEngine.Debug.Log("there");
                if (text.Contains("influence") && twitch_users.ContainsKey(user)) {
                    UnityEngine.Debug.Log("here");
                    irc.WhisperPutMessage(user, "Your current influence is: " + twitch_users[user]);
                }

                return;
            }

            if (text.StartsWith("ooc")) {
                int start = text.IndexOf('@');
                int end = -1;

                if (start != -1) {
                    int space = text.IndexOf(' ', start);
                    int comma = text.IndexOf(',', start);

                    if (space < comma && space != -1) {
                        end = space - 1;
                    } else if (comma < space && comma != -1) {
                        end = comma - 1;
                    }
                }

                string other = string.Empty;

                if (start != -1 && end != -1) {
                    other = text.Substring(start + 1, end - start);
                    CreateMessage(user, 0, " and " + other + " is scheming against you!");
                } else {
                    CreateMessage(user, 0, " is scheming against you!");
                }

                return;
            }

            bool voted = false;
            int num = 0;

            foreach (string name in poll_users) {
                if (name == user) {
                    voted = true;
                    break;
                }
            }

            if ((poll_boss_choice == true || poll_major_choice == true) && Int32.TryParse(text, out num) && !voted) {
                poll_users.Add(user);

                for (int i = 0; i < poll_results.Count; ++i) {
                    if (num - 1 == i) {
                        KeyValuePair<string, int> pair = poll_results[i];
                        poll_results[i] = new KeyValuePair<string, int>(pair.Key, pair.Value + 1);
                        break;
                    }
                }
            }

            float influence = 0;

            if (twitch_users.ContainsKey(user) == false)
                AddUser(user, 0.1f);

            influence = twitch_users[user];
            CreateMessage(user, influence, text);
        }
    }

    private void
    PollBossChoice() {
        irc.IRCPutMessage("/slow +" + max_slow_time);
        slow_on = true;
        irc.IRCPutMessage("During the duration of the boss fight you may enter a number from 1 to 12.");

        for (int i = 0; i < 12; ++i) {
            poll_results.Add(new KeyValuePair<string, int>(i.ToString(), 0));
        }

        poll_boss_choice = true;
    }

    private void
    PollMajorChoice() {
        banner_queue.Add("Major poll is in progress!");
        irc.IRCPutMessage("/slow +" + max_slow_time);
        slow_on = true;
        irc.IRCPutMessage("Please vote for one of the following by responding with a number!");
        WorldContainer world = the_world.GetComponent<WorldContainer>();
        List<string> choices = poll_choices[world.RandomChance(3)];
        string poll_message = "";

        for (int i = 0; i < choices.Count; ++i) {
            poll_message = (i + 1) + ") " + choices[i];
            poll_results.Add(new KeyValuePair<string, int>(choices[i], 0));
            irc.IRCPutMessage(poll_message);
        }

        poll_major_choice = true;
    }

    private void
    SendFeedback(string feedback) {
        for (int i = 0; i < feedback.Length; ++i) {
            if (feedback[i] == '0' && i < captured_messages.Count) {
                irc.WhisperPutMessage(captured_messages[i].Key, "This feature is not currently implemented, but we have taken note of it! You said: " + captured_messages[i].Value.Split(' ')[1]);
            }
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
        if (!poll_boss_choice && the_world.GetComponent<WorldContainer>().BOSS) {
            PollBossChoice();
        }

        if ((!poll_major_choice && scenario_controller.curr_GI >= scenario_controller.MAX_GI) && !poll_boss_choice) {
           PollMajorChoice();
        }

        if (slow_on == true) {
            if (slow_timer >= max_slow_time) {
                slow_on = false;
                slow_timer = 0.0f;
                irc.IRCPutMessage("/slowoff");
            } else {
                slow_timer += Time.deltaTime;
            }
        }

        if (poll_boss_choice == true) {
            if (poll_boss_timer >= max_poll_boss_time) {
                poll_boss_timer = 0.0f;
                string result = "";
                int max = 0;

                for (int i = 0; i < poll_results.Count; ++i) {
                    if (poll_results[i].Value > max) {
                        max = poll_results[i].Value;
                        result = poll_results[i].Key;
                    }
                }

                if (max != 0) {
                    for (int i = 0; i < poll_results.Count; ++i) {
                        KeyValuePair<string, int> pair = poll_results[i];
                        poll_results[i] = new KeyValuePair<string, int>(pair.Key, 0);
                    }

                    poll_users.Clear();
                    string command = "FireLightning_" + result;
                    scenario_controller.UpdateTwitchCommand(command);
                }
            } else {
                poll_boss_timer += Time.deltaTime;
            }
        } else if (poll_major_choice == true) {
            if (poll_major_timer >= max_poll_major_time) {
                poll_major_choice = false;
                poll_major_timer = 0.0f;
                string result = "";
                int max = 0;

                for (int i = 0; i < poll_results.Count; ++i) {
                    if (poll_results[i].Value > max) {
                        max = poll_results[i].Value;
                        result = poll_results[i].Key;
                    }
                }


                banner_queue.Add("Twitch has collectively decided on " + result);
                scenario_controller.UpdateTwitchCommand("Poll " + result);
                UnityEngine.Debug.Log("Major: " + result);
                poll_results.Clear();
                poll_users.Clear();
            } else {
                poll_major_timer += Time.deltaTime;
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

        if (save_timer >= max_save_time) {
                using (StreamWriter stream = new StreamWriter(twitch_influence_output, false)) {
                    foreach (KeyValuePair<string, float> user in twitch_users) {
                        stream.WriteLine(user.Key + "," + user.Value);
                    }
                }

                save_timer = 0.0f;
        } else {
                save_timer += Time.deltaTime;
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
                process_info.FileName = "python.exe";
                process_info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(process_info);
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
                banner_queue.Add(function_name);
                SendFeedback(feedback);
                captured_messages.Clear();
                last_write_time = write_time;
            }
        }

        if (twitch_banner_gui.activeSelf) {
                banner_timer += Time.deltaTime;

                if (banner_timer >= max_banner_time) {
                        twitch_banner_gui.SetActive(false);
                        banner_timer = 0.0f;
                }
        }

        UpdateTwitchBanner();
    }

    private void
    UpdateTwitchBanner() {
        // UnityEngine.Debug.Log("Count: " + banner_queue.Count);
        if (banner_queue.Count < 1) {
                return;
        }

        string command = banner_queue[0];
        banner_queue.RemoveAt(0);

        if (command == "setFire") {
                return;
        }

        if (command == "createMountainLion") {
            command = "Twitch has spawned a mountain lion";
        } else if (command == "createBunny") {
            command = "Twitch has spawned a bunny";
        } else if (command == "createBear") {
            command = "Twitch has spawned a bear";
        } else if (command == "giveAcorn") {
            command = "Twitch has spawned an acorn";
        } else if (command == "fallOnPlayer") {
            command = "Twitch has made a tree fall";
        } else if (command == "growTree") {
            command = "Twitch has grown a tree from an acorn";
        } else if (command == "spawnBearCub") {
            command = "Twitch has spawned a bear cub";
        } else if (command == "runAway_bear") {
            command = "Twitch has made a bear run away";
        } else if (command == "killerBunny") {
            command = "Twitch has spawned a killer bunny";
        }

        twitch_banner_gui.SetActive(true);
        twitch_banner_gui.GetComponentInChildren<Text>().text = command;
    }
}
