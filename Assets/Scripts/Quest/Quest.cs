using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quest {
    private int _id;
    private string _description;
    private Dictionary<string, int> _goals_tracker = new Dictionary<string, int>();
    private Dictionary<string, int> _goals = new Dictionary<string, int>();
    private float _last_kill_time = 0.0f;
    private Dictionary<string, int> _rewards = new Dictionary<string, int>();

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

    public float last_kill_time {
        get {return this._last_kill_time;}
        set {this._last_kill_time = value;}
    }

    public Dictionary<string, int> rewards {
        get {return this._rewards;}
        set {this._rewards = value;}
    }

    public bool
    IsCompleted() {
        foreach (string key in _goals_tracker.Keys) {
            if (_goals[key] > _goals_tracker[key])
                return false;
        }

        Debug.Log("Completed");
        return true;
    }

    public bool
    UpdateQuest() {
        // Check Inventory and Bounties
        WorldContainer world = GameObject.Find("WorldContainer").GetComponent<WorldContainer>();
        InventoryController inventory = GameObject.Find("Inventory").GetComponent<InventoryController>();
        List<string> keys = new List<string>(_goals_tracker.Keys);

        foreach (string key in keys) {
            if (inventory.inventoryItems.ContainsKey(key))
                _goals_tracker[key] = inventory.inventoryItems[key].Count;

            if (world.kills_tracker.bounties.ContainsKey(key))
                _goals_tracker[key] = world.kills_tracker.KillCount(key);
        }

        return IsCompleted();
    }
}
