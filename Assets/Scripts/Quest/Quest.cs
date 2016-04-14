using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quest : MonoBehaviour {
    private int _id;
    private string _description;
    private Dictionary<string, int> _goals_tracker = new Dictionary<string, int>();
    private Dictionary<string, int> _goals = new Dictionary<string, int>();
    private Dictionary<string, int> _rewards = new Dictionary<string, int>();
    private int _next;

    private GameObject player;
    private InventoryController inventory_controller;

    public int id {
        get {return this._id;}
        set {this._id = value;}
    }

    public string description {
        get {return this._description;}
        set {this._description = value;}
    }

    public Dictionary<string, int> goals_tracker {
        get {return this._goals_tracker;}
        set {this._goals_tracker = value;}
    }

    public Dictionary<string, int> goals {
        get {return this._goals;}
        set {this._goals = value;}
    }

    public int next {
        get {return this._next;}
        set {this._next = value;}
    }

    public Dictionary<string, int> rewards {
        get {return this._rewards;}
        set {this._rewards = value;}
    }

    private void
    Awake() {
        player = GameObject.Find("Player");
        inventory_controller = GameObject.Find("InventoryContainer").GetComponent<InventoryController>();
    }

    public void
    Initialize() {
        foreach (string key in _goals_tracker.Keys) {
            string[] goal = key.Split('_');

            if (goal[0] == "kill") {
                WorldContainer world = GameObject.Find("WorldContainer").GetComponent<WorldContainer>();
                world.SetKillTracker(goal[1]);
            }
        }
    }

    public bool
    IsCompleted() {
        foreach (string key in _goals_tracker.Keys) {
            if (_goals[key] > _goals_tracker[key])
                return false;
        }

        Debug.Log("Completed");

        // Add reward item to inventory or ground
        foreach (string key in _rewards.Keys) {
            int amount = _rewards[key];

            for (int i = 0; i < amount; ++i) {
                GameObject item = (GameObject)Instantiate(Resources.Load(key), player.transform.position, Quaternion.identity);
                // Upon failure the item remains on the ground
                inventory_controller.AddNewObject(item);
            }
        }

        return true;
    }

    public void
    Reset() {
        List<string> keys = new List<string>(_goals_tracker.Keys);

        foreach (string key in keys)
            _goals_tracker[key] = 0;
    }

    public bool
    UpdateQuest() {
        // Check Inventory and Bounties
        WorldContainer world = GameObject.Find("WorldContainer").GetComponent<WorldContainer>();
        InventoryController inventory = GameObject.Find("PlayerUICurrent").GetComponentInChildren<InventoryController>();
        List<string> keys = new List<string>(_goals_tracker.Keys);

        foreach (string key in keys) {
            string[] goal = key.Split('_');
            _goals_tracker[key] = 0;

            foreach (GameObject item in inventory_controller.inventoryItems) {
                    if (goal[1] == item.name) {
                        _goals_tracker[key] += 1;
                    }
            }

            if (world.kills_tracker.bounties.ContainsKey(goal[1]))
                _goals_tracker[key] = world.kills_tracker.KillCount(goal[1]);
        }

        return IsCompleted();
    }
}
