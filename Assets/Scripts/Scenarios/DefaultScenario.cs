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
		case "GrowNut":
			Debug.Log ("Growing Nut");
			return TryToGrowNut();
		case "SmiteTree":
			return TryToSmiteTree ();
		case "FallTree":
			return TryToFallTree ();
		case "DropNut":
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

