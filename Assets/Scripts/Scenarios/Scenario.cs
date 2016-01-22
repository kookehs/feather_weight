using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Scenario
{
	public GameObject player;
	public GameObject main;
	protected bool isTriggered;

	public abstract void EffectOutcome(string outcome);
	public bool IsTriggered() {
		CheckTriggerConditions ();
		return isTriggered;
	}
	public void Untrigger() {
		isTriggered = false;
	}

	protected abstract void CheckTriggerConditions ();
}

public struct ScenarioContainer {
	public string name;
	public object scenario;
}