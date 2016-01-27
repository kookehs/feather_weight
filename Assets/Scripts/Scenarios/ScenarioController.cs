using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


public class ScenarioController: MonoBehaviour{

	private Dictionary<string,object> scnDict; // dictionary of all Scenarios
	private object current_scenario;
	private string current_scenario_name;
	private string twitch_desire;

	// Use this for initialization
	void Start () {
		current_scenario = null;
		twitch_desire = "";
		scnDict = new Dictionary<string, object>();
		scnDict.Add ("PlayerNearTree" , new PlayerNearTree());
	}

	// Update is called once per frame
	void Update () {
		if (!IsInScenario()) {
			foreach (var entry in scnDict) {
				object container = entry.Value;
				if ((bool)InvokeScenarioMethod ("CheckTriggerConditions", null)) {
					current_scenario = container;
					current_scenario_name = entry.Key;
					// Debug.Log (currentScenario.name);
				}
			}
		}
		if (IsInScenario() && !twitch_desire.Equals("")) {
			// Debug.Log ("Invoking function");
			// Debug.Log (currentScenario.name);
			InvokeScenarioMethod("EffectTwitchDesire", twitch_desire);
			twitch_desire = "";
		}
	}

	public void GetScenario(string scenario_name, out object scenario) {
		// Debug.Log ("Obtaining Scenario");
		scnDict.TryGetValue(scenario_name, out scenario);
	}

	public string GetCurrentScenarioName() {
		return current_scenario_name;
	}

	public bool IsInScenario () {
		return !current_scenario.Equals(null);
	}

	public void UpdateTwitchCommand(string s) {
		twitch_desire = s;
	}

	private object InvokeScenarioMethod(string method_name, params object[] parameters) {
		// Debug.Log ("Calling " + method_name);
		object result = null;
		Type scenario_type = Type.GetType (current_scenario_name);
		if (scenario_type != null) {
			MethodInfo mi = scenario_type.GetMethod(method_name);
			if (mi != null) {
				result = mi.Invoke (current_scenario, parameters);
			}
		}
		return result;
	}
}
