using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Scenario
{
	protected int MINCOMMANDCOST = 0;

	protected static WorldContainer the_world;
	protected static ScenarioController master;
	protected static DefaultScenario default_scenario;
	protected static string[] separator = { "_" };

	protected int clearance_level;

	public abstract int EffectCommand(string twitch_desire);
	public abstract bool CheckTriggerConditions ();
	protected abstract void Reset ();

	public int GetClearanceLevel() {
		return clearance_level;
	}
	protected void Initialize(DefaultScenario ds, int cl) {
		if (default_scenario == null) default_scenario = ds;
		clearance_level = cl;
		if (the_world == null) the_world = GameObject.Find("WorldContainer").GetComponent<WorldContainer> ();
		if (master == null) master = GameObject.Find("WorldContainer").GetComponent<ScenarioController> ();
	}
}