using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


public class ScenarioController: MonoBehaviour
{
	public Slider influence_meter;

	private Dictionary<string,object> scenarios;              // dictionary of all Scenarios
	private string current_scenario_name;                     // the name of the current scenario
	private int current_clearance_level;                      // the clearance level of the current scenario
	private List<string> twitch_command;                      // the list of all twitch commands
	private string default_scenario_name = "DefaultScenario"; // the name of the default scenario

	public readonly float MAX_GI = 1000f;
	private readonly float MAX_GIPS = 10f;
	public float curr_GI;
	private float gips = 3.5f;        // GI per second
	private float gipf;               // GI per frame
	private float gipspgis = 0.0025f; // GIPS per GI spent

	private GameObject player;
	// Use this for initialization
	void Start ()
	{
		scenarios = new Dictionary<string, object> ();
		DefaultScenario default_scenario = new DefaultScenario ();
		scenarios.Add (default_scenario_name, default_scenario);
		scenarios.Add ("NearBearScenario", new NearBearScenario (default_scenario));
		scenarios.Add ("NearRabbitScenario", new NearRabbitScenario (default_scenario));

		current_scenario_name = default_scenario_name;
		current_clearance_level = 0;
		twitch_command = new List<string> ();

		curr_GI = 0f;
		gipf = gips * Time.deltaTime;

		player = GameObject.Find ("Player");
		//below are temporary test lines
		//InvokeRepeating ("DebugEverySecond", 0, 1f);
	}

	void Update() {
		if (player == null) return;

		GIRegen ();

		if (twitch_command.Count > 0) {
			// Making sure that the Trigger Conditions still hold
			if ((bool)InvokeScenarioMethod ("CheckTriggerConditions", current_scenario_name, null)) {
				string command = "";
				if (twitch_command.Count != 0) {
					command = twitch_command [0];
					twitch_command.RemoveAt (0);
				}
				// Effecting the twitch desire
				if (!command.Equals ("")) {
					int influence_cost = (int)InvokeScenarioMethod ("EffectCommand", current_scenario_name, command);
					// A termination of -1 means that the Scenario considers itself finished
					ExpendGI (influence_cost);
				}
			}else {
				current_scenario_name = default_scenario_name;
				current_clearance_level = 0;
			}
		}
		if (twitch_command.Count == 0) {
			// Checking Trigger Conditions for each Scenario
			CheckTriggerConditions();
		}
	}

	public string GetCurrentScenarioName ()
	{
		return current_scenario_name;
	}

	public void UpdateTwitchCommand (string s)
	{
		twitch_command.Add (s);
	}

	public float GetCurrentGI () {
		return curr_GI;
	}

	private void CheckTriggerConditions() {
		foreach (var i in scenarios) {
			string scenario_name = i.Key;
			if ((bool)InvokeScenarioMethod ("CheckTriggerConditions", scenario_name, null)) {
				// If the Scenario's Trigger Conditions are fulfilled, get the clearance level
				int cl = (int)InvokeScenarioMethod ("GetClearanceLevel", scenario_name, null);
				// If the clearance level is higher than the current clearance level, then update current scenario, else do nothing
				if (cl > current_clearance_level) {
					current_scenario_name = scenario_name;
					current_clearance_level = cl;
				}
			}
		}
	}

	private object InvokeScenarioMethod (string method_name, string scenario_name, params object[] parameters)
	{
		// Debug.Log ("Calling " + method_name);
		object result = null;
		Type scenario_type = Type.GetType (scenario_name);
		if (scenario_type != null) {
			MethodInfo mi = scenario_type.GetMethod (method_name);
			if (mi != null) {
				result = mi.Invoke (scenarios[scenario_name], parameters);
			}
		}
		return result;
	}

	private void GIRegen() {
		curr_GI = (curr_GI + gipf > MAX_GI) ? MAX_GI: curr_GI + gipf;
		influence_meter.value = curr_GI;
	}

	private void ExpendGI(int cost) {
		curr_GI -= cost;
		influence_meter.value = curr_GI;
		float d_gips = gips + gipspgis * cost;
		gips = (d_gips > MAX_GIPS) ? MAX_GIPS : d_gips;
	}

	private void DebugEverySecond() {
		Debug.Log (gips);
	}

}
