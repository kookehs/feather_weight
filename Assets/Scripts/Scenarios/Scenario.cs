using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Scenario
{
	protected WorldContainer the_world;
	protected DefaultScenario default_scenario;
	protected string[] separator = {"_"};

	public abstract int EffectTwitchDesire(string command);
	public abstract bool CheckTriggerConditions ();
	protected void InitializeWorldContainer() {
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer>();
	}
}