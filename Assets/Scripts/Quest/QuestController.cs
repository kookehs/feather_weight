using LitJson;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class QuestController : MonoBehaviour {
    private bool landmark_discovered = false;
    private List<Quest> _quests = new List<Quest>();

    private Quest _current_quest;
    private bool _current_quest_completed = false;

    public List<Quest> quests {
        get {return this._quests;}
        set {this._quests = value;}
    }

    public Quest current_quest {
        get {return this._current_quest;}
        set {this._current_quest = value;}
    }

    public bool current_quest_completed {
        get {return this._current_quest_completed;}
        set {this._current_quest_completed = value;}
    }

    public void
    AssignQuest(int index) {
        if (index != -1 && index < _quests.Count) {
            current_quest = _quests[index];
        } else {
            Random.seed = System.Environment.TickCount;
            // TODO(bill): Handle random generation better
            // Float is being converted to int, the chance for _quests.count - 1 is extremely low
            current_quest = _quests[Random.Range(0, _quests.Count - 1)];
        }
    }

    private void
    Awake() {
        GameObject.Find("QuestHUD").GetComponent<CanvasGroup>().alpha = 0.0f;
        string path = "C:/Users/Bill/Documents/nomad/Assets/Scripts/Quest/Quests.json";
        LoadJsonFile(path);
    }

    private void
    LoadJsonFile(string path) {
        string json_data = string.Empty;

        using (StreamReader stream = new StreamReader(path)) {
           json_data = stream.ReadToEnd();
        }

        JsonData quest_structure = JsonMapper.ToObject(json_data);

        for (int i = 0; i < quest_structure["quests"].Count; ++i) {
            Quest quest = new Quest();
            quest.id = (int)quest_structure["quests"][i]["id"];
            quest.description = (string)quest_structure["quests"][i]["description"];

            foreach (string key in quest_structure["quests"][i]["goals"].Keys) {
                quest.goals.Add(key, (int)quest_structure["quests"][i]["goals"][key]);
                quest.goals_tracker.Add(key, 0);
            }

            foreach (string key in quest_structure["quests"][i]["rewards"].Keys) {
                quest.rewards.Add(key, (int)quest_structure["quests"][i]["rewards"][key]);
            }

            _quests.Add(quest);
        }
    }

    private void
    OnTriggerEnter(Collider other) {
        if (landmark_discovered == false)
            AssignQuest(-1);

        landmark_discovered = true;
    }

    private void
    PrintCurrentQuest() {
        string result = string.Empty;
        result += "id: " + current_quest.id + "\n";
        result += "description: " + current_quest.description + "\n";
        result += "goals: ";

        foreach (string key in current_quest.goals.Keys) {
            result += key + ": " + current_quest.goals[key] + ", ";
        }

        result += "\nrewards: ";

        foreach (string key in current_quest.rewards.Keys) {
            result += key + ": " + current_quest.rewards[key] + ", ";
        }

        Debug.Log(result);
    }

    private void
    PrintQuests() {
        foreach (Quest q in _quests) {
            string result = string.Empty;
            result += "id: " + q.id + "\n";
            result += "description: " + q.description + "\n";
            result += "goals: ";

            foreach (string key in q.goals.Keys) {
                result += key + ": " + q.goals[key] + ", ";
            }

            result += "\nrewards: ";

            foreach (string key in q.rewards.Keys) {
                result += key + ": " + q.rewards[key] + ", ";
            }

            Debug.Log(result);
        }
    }

    private void
    Update() {
        if (landmark_discovered == true) {
            _current_quest.UpdateQuest();
            GameObject.Find("QuestHUD").GetComponent<CanvasGroup>().alpha = 1.0f;
            GameObject quest_info = GameObject.Find("QuestInfo");
            Text quest_text = quest_info.GetComponent<Text>();
            quest_text.text = _current_quest.description + "\n";

            foreach (string key in _current_quest.goals.Keys) {
                quest_text.text += key + ": " + _current_quest.goals_tracker[key] + "/" + _current_quest.goals[key] + "\n";
            }
        }
    }

}