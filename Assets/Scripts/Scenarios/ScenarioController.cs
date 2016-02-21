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
	private int current_clearance_level;
	private List<string> twitch_desire;
        private string default_scenario_name = "DefaultScenario";

	// Use this for initialization
	void Start () {
		Debug.Log ("ScenarioController Start");
		scnDict = new Dictionary<string, object>();
		DefaultScenario default_scenario = new DefaultScenario ();
		scnDict.Add (default_scenario_name, default_scenario);
		scnDict.Add ("NearBearScenario", new NearBearScenario (default_scenario));

		current_scenario = default_scenario;
		current_scenario_name = default_scenario_name;
		current_clearance_level = 0;
		twitch_desire = new List<string>();
		Debug.Log (twitch_desire);
		//below are temporary test lines
		twitch_desire.Add ("SmiteTree");
	}

	// Update is called once per frame
	void FixedUpdate () {
		//Debug.Log (current_scenario_name);

			if (twitch_desire.Count == 0) {
			// Checking Trigger Conditions for each Scenario
			foreach (var entry in scnDict) {
				object scenario = entry.Value;
				string scenario_name = entry.Key;
				if ((bool)InvokeScenarioMethod ("CheckTriggerConditions", scenario_name, scenario, null)) {
					// If the Scenario's Trigger Conditions are fulfilled, get the clearance level
					int cl = (int) InvokeScenarioMethod ("GetClearanceLevel", scenario_name, scenario, null);
					// If the clearance level is higher than the current clearance level, then update current scenario, else do nothing
					if (cl > current_clearance_level) {
						current_scenario = scenario;
						current_scenario_name = scenario_name;
						current_clearance_level = cl;
					}
				}
			}
		}else {
			// Making sure that the Trigger Conditions still hold
			if ((bool)InvokeScenarioMethod("CheckTriggerConditions", current_scenario_name, current_scenario, null)) {
				string command = "";
				if (twitch_desire.Count != 0) {
					command = twitch_desire [0];
					twitch_desire.RemoveAt (0);
				}
				// Effecting the twitch desire
				if (!command.Equals ("")) {
					int termination = (int)InvokeScenarioMethod ("EffectTwitchDesire", current_scenario_name, current_scenario, command);
					// A termination of -1 means that the Scenario considers itself finished
					if (termination == -1) {
						current_scenario_name = default_scenario_name;
						current_clearance_level = 0;
						GetScenario (current_scenario_name, out current_scenario);
					}
				} else {
					current_scenario_name = default_scenario_name;
					current_clearance_level = 0;
					GetScenario (current_scenario_name, out current_scenario);
				}
			}
		}
	}

	private void GetScenario(string scenario_name, out object scenario) {
		// Debug.Log ("Obtaining Scenario");
		scnDict.TryGetValue(scenario_name, out scenario);
	}

	public string GetCurrentScenarioName() {
		return current_scenario_name;
	}

	public bool IsInScenario () {
		return true;
		//return !current_scenario_name.Equals(default_scenario_name);
	}

	public void UpdateTwitchCommand(string s) {
		twitch_desire.Add(s);
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
