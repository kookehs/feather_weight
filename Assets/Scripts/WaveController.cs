﻿using UnityEngine;
using UnityEngine.UI;
using System;

public class WaveController : MonoBehaviour {
    private static Text _time_limit;
    private static float _current_time = 0.0f;
    private static int _current_wave = 0;
    private static bool _shop_phase = false;
    private static float _max_shop_time = 30.0f;
    private static bool _wave_phase = true;

    public static InventoryController inventory;
    private static GameObject[] _spawners;

    public static int current_wave {
        get {return _current_wave;}
        set {_current_wave = value;}
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
        _current_time = WaveToSeconds(_current_wave);
        TwitchController.AddToBannerQueue("Wave " + _current_wave);
        InvokeRepeating("DisplayTime", 0.0f, 1.0f);
        _spawners = GameObject.FindGameObjectsWithTag("BearCave");

        foreach (GameObject spawner in _spawners) {
           spawner.GetComponent<CreatureSpawn>().UpdateSpawnFreq(3600.0f);
        }

        QuestController.AssignQuest(1);
    }

    private void
    DisplayTime() {
        int minutes = (int)(_current_time / 60);
        int seconds = (int)(_current_time % 60);
        string pad = (seconds / 10 == 0) ? "0" : "";
        _time_limit.text = minutes.ToString() + ":" + pad + seconds.ToString();
        // Debug.Log(_time_limit.text);
    }

    void OnLevelWasLoaded (int level) {
        string current_level = Application.loadedLevelName;

        if (current_level.Contains("Hub")) {
            Vector3 point = GameObject.Find("SpawnPoint").transform.position;
            GameObject.Find("Player").transform.position = point;
        } else if (current_level.Contains("Shop")) {
            TwitchController.SetupShop();
        }
    }

    private static void
    ShopPhase() {
        // Remove chickens from inventory
        CheckInventory ci = new CheckInventory();
        ci.findAndRemoveChickens(inventory);

		try{
			GameObject.Find ("PlayerUICurrent").transform.FindChild("EventSystem").gameObject.SetActive(false);
		}catch(Exception e){
			Debug.Log ("No EventSystem" + e.Message);
		}

        Application.LoadLevel("ShopCenter");
        _current_time = _max_shop_time;
        TwitchController.AddToBannerQueue("Shopping Phase");
        //TwitchController.SlowModeOn(60.0f);
        TwitchIRC.IRCPutMessage("During the duration of the shopping phase you may enter a number to vote");
    }

    private void
    Update() {
        if (_current_time <= 0.0f) {
            if (_wave_phase == true) {
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
            _current_time -= Time.deltaTime;
        }
    }

    private static void
    WavePhase() {
		GameObject.Find ("PlayerUIElements").GetComponent<GrabPlayerUIElements> ().RestPlayerUI ();
		//inventory.moveGameObjectsParent ();
		try{
			GameObject.Find ("PlayerUICurrent").transform.FindChild("EventSystem").gameObject.SetActive(true);
		}catch(Exception e){
			Debug.Log ("No EventSystem" + e.Message);
		}

		_current_time = WaveToSeconds(_current_wave);
		TwitchController.AddToBannerQueue("Wave " + _current_wave);
		_spawners = GameObject.FindGameObjectsWithTag("BearCave");
		foreach (GameObject spawner in _spawners) {
			spawner.GetComponent<CreatureSpawn>().UpdateSpawnFreq(10.0f);
		}

		QuestController.AssignQuest(1);

        Application.LoadLevel("HexLayoutChickenroom");
    }

    private static float
    WaveToSeconds(int wave) {
        return (float)Math.Pow(wave, 2.25 / 2) + 60;
    }

}
