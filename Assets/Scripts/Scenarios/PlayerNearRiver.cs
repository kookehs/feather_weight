using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNearRiver : Scenario {

	public List<GameObject> adjRiverPoint;

	// Use this for initialization
	public PlayerNearRiver(DefaultScenario ds) {
		Initialize (ds, 0);
		adjRiverPoint = new List<GameObject> ();
	}

	public override int EffectCommand(string outcome){
		switch (outcome) {
		case "RiverStuff":
			return 0;
		default:
			return default_scenario.EffectCommand (outcome);
		}
	}

	public override bool CheckTriggerConditions() {
		if (adjRiverPoint.Count != 0) return true;
		return false;
	}

	protected override void Reset() {
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
