using UnityEngine;

public class DefaultScenario: Scenario {

	public DefaultScenario () {
		InitializeWorldContainer ();
	}

	public override bool CheckTriggerConditions() {
		return true;
	}

	public override int EffectTwitchDesire(string command) {
		string[] parameters = command.Split (separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (parameters[0]) {
		case "GrowNut":
			Debug.Log ("Growing Nut");
			return TryToGrowNut ();
		case "SmiteTree":
			Debug.Log ("Smiting Tree");
			return TryToSmiteTree ();
		case "FallTree":
			Debug.Log ("Falling Tree");
			return TryToFallTree ();
		default:
			return 0;
		}
	}

	private int TryToGrowNut() {
		GameObject nut = the_world.NearestObjectToPlayer("nut");
		if (nut != null) {
			GameObject.DestroyImmediate (nut);
			Debug.Log ("Nut Destroyed");
			the_world.UpdateObjectArray ("nut");
			return 1;
		}
		return 0;
	}

	private int TryToSmiteTree() {
		GameObject tree = the_world.NearestObjectToPlayer ("tree");
		if (tree != null) {
			tree.GetComponent<Tree>().GetSmitten ();
		}
		return 0;
	}

	private int TryToFallTree() {
		GameObject tree = the_world.NearestObjectToPlayer ("tree");
		if (tree != null) {
			tree.GetComponent<Tree> ().Fall ();
		}
		return 0;
	}
}

