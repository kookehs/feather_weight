using UnityEngine;

public class DefaultScenario: Scenario {

	public DefaultScenario () {
		InitializeWorldContainer ();
	}

	public override bool CheckTriggerConditions() {
		return false;
	}

	public override int EffectTwitchDesire(string outcome) {
		switch (outcome) {
		case "GrowNut":
			Debug.Log ("Growing Nut");
			return TryToGrowNut();
		default:
			return 0;
		}
	}

	private int TryToGrowNut() {
		GameObject nut = the_world.NearestToPlayer("nut");
		if (nut != null) {
			GameObject.DestroyImmediate (nut);
			Debug.Log ("Nut Destroyed");
			the_world.Update ("nut");
		}
		return 0;
	}
}

