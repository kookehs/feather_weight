using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNearTree : Scenario {

	public List<GameObject> adjTrees;

	// Use this for initialization
	public PlayerNearTree(DefaultScenario ds) {
		the_world = GameObject.Find ("WorldContainer");
		default_scenario = ds;
		adjTrees = new List<GameObject> ();
	}

	public override int EffectTwitchDesire(string outcome){
		switch (outcome) {
        case "giveAcorn":
            return DropNuts ();
		case "DropNuts":
			return DropNuts ();
		default:
			return default_scenario.EffectTwitchDesire (outcome);
		}
	}

	public void addTree(GameObject tree) {
		adjTrees.Add (tree);
	}

	public void removeTree(GameObject tree) {
		adjTrees.Remove (tree);
	}

	public override bool CheckTriggerConditions() {
		if (adjTrees.Count != 0) return true;
		return false;
	}

	private int DropNuts(){
		foreach (GameObject tree in adjTrees) {
			// Debug.Log ("Dropping Nuts");
			tree.GetComponent<Tree> ().DropNut ();
		}
		return 0;
	}
}
