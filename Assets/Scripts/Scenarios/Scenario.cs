﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Scenario
{
	protected int MINCOMMANDCOST = 0;

	protected WorldContainer the_world;
	protected ScenarioController master;
	protected DefaultScenario default_scenario;
	protected int clearance_level;
	protected string[] separator = { "_" };

	public abstract int EffectCommand(string twitch_desire);
	public abstract bool CheckTriggerConditions ();
	protected abstract void Reset ();

	public int GetClearanceLevel() {
		return clearance_level;
	}
	protected void Initialize(DefaultScenario ds, int cl) {
		default_scenario = ds;
		clearance_level = cl;
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
		master = GameObject.Find ("ScenarioController").GetComponent<ScenarioController> ();
	}
}