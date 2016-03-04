using UnityEngine;
using System;
using System.Collections.Generic;

public class NearBearScenario: Scenario
{
	private GameObject the_bear = null;

	public NearBearScenario (DefaultScenario ds)
	{
		Initialize (ds, 1);
	}

	public override bool CheckTriggerConditions ()
	{
		if (the_bear != null) {
			// If the current bear has moved outside the radius of consideration, remove it from consideration
			if (!the_world.IsObjectNearPlayer (the_bear, the_world.GetViewableRadius ()))
				the_bear = null;
		}
		if (the_bear == null) {
			//If there is no bear currently being considered, find such a bear if it exists
			GameObject bear = the_world.GetObjectNearestPlayer ("Bear");
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

	protected override void Reset ()
	{
		the_bear = null;
	}

	public override int EffectCommand (string input)
	{
		string[] parameters = input.Split (separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (parameters [0]) {
		case "increaseHostility":
			return TryToAffectFriendliness ("negative", the_bear);
		case "decreaseHostility":
			return TryToAffectFriendliness ("positive", the_bear);
		case "MadBears":
			return TryToMassAffectFriendliness ("negative");
		case "HappyBears":
			return TryToMassAffectFriendliness ("positive");
		case "spawnBearCub":
			return TryToSpawnCub (the_bear);
		case "runAway":
		case "runAwayBear":
                        //the_bear.GetComponent<BearNMA>().setRun();
			Debug.Log ("Need a run away function for Bear");
			return 0;
		case "increaseStrength":
			the_bear.GetComponent<BearNMA> ().rage ();
			return 0;
		default:
			return default_scenario.EffectCommand (input);
		}
	}

	private int TryToMassAffectFriendliness (string sign)
	{
		int cost = 100;
		if (master.GetCurrentGI () > cost) {
			List<GameObject> bears = the_world.GetAllObjectsNearPlayer ("Bear");
			if (bears.Count != 0) {
				foreach (GameObject bear in bears)
					TryToAffectFriendliness (sign, bear);
				return cost;
			}
		}
		return MINCOMMANDCOST;
	}

	private int TryToAffectFriendliness (string sign, GameObject b)
	{
		int cost = 100;
		if (master.GetCurrentGI () > cost && b != null) {
			BearNMA bear = b.GetComponent<BearNMA> ();
			if (sign.Equals ("negative"))
				bear.decreaseFriendliness ();
			else
				bear.increaseFriendliness ();
			return cost;
		}
		return MINCOMMANDCOST;
	}

	private int TryToSpawnCub (GameObject b)
	{
		int cost = 100;
		if (master.GetCurrentGI () > cost && b != null) {
			BearNMA bear = b.GetComponent<BearNMA> ();
			Debug.Log ("Need a make cub function for Bear");
		}
		return MINCOMMANDCOST;
	}
}
