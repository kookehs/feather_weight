﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class WaveController : MonoBehaviour {
    private static Text _time_limit;
    private static float _current_time = 0.0f;
    private static int _current_wave = 0;
    private static bool _goal_completed = false;
    private static bool _shop_phase = false;
    private static float _shop_trasition_time = 10.0f;
    private static float _shop_trasition_timer = 0.0f;
    private static float _max_shop_time = 30.0f;
    private static bool _wave_phase = true;
    private static int _tutorial_waves = 1;

	public static float bear_hp = 40f;
	public static float bear_spd = 2f;
	public static float wolf_hp = 40f;
	public static float wolf_spd = 2.5f;
	public static float hand_hp = 50f;

    public static InventoryController inventory;
    public AudioSource countdown;
    private static GameObject[] _spawners;

    public static int current_wave {
        get {return _current_wave;}
        set {_current_wave = value;}
    }

    public static bool goal_completed {
        get {return _goal_completed;}
        set {_goal_completed = value;}
    }

    public static bool shop_phase {
        get {return _shop_phase;}
        set {_shop_phase = value;}
    }

    public static float max_shop_time {
        get {return _max_shop_time;}
        set {_max_shop_time = value;}
    }

    public static float current_time {
        get {return _current_time;}
        set {_current_time = value;}
    }

    public static bool wave_phase {
        get {return _wave_phase;}
        set {_wave_phase = value;}
    }

    private void
    Awake() {
        inventory = GameObject.Find("InventoryContainer").GetComponent<InventoryController>();
        _time_limit = GameObject.Find("TimeLimit").GetComponent<Text>();
        InvokeRepeating("DisplayTime", 0.0f, 1.0f);
        _spawners = GameObject.FindGameObjectsWithTag("BearCave");

        // The following is debug code remove in release
        float delay = (_current_wave == 0) ? 3600.0f : 5.0f;
        float repeat = (_current_wave == 0) ? 3600.0f : 10.0f;

        foreach (GameObject spawner in _spawners) {
            spawner.GetComponent<CreatureSpawn>().UpdateSpawnFreq(delay, repeat);
        }

        if (_current_wave > _tutorial_waves - 1) {
            _current_time = WaveToSeconds(_current_wave);
        }

        ChickenSpawner.count = 0;
        TwitchController.AddToBannerQueue("Wave " + _current_wave);
        QuestController.current_quests.Clear();
        QuestController.AssignQuest(1);
        TwitchActionController.SetAPFillSpeed();
    }

    private void
    DisplayTime() {
        if (_current_wave < _tutorial_waves && Application.loadedLevelName.Contains("Chicken")) {
            _time_limit.color = new Color(1.0f, 1.0f, 1.0f);
            _time_limit.text = "--:--";
            return;
        }

        if (_current_time > 11) {
            _time_limit.color = new Color(1.0f, 1.0f, 1.0f);
        } else {
            _time_limit.color = new Color(1.0f, 0.0f, 0.0f);
        }

        if (_current_time == 11) {
            InvokeRepeating("Countdown", 1f, 1f);
        }

        if (_current_time == 1) {
            CancelInvoke("Countdown");
        }

        int minutes = (int)(_current_time / 60);
        int seconds = (int)(_current_time % 60);
        string pad = (seconds / 10 == 0) ? "0" : "";
        _time_limit.text = minutes.ToString() + ":" + pad + seconds.ToString();
    }

    private void
    Countdown() {
        countdown.Play();
    }

    private void
    NotEnoughChickens() {
        GameObject twitch_data = GameObject.FindGameObjectWithTag("TwitchData");

        if (twitch_data != null) {
            twitch_data.GetComponent<EnterCredits>().isGameOver = 2;
        }

        try {
            GameObject.Find("PlayerUICurrent").transform.FindChild("EventSystem").gameObject.SetActive(false);
        } catch (Exception e){
            Debug.Log("No EventSystem" + e.Message);
        }

        GameObject player = GameObject.Find("Player");

        if (player != null) {
            player.GetComponent<Health>().health = 0;
        }
    }

    private void
    OnLevelWasLoaded(int level) {
        string current_level = Application.loadedLevelName;

        if (current_level.Contains("Chicken")) {
            Vector3 point = GameObject.Find("SpawnPoint").transform.position;
            GameObject.Find("Player").transform.position = point;
            _spawners = GameObject.FindGameObjectsWithTag("BearCave");
            float delay = (_current_wave == 0) ? 3600.0f : 5.0f;
            float repeat = (_current_wave == 0) ? 3600.0f : 10.0f;

            foreach (GameObject spawner in _spawners) {
                spawner.GetComponent<CreatureSpawn>().UpdateSpawnFreq(delay, repeat);
            }

            ChickenSpawner.count = 0;
            TwitchController.used_names.Clear();

            if (_current_wave > _tutorial_waves - 1) {
                _current_time = WaveToSeconds(_current_wave);
            }

            TwitchController.AddToBannerQueue("Wave " + _current_wave);
            QuestController.current_quests.Clear();
            QuestController.AssignQuest(1);
            TwitchActionController.SetAPFillSpeed();
            TwitchController.SendVerbs("");
        } else if (current_level.Contains("Shop")) {
            TwitchController.SetupShop();
            _current_time = _max_shop_time;
            TwitchController.AddToBannerQueue("Shopping Phase");
            // TwitchController.SlowModeOn(60.0f);
            TwitchIRC.IRCPutMessage("During the duration of the shopping phase you may enter a number to vote");
        }
    }

    private static void
    ShopPhase() {
        try {
            GameObject.Find("PlayerUICurrent").transform.FindChild("EventSystem").gameObject.SetActive(false);
        } catch(Exception e) {
            Debug.Log("No EventSystem" + e.Message);
        }

        TwitchController.AddToBannerQueue("Moving to shop in 5 seconds");
    }

    private void
    Update() {
        if (_shop_phase == true && Application.loadedLevelName.Contains("Chicken")) {
            if (_shop_trasition_timer >= _shop_trasition_time) {
                // Remove chickens from inventory
                CheckInventory ci = new CheckInventory();
                ci.findAndRemoveChickens(inventory);

                try {
                    GameObject.Find ("PlayerUIElements").GetComponent<GrabPlayerUIElements> ().RestPlayerUI ();
                    inventory.moveGameObjectsParent ();
                    GameObject.Find ("PlayerUICurrent").transform.FindChild ("EventSystem").gameObject.SetActive (true);
                } catch (Exception e) {
                    Debug.Log ("No EventSystem" + e.Message);
                }

                Application.LoadLevel("ShopCenter");
                _shop_trasition_timer = 0.0f;
            } else {
                _shop_trasition_timer += Time.deltaTime;
            }
        }

        if (_current_wave < _tutorial_waves) {
            if (_wave_phase == true && _goal_completed == true) {
                _shop_phase = true;
                _wave_phase = false;
                ShopPhase();
            } else if (_shop_phase == true && Application.loadedLevelName.Contains("Shop")) {
                if (_current_time <= 0.0f) {
                    _shop_phase = false;
                    _wave_phase = true;
                    WavePhase();
                } else {
                    _current_time -= Time.deltaTime;
                }
            }
        } else {
            if (_current_time <= 0.0f) {
                if (_wave_phase == true) {
                    if (_goal_completed == false) {
                        NotEnoughChickens();
                        return;
                    }

                    ++_current_wave;
                    _shop_phase = true;
                    _wave_phase = false;
                    ShopPhase();
                } else if (_shop_phase == true) {
                    _shop_phase = false;
                    _wave_phase = true;
                    WavePhase();
                }
            } else {
                if (_current_time > 0.0f) {
                    _current_time -= Time.deltaTime;
                }
            }
        }
    }

    private static void
    WavePhase() {
        try {
            GameObject.Find("PlayerUIElements").GetComponent<GrabPlayerUIElements>().RestPlayerUI();
            inventory.moveGameObjectsParent ();
            GameObject.Find("PlayerUICurrent").transform.FindChild("EventSystem").gameObject.SetActive(true);
        } catch (Exception e) {
            Debug.Log("No EventSystem" + e.Message);
        }

        ++_current_wave;
        _goal_completed = false;
        Application.LoadLevel("HexLayoutChickenroom");
    }

    private static float
    WaveToSeconds(int wave) {
        return (float)Math.Pow(wave, 2.25 / 2) + 60;
    }
}
