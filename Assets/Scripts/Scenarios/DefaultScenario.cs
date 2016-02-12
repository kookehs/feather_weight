using UnityEngine;

public class DefaultScenario: Scenario {

	public DefaultScenario () {
		InitializeWorldContainer ();
		clearance_level = 0;
	}

	public override bool CheckTriggerConditions() {
		return true;
	}

	public override int EffectTwitchDesire(string input) {
		string[] parameters = input.Split (separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (parameters[0]) {
		case "growTree":
			Debug.Log ("Growing Nut");
			return TryToGrowNut();
		case "setFire":
			return TryToSmiteTree ();
		case "fallOnPlayer":
                        Debug.Log("Fall On Player Please");
			return TryToFallTree ();
		case "giveAcron":
			return TryToDropNut ();
		default:
			return 0;
		}
	}

	protected override void Reset() {}

	private int TryToGrowNut() {
		GameObject nut = the_world.GetObjectNearestPlayer("nut");
		if (nut != null) {
			the_world.Remove (nut);
			Debug.Log ("Nut Destroyed");
			return 1;
		}
		return 0;
	}

	private int TryToDropNut () {
		GameObject tree = the_world.GetObjectNearestPlayer("tree");
		if (tree != null) {
			tree.GetComponent<Tree> ().DropNut ();
			return 1;
		}
		return 0;
	}

	private int TryToSmiteTree() {
		GameObject tree = the_world.GetObjectNearestPlayer("tree");
		if (tree != null) {
			tree.GetComponent<Tree> ().GetSmitten ();
			return 1;
		}
		return 0;
	}

	private int TryToFallTree() {
		GameObject tree = the_world.GetObjectNearestPlayer("tree");
		if (tree != null) {
			tree.GetComponent<Tree> ().Fall ();
			return 1;
		}
		return 0;
	}
}
