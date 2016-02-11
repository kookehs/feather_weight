using UnityEngine;
using System;
using System.Collections.Generic;

public class NearBearScenario: Scenario
{
	private GameObject the_bear = null;

	public NearBearScenario (DefaultScenario ds)
	{
		InitializeWorldContainer ();
		default_scenario = ds;
		clearance_level = 1;
	}

	public override bool CheckTriggerConditions() {
		if (the_bear != null) {
			// If the current bear has moved outside the radius of consideration, remove it from consideration
			if(!the_world.IsObjectNearPlayer(the_bear, the_world.viewableRadius)) the_bear = null;
		}
		if (the_bear == null) {
			//If there is no bear currently being considered, find such a bear if it exists
			GameObject bear = the_world.GetObjectNearestPlayer ("bear");
			if (bear != null) {
				the_bear = bear;
				//A bear is found within the radius of consideration
				return true;
			} else
				//A bear was not found within the radius of consideration
				return false;
		}
		//A bear that is within the radius of consideration is being considered
		return true;
	}

	protected override void Reset() {
		the_bear = null;
	}

	public override int EffectTwitchDesire(string input) {
		string[] parameters = input.Split (separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (parameters[0]) {
		case "MadBear":
			return TryToAffectFriendliness ("negative", the_bear);
		case "HappyBear":
			return TryToAffectFriendliness ("positive", the_bear);
		case "MadBears":
			return TryToMassAffectFriendliness ("negative");
		case "HappyBears":
			return TryToMassAffectFriendliness ("positive");
		case "AsexualReproduction":
			return TryToSpawnCub (the_bear);
		default:
			return 0;
		}
	}

	private int TryToMassAffectFriendliness(string sign) {
		List<GameObject> bears = the_world.GetAllObjectsNearPlayer ("bear");
		if (bears.Count != 0) {
			foreach (GameObject bear in bears)
				TryToAffectFriendliness (sign, bear);
			return 1;
		} else
			return 0;
	}

	private int TryToAffectFriendliness(string sign, GameObject b) {
		BearRB bear = b.GetComponent<BearRB> ();
		if (sign.Equals("negative")) bear.decreaseFriendliness();
		                        else bear.increaseFriendliness();
		return 1;
	}

	private int TryToSpawnCub(GameObject b) {
		BearRB bear = b.GetComponent<BearRB> ();
		bear.makeCub();
		return 1;
	}
}