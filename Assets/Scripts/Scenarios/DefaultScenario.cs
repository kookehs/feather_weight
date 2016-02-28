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
			return TryToGrowNut();
		case "setFire":
			return TryToSmiteTree ();
		case "fallOnPlayer":
			return TryToFallTree ();
		case "giveAcorn":
			return TryToDropNut ();
		case "Day":
			return TryToChangeTimeOfDay ("DAY");
		case "Night":
			return TryToChangeTimeOfDay ("NIGHT");
		default:
			return 0;
		}
	}

	protected override void Reset() {}

	private int TryToGrowNut() {
		GameObject nut = the_world.GetObjectNearestPlayer("Nut");
		if (nut != null) {
			the_world.Remove (nut);
			Debug.Log ("Nut Destroyed");
			return 1;
		}
		return 0;
	}

	private int TryToDropNut () {
		GameObject tree = the_world.GetObjectNearestPlayer("Tree");
		if (tree != null) {
			tree.GetComponent<Tree> ().DropNut ();
			return 1;
		}
		return 0;
	}

	private int TryToSmiteTree() {
		GameObject tree = the_world.GetObjectNearestPlayer("Tree");
		if (tree != null) {
			tree.GetComponent<Tree> ().GetSmitten ();
			return 1;
		}
		return 0;
	}

	private int TryToFallTree() {
		GameObject tree = the_world.GetObjectNearestPlayer("Tree");
		if (tree != null) {
			tree.GetComponent<Tree> ().Fall ();
			return 1;
		}
		return 0;
	}

	private int TryToChangeTimeOfDay(string t) {
		the_world.GetComponent<WeatherController> ().ChangeTimeOfDay (t);
		return 0;
	}
}
