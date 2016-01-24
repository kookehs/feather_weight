using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


public class ScenarioController: MonoBehaviour{

	public GameObject player;
	public GameObject the_world;

	private Dictionary<string,ScenarioContainer> scnDict; // dictionary of all Scenarios
	private bool is_in_scenario;
	private ScenarioContainer currentScenario;
	private readonly ScenarioContainer nullScenario = new ScenarioContainer {name = null, scenario = null};
	private string outcome_command;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		the_world = GameObject.Find ("WorldContainer");
		currentScenario = nullScenario;
		is_in_scenario = false;
		outcome_command = "";
		scnDict = new Dictionary<string,ScenarioContainer>();
		scnDict.Add ("PlayerNearTree" , new ScenarioContainer {name = "PlayerNearTree", scenario = new PlayerNearTree()});
	}

	// Update is called once per frame
	void Update () {
		if (!is_in_scenario) {
			foreach (var entry in scnDict) {
				ScenarioContainer container = entry.Value;
				if ((bool)InvokeScenarioMethod ("IsTriggered", container, null)) {
					currentScenario = container;
					// Debug.Log (currentScenario.name);
					is_in_scenario = true;
				}
			}
		}
		if (is_in_scenario && !outcome_command.Equals("")) {
			// Debug.Log ("Invoking function");
			// Debug.Log (currentScenario.name);
			InvokeScenarioMethod("EffectOutcome", currentScenario, outcome_command);
			outcome_command = "";
		}
	}

	public void GetScenario(string scenario_name, out ScenarioContainer scenario) {
		// Debug.Log ("Obtaining Scenario");
		scnDict.TryGetValue(scenario_name, out scenario);
	}

	public string GetCurrentScenarioName() {
		string result = currentScenario.name;
		return result;
	}

	public void TriggerScenario(string scenario_name) {
		ScenarioContainer scenario;
		bool exist = scnDict.TryGetValue (scenario_name, out scenario);
		if (exist) currentScenario = scenario;
		}

	public bool IsInScenario () {
		return !currentScenario.Equals(nullScenario);
	}

	public void UpdateTwitchCommand(string s) {
		outcome_command = s;
	}

	private object InvokeScenarioMethod(string method_name, ScenarioContainer container, params object[] parameters) {
		// Debug.Log ("Calling " + method_name);
		object result = null;
		Type scenario_type = Type.GetType (container.name);
		if (scenario_type != null) {
			MethodInfo mi = scenario_type.GetMethod(method_name);
			if (mi != null) {
				result = mi.Invoke (container.scenario, parameters);
			}
		}
		return result;
	}
}
