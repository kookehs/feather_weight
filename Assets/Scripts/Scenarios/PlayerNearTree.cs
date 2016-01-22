using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNearTree : Scenario {

	public List<GameObject> adjTrees;

	// Use this for initialization
	public PlayerNearTree() {
		isTriggered = false;
		player = GameObject.Find ("Player");
		adjTrees = new List<GameObject> ();
	}

	public override void EffectOutcome(){
		string command = PollOutcome ();
		switch (command) {
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

	protected override void CheckTriggerConditions() {
		if (adjTrees.Count != 0) isTriggered = true;
	}

	protected override string PollOutcome() {
		//DO SOMETHING HERE
		return "DropNuts";
	}

	private void DropNuts(){
		foreach (GameObject tree in adjTrees) {
			Debug.Log ("Dropping Nuts");
			tree.GetComponent<Tree> ().DropNut ();
		}
	}
}