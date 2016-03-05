using LitJson;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class QuestController : MonoBehaviour {
	private bool landmark_discovered = true; // SET THIS TO FALSE LATER
    private List<Quest> _quests = new List<Quest>();

	private List<Quest> _current_quests = new List<Quest>();
    private bool _current_quest_completed = false;

    public List<Quest> quests {
        get {return this._quests;}
        set {this._quests = value;}
    }

    public List<Quest> current_quests {
        get {return this._current_quests;}
        set {this._current_quests = value;}
    }

    public bool current_quest_completed {
        get {return this._current_quest_completed;}
        set {this._current_quest_completed = value;}
    }

    public void
    AssignQuest(int index) {
        if (index != -1 && index < _quests.Count) {
			_current_quests.Add(_quests[index]);
			_current_quests[LastQuestIndex()].Initialize();
        } else {
            //Random.seed = System.Environment.TickCount;
            // TODO(bill): Handle random generation better
            // Float is being converted to int, the chance for _quests.count - 1 is extremely low
            //_current_quest = _quests[Random.Range(0, _quests.Count - 1)];
        }
    }

	public void 
	AssignQuest(int[] indeces) {
		foreach (int i in indeces) AssignQuest (i);
	}

    private void
    Awake() {
        GameObject.Find("QuestHUD").GetComponent<CanvasGroup>().alpha = 0.0f;
        string path = Application.dataPath + "/Scripts/Quest/Quests.json";
        LoadJsonFile(path);
		AssignQuest (new int[]{1,2,3});
    }
	
	private int
	LastQuestIndex() {
		return _current_quests.Count - 1;			
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

            quest.next = (int)quest_structure["quests"][i]["next"];

            _quests.Add(quest);
        }
    }

    private void
    OnTriggerEnter(Collider other) {
        if (other.gameObject.name == "Player") {
            if (landmark_discovered == false)
				AssignQuest(new int[] {1,2,3});

            landmark_discovered = true;
        }
    }

    private void
    PrintCurrentQuests() {
		PrintQuests (_current_quests);
    }

    private void
	PrintQuests(List<Quest> quests) {
        foreach (Quest q in quests) {
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
			Text quest_text = GameObject.Find ("QuestInfo").GetComponent<Text>();
			quest_text.text = "None";
			foreach (Quest q in _current_quests) {
				bool completed = q.UpdateQuest ();
				GameObject.Find ("QuestHUD").GetComponent<CanvasGroup> ().alpha = 1.0f;
				GameObject quest_info = GameObject.Find ("QuestInfo");
				if (quest_text.text.Equals ("None"))
					quest_text.text = q.description + "\n";
				else quest_text.text += q.description + "\n";

				foreach (string key in q.goals.Keys) {
					string[] goal = key.Split ('_');
					quest_text.text += goal [1] + ": " + q.goals_tracker [key] + "/" + q.goals [key] + "\n";
				}

				/*if (completed == true) {
					Debug.Log ("Next quest");
					q.Reset ();
					AssignQuest (_current_quest.next);
				}*/
			}
		}
    }
}
