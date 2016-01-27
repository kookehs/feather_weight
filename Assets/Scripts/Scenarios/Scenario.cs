using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Scenario
{
	protected GameObject the_world;

	public abstract void EffectTwitchDesire(string twitch_desire);
	public abstract bool CheckTriggerConditions ();
}