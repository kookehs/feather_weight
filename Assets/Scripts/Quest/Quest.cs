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
                WorldContainer.SetKillTracker(goal[1]);
            } else if (goal[0] == "collect") {
                WorldContainer.SetCountTracker(goal[1]);
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
                GameObject player = GameObject.Find("Player");
                GameObject item = (GameObject)Instantiate(Resources.Load(key), player.transform.position, Quaternion.identity);
                // Upon failure the item remains on the ground
                InventoryController inventory_controller = GameObject.Find("InventoryContainer").GetComponent<InventoryController>();
                inventory_controller.AddNewObject(item);
                TwitchController.AddToBannerQueue("Quest completed");
                Instantiate(Resources.Load("EcstaticSparks"), player.transform.position, Quaternion.identity);
                Instantiate(Resources.Load("AwesomeSparks"), player.transform.position, Quaternion.identity);
                Instantiate(Resources.Load("AmazingSparks"), player.transform.position, Quaternion.identity);
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
        InventoryController inventory = GameObject.Find("PlayerUICurrent").GetComponentInChildren<InventoryController>();
        List<string> keys = new List<string>(_goals_tracker.Keys);

        foreach (string key in keys) {
            string[] goal = key.Split('_');

            if(WorldContainer.counts_tracker.counts.ContainsKey(goal[1])) {
                _goals_tracker[key] = WorldContainer.counts_tracker.CountCount(goal[1]);
            }

            if (WorldContainer.kills_tracker.bounties.ContainsKey(goal[1]))
                _goals_tracker[key] = WorldContainer.kills_tracker.KillCount(goal[1]);
        }

        return IsCompleted();
    }
}
