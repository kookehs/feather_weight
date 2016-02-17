using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quest {
    private int _id;
    private string _description;
    private Dictionary<string, int> _goals_tracker = new Dictionary<string, int>();
    private Dictionary<string, int> _goals = new Dictionary<string, int>();
    private Dictionary<string, int> _rewards = new Dictionary<string, int>();
    private int _next;

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
        InventoryController inventory = GameObject.Find("Inventory").GetComponent<InventoryController>();
        List<string> keys = new List<string>(_goals_tracker.Keys);

        foreach (string key in keys) {
            string[] goal = key.Split('_');

            if (inventory.inventoryItems.ContainsKey(goal[1]))
                _goals_tracker[key] = inventory.inventoryItems[goal[1]].Count;

            if (world.kills_tracker.bounties.ContainsKey(goal[1]))
                _goals_tracker[key] = world.kills_tracker.KillCount(goal[1]);
        }

        return IsCompleted();
    }
}
