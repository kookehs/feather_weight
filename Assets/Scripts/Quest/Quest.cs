using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quest {
    private int _id;
    private string _description;
    private Dictionary<string, int> _goal_tracker = new Dictionary<string, int>();
    private Dictionary<string, int> _goals = new Dictionary<string, int>();
    private Dictionary<string, int> _rewards = new Dictionary<string, int>();

    public int id {
        get {return this._id;}
        set {this._id = value;}
    }

    public string description {
        get {return this._description;}
        set {this._description = value;}
    }

    public Dictionary<string, int> goal_tracker {
        get {return this._goal_tracker;}
        set {this._goal_tracker = value;}
    }

    public Dictionary<string, int> goals {
        get {return this._goals;}
        set {this._goals = value;}
    }

    public Dictionary<string, int> rewards {
        get {return this._rewards;}
        set {this._rewards = value;}
    }

    public bool
    IsCompleted() {
        foreach (string key in _goal_tracker.Keys) {
            if (_goals[key] > _goal_tracker[key])
                return false;
        }

        Debug.Log("Completed");
        return true;
    }

    public bool
    UpdateQuest() {
        // Check Inventory
        InventoryController inventory = GameObject.Find("Inventory").GetComponent<InventoryController>();

        foreach (string key in _goal_tracker.Keys) {
            if (inventory.inventoryItems.ContainsKey(key)) {
                Debug.Log(inventory.inventoryItems[key].Count);
                _goal_tracker[key] = inventory.inventoryItems[key].Count;
            }
        }

        // Check Kills
        return IsCompleted();
    }
}
