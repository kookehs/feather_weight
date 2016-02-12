using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Scenario
{
	protected WorldContainer the_world;
	protected DefaultScenario default_scenario;
	protected int clearance_level;
	protected string[] separator = { "_" };

	public abstract int EffectTwitchDesire(string twitch_desire);
	public abstract bool CheckTriggerConditions ();
	protected abstract void Reset ();

	public int GetClearanceLevel() {
		return clearance_level;
	}
	protected void InitializeWorldContainer() {
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
	}
}