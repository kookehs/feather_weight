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
		scnDict = new Dictionary<string, object>();
		DefaultScenario default_scenario = new DefaultScenario ();
		scnDict.Add ("DefaultScenario", default_scenario);
		scnDict.Add ("PlayerNearTree" , new PlayerNearTree(default_scenario));
	
		current_scenario = default_scenario;
		current_scenario_name = "DefaultScenario";
		twitch_desire = "";
	}

	// Update is called once per frame
	void Update () {
		if (!IsInScenario()) {
			foreach (var entry in scnDict) {
				object scenario = entry.Value;
				string scenario_name = entry.Key;
				if ((bool)InvokeScenarioMethod ("CheckTriggerConditions", scenario_name, scenario, null)) {
					current_scenario = scenario;
					current_scenario_name = scenario_name;
					// Debug.Log (currentScenario.name);
				}
			}
		}
		if (IsInScenario() && !twitch_desire.Equals("")) {
			// Debug.Log ("Invoking function");
			// Debug.Log (currentScenario.name);
			int termination = (int) InvokeScenarioMethod("EffectTwitchDesire", current_scenario_name, current_scenario, twitch_desire);
			if (termination == 1) {
				current_scenario_name = "DefaultScenario";
				GetScenario(current_scenario_name, out current_scenario);
			}
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
		return current_scenario != null;
	}

	public void UpdateTwitchCommand(string s) {
		twitch_desire = s;
	}

	private object InvokeScenarioMethod(string method_name, string scenario_name, object scenario, params object[] parameters) {
		// Debug.Log ("Calling " + method_name);
		object result = null;
		Type scenario_type = Type.GetType (scenario_name);
		if (scenario_type != null) {
			MethodInfo mi = scenario_type.GetMethod(method_name);
			if (mi != null) {
				result = mi.Invoke (scenario, parameters);
			}
		}
		return result;
	}
}
