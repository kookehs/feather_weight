using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNearTree : Scenario {

	public List<GameObject> adjTrees;

	// Use this for initialization
	public PlayerNearTree() {
		the_world = GameObject.Find ("WorldContainer");
		adjTrees = new List<GameObject> ();
	}

	public override void EffectTwitchDesire(string outcome){
		switch (outcome) {
                case "giveAcorn":
                        DropNuts ();
                        break;
		case "DropNuts":
			DropNuts ();
			break;
		default:
			break;
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

	private void DropNuts(){
		foreach (GameObject tree in adjTrees) {
			// Debug.Log ("Dropping Nuts");
			tree.GetComponent<Tree> ().DropNut ();
		}
	}
}
