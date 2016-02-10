using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNearRiver : Scenario {

	public List<GameObject> adjRiverPoint;

	// Use this for initialization
	public PlayerNearRiver(DefaultScenario ds) {
		the_world = GameObject.Find ("WorldContainer");
		default_scenario = ds;
		adjRiverPoint = new List<GameObject> ();
	}

	public override int EffectTwitchDesire(string outcome){
		switch (outcome) {
		case "RiverStuff":
			return 0;
		default:
			return default_scenario.EffectTwitchDesire (outcome);
		}
	}

	public override bool CheckTriggerConditions() {
		if (adjRiverPoint.Count != 0) return true;
		return false;
	}

	public void addRiverPoint(GameObject riverPoint) {
		adjRiverPoint.Add (riverPoint);
	}

	public void removeRiverPoint(GameObject riverPoint) {
		adjRiverPoint.Remove (riverPoint);
	}

	public int RiverStuff(GameObject riverPoint){
		//shoot out a fish
		//spawn bear to come eat
		return 0;
	}
}
