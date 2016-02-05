using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Scenario
{
	protected WorldContainer the_world;
	protected DefaultScenario default_scenario;

	public abstract int EffectTwitchDesire(string twitch_desire);
	public abstract bool CheckTriggerConditions ();
	protected void InitializeWorldContainer() {
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer>();
	}
}