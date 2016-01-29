using UnityEngine;

public class DefaultScenario: Scenario {
	public DefaultScenario () {
		the_world = GameObject.Find ("WorldContainer");
	}

	public override bool CheckTriggerConditions() {
		return false;
	}

	public override int EffectTwitchDesire(string outcome) {
		switch (outcome) {
		default:
			return 0;
		}
	}
}

