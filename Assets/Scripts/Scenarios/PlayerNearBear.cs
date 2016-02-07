using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNearBear : Scenario {

	public List<GameObject> adjBears;

	// Use this for initialization
	public PlayerNearBear(DefaultScenario ds) {
		the_world = GameObject.Find ("WorldContainer");
		default_scenario = ds;
		adjBears = new List<GameObject> ();
	}

	public override int EffectTwitchDesire(string outcome){
		switch (outcome) {
		case "increaseHostility":
			return increaseHostility ();
		case "decreaseHostility":
			return decreaseHostilty ();
		case "guardTree":
			return guardTree ();
		default:
			return default_scenario.EffectTwitchDesire (outcome);
		}
	}

	public void addBear(GameObject bear) {
		adjBears.Add (bear);
	}

	public void removeBear(GameObject bear) {
		adjBears.Remove (bear);
	}

	public override bool CheckTriggerConditions() {
		if (adjBears.Count != 0) return true;
		return false;
	}

	private void increaseHostility(){
		foreach (GameObject bear in adjBears) {
			bear.GetComponent<Bear> ().decreaseFriendliness();
		}
	}

	private void decreaseHostilty(){
		foreach (GameObject bear in adjBears) {
			bear.GetComponent<Bear> ().increaseFriendliness ();
		}
	}

	private void guardTree(){
		GameObject t = the_world.GetComponent<WorldContainer> ().PlayerNearTreeCheck;
		foreach (GameObject bear in adjBears) {
			bear.GetComponent<Bear> ().setGuard (t);
		}
	}
		
}
