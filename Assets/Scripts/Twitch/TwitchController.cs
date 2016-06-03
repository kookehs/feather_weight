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
    public static float max_captured_time = 5.0f;
    private static Text displayed_messages;

    // public string twitch_influence_output = "Data/twitch_influence.txt";

    public static float influence_amount = 0.1f;
    private static float influence_timer = 0.0f;
    public static float max_influence_time = 60.0f;
    private static Dictionary<string, float> twitch_users = new Dictionary<string, float>();
    private static List<string> _used_names = new List<string>();

    private static string instructions;
    public static float max_slow_time = 30.0f;
    private static bool slow_on = false;
    private static float slow_timer = 0.0f;

    private static List<KeyValuePair<string, int>> _poll_results = new List<KeyValuePair<string, int>>();
    private static List<string> poll_users = new List<string>();

    private static bool _polled_shop = false;

    public static float max_poll_boss_time = 10.0f;
    public static bool poll_boss_choice = false;
    private static float poll_boss_timer = 0.0f;

    public static float max_save_time = 30.0f;
    private static float save_timer = 0.0f;

    private static GameObject twitch_banner_gui;
    public static float max_banner_time = 3.0f;
    private static float banner_timer = 0.0f;
    private static List<string> banner_queue = new List<string>();

	public AudioClip twitchBuys;

    public static List<KeyValuePair<string, int>> poll_results {
        get {return _poll_results;}
        set {_poll_results = value;}
    }

    public static List<string> used_names {
        get {return _used_names;}
        set {_used_names = value;}
    }

    public static bool polled_shop {
        get {return _polled_shop;}
        set {_polled_shop = value;}
    }

    public static void
    AddToBannerQueue(string message) {
        if (message == string.Empty) {
            return;
        }

        banner_queue.Add(message);
    }

    private static void
    AddUser(string user, float influence) {
        twitch_users.Add(user, influence);
    }

    private void
    Awake() {
        captured_timer = 0.0f;
        influence_timer = 0.0f;
        _polled_shop = false;
        save_timer = 0.0f;
        banner_timer = 0.0f;
        GameObject playerUICurrent = GameObject.Find ("PlayerUICurrent");

        if (GameObject.Find("TwitchContents") != null) {
            displayed_messages = GameObject.Find("TwitchContents").GetComponent<Text>();
        }

        if (playerUICurrent != null) {
            hud = playerUICurrent.transform.FindChild("ChatHUD").gameObject;
            irc = playerUICurrent.GetComponentInChildren<TwitchIRC>();
            twitch_banner_gui = playerUICurrent.transform.FindChild("TwitchActionPopUp").gameObject;
            twitch_banner_gui.SetActive(false);

            // This function will be called for every received message
            TwitchIRC.irc_message_received_event.AddListener(MessageListener);
        }

        // TODO(bill): Update instructions to refelect new design
        instructions = "Welcome to Feather Weight! Type statements to stop the nomad's progress! Ex. \"spawn bear h8\". Actions cost influence points. Collaboration between viewers is encouraged. To hide your chat prefix your statements with \"ooc\"";
        LoadUsers();
        banner_timer = max_banner_time;
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
        if (displayed_messages == null) {
            return;
        }

        // Prevent repeated answers and capture messages to send off to Python
        bool capture = (influence > 0) ? true : false;

        if (capture && CheckIfRepeated(user) == false && WaveController.shop_phase == false) {
                captured_messages.Add(new KeyValuePair<string, string>(user, influence + " " + message));
        }

        string offset = (capture == true) ? ": " : string.Empty;
        Color32 color = NameToColor(user);
        string hex_color = "#" + color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2");
        displayed_messages.text += "<color=" + hex_color + ">" + user + "</color>" + offset + message + "\n";
        hud.transform.FindChild("Scrollbar").GetComponent<Scrollbar>().value = 0.0f;
    }

    private static void
    LoadUsers() {
        TextAsset name_file = Resources.Load<TextAsset>("Twitch/Names") as TextAsset;

        if (name_file != null) {
            List<string> names = new List<string>(name_file.text.Split('\n'));

            foreach (string name in names) {
                if (twitch_users.ContainsKey(name) == false) {
                    twitch_users.Add(name, 0.1f);
                }
            }
        }

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
        // UnityEngine.Debug.Log(message);

        if (message.StartsWith("ping ")) {
            TwitchIRC.IRCPutCommand(message.Replace("ping", "PONG"));
        } else if (message.Split(' ')[1] == "001") {
            // 001 command is received after successful connection
            // Requests must come before joining a channel
            // This allows us to receive JOIN and PART
            TwitchIRC.valid_login = true;
            UnityEngine.Debug.Log("Successfully connected to Twitch");
            TwitchIRC.IRCPutCommand("CAP REQ :twitch.tv/membership");
            TwitchIRC.IRCPutCommand("JOIN #" + TwitchIRC.channel_name);
            SendInstructions();
            SendVerbs("");
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

            if (text.StartsWith("@featherweighttv")) {
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

                    if (space != -1) {
                        end = space - 1;
                    }
                }

                string other = string.Empty;

                if (start != -1 && end != -1) {
                    other = text.Substring(start + 1, end - start);
                    CreateMessage(user, 0, " and " + other + " are scheming against you!");
                } else {
                    CreateMessage(user, 0, " is scheming against you!");
                }

                return;
            } else if (text.StartsWith("!verb") || text.StartsWith("!verbs") || text.StartsWith("!cmd") || text.StartsWith("!command") || text.StartsWith("!commands")) {
                SendVerbs(user);
                return;
            }

            if (WaveController.shop_phase == true) {
                int voted = poll_users.IndexOf(user);
                int num = 0;

                if (voted == -1 && Int32.TryParse(text, out num)) {
                    if (num > 0 && num <= _poll_results.Count) {
                        poll_users.Add(user);
                        num -=1;
                        KeyValuePair<string, int> pair = _poll_results[num];
                        _poll_results[num] = new KeyValuePair<string, int>(pair.Key, pair.Value + 1);
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

    private static Color
    NameToColor(string name) {
        int length = name.Length;
        int magic_number = 0;
        UnityEngine.Random.seed = length + (int)name[0] + (int)name[length - 1] + magic_number;
        float r = UnityEngine.Random.Range(0.25f, 0.55f);
        float g = UnityEngine.Random.Range(0.25f, 0.55f);
        float b = UnityEngine.Random.Range(0.25f, 0.55f);
        Color color = new Color(r, g, b);
        return color;
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

    private void
    PollShopChoice() {
        if (WaveController.shop_phase == true && WaveController.current_time <= 1.0f && _polled_shop == false && Application.loadedLevelName.Contains("Shop")) {
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
           UnityEngine.Debug.Log(result);

           if (result == string.Empty) {
                AddToBannerQueue("Twitch didn't vote.");
           } else {
               AddToBannerQueue("Twitch voted for " + result + ".");
				GetComponent<AudioSource> ().PlayOneShot (twitchBuys);
           }

           TwitchActionController.Purchase(result);
           _polled_shop = true;
        }
    }

    public static string
    RandomUser() {
        int index = WorldContainer.RandomChance(twitch_users.Count);
        List<string> users = new List<string>(twitch_users.Keys);
        string user = users[0];
        users.RemoveAt(0);

        while (users.Count > 0 && _used_names.IndexOf(user) != -1) {
            user = users[0];
            users.RemoveAt(0);
        }

        _used_names.Add(user);
        return user;
    }

    public static void
    RemoveFromUsed(string name) {
        _used_names.Remove(name);
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
    SendVerbs(string user) {
        string verbs = string.Empty;

        bool comma = false;

        foreach (string verb in TwitchActionController.verbs) {
            if (comma == true) {
                verbs += ", ";
            }

            verbs += verb;
            comma = true;
        }

        if (user == string.Empty) {
            TwitchIRC.IRCPutMessage("Here's a list of available Twitch commands!");
            TwitchIRC.IRCPutMessage(verbs);
        } else {
            TwitchIRC.WhisperPutMessage(user, verbs);
        }
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
                    if (twitch_users.ContainsKey(pair.Key)) {
                        messages.Add(twitch_users[pair.Key] + " " + pair.Value);
                    }
                }

                StringReader.ReadStrings(messages);
                TwitchActionController.Do (StringReader.command, StringReader.effect, StringReader.hex);
                captured_messages.Clear();
            }
        } else {
            captured_timer += Time.deltaTime;
        }

        if (banner_timer >= max_banner_time && twitch_banner_gui != null) {
            twitch_banner_gui.SetActive(false);
            banner_timer = 0.0f;

            if (banner_queue.Count > 0) {
                string banner = banner_queue[0];
                banner_queue.RemoveAt(0);
                twitch_banner_gui.SetActive(true);
                twitch_banner_gui.GetComponentInChildren<Text>().text = banner;
            }
        } else {
            banner_timer += Time.deltaTime;
        }
    }
}
