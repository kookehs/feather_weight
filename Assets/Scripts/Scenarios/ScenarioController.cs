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
	public Slider GI_meter;
	public Image GI_fill;

	private Dictionary<string,object> scenarios;              // dictionary of all Scenarios
	private string current_scenario_name;                     // the name of the current scenario
	private int current_clearance_level;                      // the clearance level of the current scenario
	public List<string> twitch_command;                      // the list of all twitch commands
	private string default_scenario_name = "DefaultScenario"; // the name of the default scenario

	public readonly float MAX_GI = 1000f;
	private readonly float MAX_GIPS = 10f;
	public float curr_GI;
	private float gips = 3.5f;        // GI per second
	private float gipf;               // GI per frame
	private float gipspgis = 0.0025f; // GIPS per GI spent
	public bool GI_expended = false;
	public float flash_speed = 2f;
	public Color flash_color = new Color(0.392f, 0.555f, 0.647f, 1f);
	private Color _GI_fill_color = new Color(0.392f, 0.255f, 0.647f, 1f);

	private GameObject player;
	// Use this for initialization
	void Start ()
	{
		scenarios = new Dictionary<string, object> ();
		DefaultScenario default_scenario = new DefaultScenario ();
		scenarios.Add (default_scenario_name, default_scenario);
		scenarios.Add ("NearBearScenario", new NearBearScenario (default_scenario));
		scenarios.Add ("NearRabbitScenario", new NearRabbitScenario (default_scenario));
		//scenarios.Add ("BossFightScenario", new BossFightScenario (default_scenario));

		current_scenario_name = default_scenario_name;
		current_clearance_level = 0;
		twitch_command = new List<string> ();

		curr_GI = 500.0f;
		gipf = gips * Time.deltaTime;

		player = GameObject.Find ("Player");
		InvokeRepeating ("DisplayGI", 0, 0.5f);

		//below are temporary test lines
		twitch_command.Add("growTree");
		//InvokeRepeating ("DebugEverySecond", 0, 1f);
                GI_meter = GameObject.Find("InfluenceHUD").GetComponent<Slider>();
                GI_fill = GameObject.Find("InfluenceFill").GetComponent<Image>();
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
					if (influence_cost > 0) {
						ExpendGI (influence_cost);
						GI_expended = true;
					}
				}
			}else CheckTriggerConditions();
		}
		if (twitch_command.Count == 0) {
			// Checking Trigger Conditions for each Scenario
			CheckTriggerConditions();
		}
		if (GI_expended) GI_fill.color = flash_color;
		else GI_fill.color = Color.Lerp (GI_fill.color, _GI_fill_color, flash_speed * Time.deltaTime);
		GI_expended = false;
	}

	void OnApplicationQuit() {
		CancelInvoke ();
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

	private void DisplayGI () {
		GameObject GI_gui = GameObject.Find ("Influence");
		if (!GI_gui) return;
		Text influence = GI_gui.GetComponent<Text>();
		influence.text = curr_GI.ToString("F0");
	}

	private void GIRegen() {
		curr_GI = Mathf.Min (MAX_GI, curr_GI + gipf);
		GI_meter.value = curr_GI;
	}

	private void ExpendGI(int cost) {
		curr_GI -= cost;
		GI_meter.value = curr_GI;
		gips = Mathf.Min (MAX_GIPS, gips + gipspgis * cost);
	}

	private void DebugEverySecond() {
		Debug.Log (gips);
	}

}
