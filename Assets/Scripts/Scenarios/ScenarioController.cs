using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;


public class ScenarioController: MonoBehaviour{

	private Dictionary<string,ScenarioContainer> scnDict; // dictionary of all Scenarios
	private ScenarioContainer currentScenario;
	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		scnDict = new Dictionary<string,ScenarioContainer>();
		scnDict.Add ("PlayerNearTree" , new ScenarioContainer {name = "PlayerNearTree", scenario = new PlayerNearTree()});
	}
	
	// Update is called once per frame
	void Update () {
		foreach (var entry in scnDict) {
			ScenarioContainer container = entry.Value;
			if ((bool)InvokeScenarioMethod("IsTriggered", container))
				InvokeScenarioMethod("EffectOutcome",container);
		}
	}

	public ScenarioContainer GetScenario(string scenario_name) {
		Debug.Log ("Obtaining Scenario");
		ScenarioContainer scenario = new ScenarioContainer {name = null, scenario = null};
		scnDict.TryGetValue(scenario_name, out scenario); 
		return scenario;
	}

	public void TriggerScenario(string scenario_name) {
		ScenarioContainer scenario;
		bool exist = scnDict.TryGetValue (scenario_name, out scenario);
		if (exist) currentScenario = scenario;
	}

	private object InvokeScenarioMethod(string method_name, ScenarioContainer container) {
		object result = null;
		Type scenario_type = Type.GetType (container.name);
		if (scenario_type != null) {
			MethodInfo mi = scenario_type.GetMethod(method_name);
			if (mi != null) {
				result = mi.Invoke (container.scenario, null);
			}
		}
		return result;
	}
}
