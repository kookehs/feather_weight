using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public class TwitchController : MonoBehaviour {
    private static GameObject hud;
    private static TwitchIRC irc;

    private static List<KeyValuePair<string, string>> captured_messages = new List<KeyValuePair<string, string>>();
    private static float captured_timer = 0.0f;
    public static float max_captured_time = 10.0f;
    private static Text displayed_messages;

    // public string twitch_influence_output = "Data/twitch_influence.txt";

    public static float influence_amount = 0.1f;
    private static float influence_timer = 0.0f;
    public static float max_influence_time = 60.0f;
    private static Dictionary<string, float> twitch_users = new Dictionary<string, float>();
    private static List<string> used_names = new List<string>();

    private static string instructions;
    public static float max_slow_time = 30.0f;
    private static bool slow_on = false;
    private static float slow_timer = 0.0f;

    private static List<KeyValuePair<string, int>> _poll_results = new List<KeyValuePair<string, int>>();
    private static List<string> poll_users = new List<string>();

    public static float max_poll_shop_time = 3600.0f;
    private static float poll_shop_timer = 0.0f;

    public static float max_poll_boss_time = 10.0f;
    public static bool poll_boss_choice = false;
    private static float poll_boss_timer = 0.0f;

    public static float max_save_time = 30.0f;
    private static float save_timer = 0.0f;

    private static GameObject twitch_banner_gui;
    private static GameObject twitch_action;
    public static float max_banner_time = 5.0f;
    private static float banner_timer = 0.0f;
    private static List<string> banner_queue = new List<string>();

    public static List<KeyValuePair<string, int>> poll_results {
        get {return _poll_results;}
        set {_poll_results = value;}
    }

    public static void
    AddToBannerQueue(string message) {
        banner_queue.Add(message);
    }

    private static void
    AddUser(string user, float influence) {
        twitch_users.Add(user, influence);
    }

    private void
    Awake() {
        GameObject playerUICurrent = GameObject.Find ("PlayerUICurrent");

        if (GameObject.Find("TwitchContents") != null) {
            displayed_messages = GameObject.Find("TwitchContents").GetComponent<Text>();
        }

        if (playerUICurrent != null) {
            hud = playerUICurrent.transform.FindChild("ChatHUD").gameObject;
            irc = playerUICurrent.GetComponentInChildren<TwitchIRC>();
            twitch_banner_gui = playerUICurrent.transform.FindChild("TwitchActionPopUp").gameObject;
            twitch_action = GameObject.Find("TwitchAction");
            twitch_action.SetActive(false);
            twitch_banner_gui.SetActive(false);

            // This function will be called for every received message
            TwitchIRC.irc_message_received_event.AddListener(MessageListener);
        }

        // TODO(bill): Update instructions to refelect new design
        instructions = "Welcome to Panopticon! Type statements to stop the nomad's progress! Ex. \"that bear attacks you\". If we aren't able to parse your statement, we will let you know. All actions cost 100 influence. Collaboration between chatters is encouraged. To hide your chat prefix your statements with \"ooc\" Happy Panopticonning!";
        LoadUsers();
    }

    private static bool
    CheckIfRepeated(string user) {
        foreach (KeyValuePair<string, string> line in captured_messages) {
            if (line.Key == user)
                return true;
        }

        return false;
    }

    private static void
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

    private static void
    LoadUsers() {
        /*if (File.Exists(twitch_influence_output) == false)
            return;

        using (StreamReader stream = new StreamReader(twitch_influence_output)) {
            string line = string.Empty;

            while ((line = stream.ReadLine()) != null) {
                string[] keyvalue = line.Split(',');
                twitch_users.Add(keyvalue[0], float.Parse(keyvalue[1]));
            }
        }*/
    }

    private static void
    MessageListener(string message) {
        if (message.StartsWith("ping ")) {
            TwitchIRC.IRCPutCommand(message.Replace("ping", "PONG"));
        } else if (message.Split(' ')[1] == "001") {
            // 001 command is received after successful connection
            // Requests must come before joining a channel
            // This allows us to receive JOIN and PART
            TwitchIRC.IRCPutCommand("CAP REQ :twitch.tv/membership");
            TwitchIRC.IRCPutCommand("JOIN #" + TwitchIRC.channel_name);
            SendInstructions();
        } else if (message.Contains("join #" + TwitchIRC.channel_name)) {
            int user_end = message.IndexOf("!");
            string user = message.Substring(1, user_end - 1);

            if (user != TwitchIRC.channel_name) {
               SendInstructions(user);
            }
        } else if (message.Contains("privmsg #")) {
            // Split string after the index of the command
            int message_start = message.IndexOf("privmsg #");
            string text = message.Substring(message_start + TwitchIRC.channel_name.Length + 11);
            string user = message.Substring(1, message.IndexOf('!') - 1);

            if (user == TwitchIRC.channel_name)
                return;

            if (text.StartsWith("@panopticonthegame")) {
                if (text.Contains("influence") && twitch_users.ContainsKey(user)) {
                    TwitchIRC.WhisperPutMessage(user, "Your current influence is: " + twitch_users[user]);
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

            if (WaveController.shop_phase == true) {
                int voted = poll_users.IndexOf(user);
                int num = 0;

                if (voted == -1 && Int32.TryParse(text, out num)) {
                    if (num > -1 && num < _poll_results.Count) {
                        poll_users.Add(user);
                        int offset = (poll_boss_choice == true) ? 0 : -1;
                        KeyValuePair<string, int> pair = _poll_results[num - offset];
                        _poll_results[num - offset] = new KeyValuePair<string, int>(pair.Key, pair.Value + 1);
                    }
                }
            }

            float influence = 0;

            if (twitch_users.ContainsKey(user) == false) {
                AddUser(user, 0.1f);
            }

            influence = twitch_users[user];
            CreateMessage(user, influence, text);
        }
    }

    private static void
    PollBossChoice() {
        if (poll_boss_choice == false && WorldContainer.BOSS) {
            //SlowModeOn(max_slow_time);
            TwitchIRC.IRCPutMessage("During the duration of the boss fight you may enter a number from 1 to 12.");

            for (int i = 0; i < 12; ++i) {
                _poll_results.Add(new KeyValuePair<string, int>(i.ToString(), 0));
            }

            poll_boss_choice = true;
        } else if (poll_boss_choice == true) {
            if (poll_boss_timer >= max_poll_boss_time) {
                poll_boss_timer = 0.0f;
                string result = "";
                int max = 0;

                for (int i = 0; i < _poll_results.Count; ++i) {
                    if (_poll_results[i].Value > max) {
                        max = _poll_results[i].Value;
                        result = _poll_results[i].Key;
                    }
                }

                if (max != 0) {
                    for (int i = 0; i < _poll_results.Count; ++i) {
                        KeyValuePair<string, int> pair = _poll_results[i];
                        _poll_results[i] = new KeyValuePair<string, int>(pair.Key, 0);
                    }

                    poll_users.Clear();
                    string command = "FireLightning_" + result;
                    // TODO(tai): Call function to fire lightning
                }
            } else {
                poll_boss_timer += Time.deltaTime;
            }
        }
    }

    private static void
    PollShopChoice() {
        if (WaveController.shop_phase == true) {
            if (poll_shop_timer >= WaveController.max_shop_time) {
                poll_shop_timer = 0.0f;
                string result = "";
                int max = 0;

                for (int i = 0; i < _poll_results.Count; ++i) {
                    if (_poll_results[i].Value > max) {
                        max = _poll_results[i].Value;
                        result = _poll_results[i].Key;
                    }
                }

                poll_users.Clear();
                _poll_results.Clear();
                AddToBannerQueue(result);
                TwitchActionController.Purchase(result);
            } else {
                poll_shop_timer += Time.deltaTime;
            }
        }
    }

    public static string
    RandomUser() {
        int index = WorldContainer.RandomChance(twitch_users.Count);
        List<string> users = new List<string>(twitch_users.Keys);
        string user = users[index];
        int used = used_names.IndexOf(user);
        // TODO(bill): Remove the following if reuse of names is unwanted
        used = -1;
        return (used == -1) ? user : "NULL";
    }

    private static void
    SendFeedback(string feedback) {
        for (int i = 0; i < feedback.Length; ++i) {
            if (feedback[i] == '0' && i < captured_messages.Count) {
                TwitchIRC.WhisperPutMessage(captured_messages[i].Key, "This feature is not currently implemented, but we have taken note of it! You said: " + captured_messages[i].Value.Split(' ')[1]);
            }
        }
    }

    private static void
    SendInstructions() {
        // Put the room in slow mode so we can have instructions displayed
        //SlowModeOn(max_slow_time);
        TwitchIRC.IRCPutMessage(instructions);
    }

    private static void
    SendInstructions(string user) {
        TwitchIRC.WhisperPutMessage(user, instructions);
    }

    public static void
    SetupShop() {
        List<string> verbs = TwitchActionController.VerbShop(3);

        for (int i = 0; i < verbs.Count; ++i) {
            string verb = verbs[i];
            _poll_results.Add(new KeyValuePair<string, int>(verb, 0));
            int index = i + 1;
            GameObject.Find("Verb" + index + "Text").GetComponent<Text>().text = index + ". " + verb;
        }

        try {
            GameObject.Find("PlayerUICurrent").transform.FindChild("EventSystem").gameObject.SetActive(false);
        } catch (Exception e) {
            UnityEngine.Debug.Log(e.Message);
        }
    }

    public static void
    SlowModeOn(float time) {
        TwitchIRC.IRCPutMessage("/slow " + time);
        slow_on = true;
    }

    public static void
    SlowModeOff() {
        TwitchIRC.IRCPutMessage("/slowoff");
    }

    private void
    Update() {
        // TODO(bill): Enable when boss is / if implemented
        // PollBossChoice();
        PollShopChoice();

        if (slow_on == true) {
            if (slow_timer >= max_slow_time) {
                slow_on = false;
                slow_timer = 0.0f;
                //SlowModeOff();
            } else {
                slow_timer += Time.deltaTime;
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

        /*if (save_timer >= max_save_time) {
            using (StreamWriter stream = new StreamWriter(twitch_influence_output, false)) {
                foreach (KeyValuePair<string, float> user in twitch_users) {
                    stream.WriteLine(user.Key + "," + user.Value);
                }
            }

            save_timer = 0.0f;
        } else {
            save_timer += Time.deltaTime;
        }*/

        if (captured_timer >= max_captured_time) {
            captured_timer = 0.0f;

            if (captured_messages.Count > 0) {
                List<string> messages = new List<string>();

                foreach (KeyValuePair<string, string> pair in captured_messages) {
					messages.Add(twitch_users[pair.Key] + " " + pair.Value);
                }

                StringReader.ReadStrings(messages);
				TwitchActionController.Do (StringReader.command, StringReader.effect, StringReader.hex);
                //twitch_action.SetActive (true);
                captured_messages.Clear();
            }
        } else {
            captured_timer += Time.deltaTime;
        }

        if (twitch_banner_gui != null && twitch_banner_gui.activeSelf) {
            banner_timer += Time.deltaTime;

            if (banner_timer >= max_banner_time) {
                twitch_banner_gui.SetActive(false);
                twitch_action.SetActive (false);
            }

            banner_timer = 0.0f;
        }

        UpdateTwitchBanner();
    }

    private static void
    UpdateTwitchBanner() {
        // TODO(bill): Move banner related stuff to a controller
        // The following should probably be handled by which ever script
        // actuates commands
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
