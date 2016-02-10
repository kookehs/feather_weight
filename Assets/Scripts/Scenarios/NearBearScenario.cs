using UnityEngine;

public class NearBearScenario: Scenario
{
	public NearBearScenario (DefaultScenario ds)
	{
		default_scenario = ds;
		InitializeWorldContainer ();
	}

	public override bool CheckTriggerConditions() {
		return false;
	}

	public override int EffectTwitchDesire(string command) {
		string[] parameters = command.Split (separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (parameters[0]) {
		case "IncreaseHostility":
			return TryToAffectFriendliness ("negative");
		case "DecreaseHostility":
			return TryToAffectFriendliness ("positive");
		case "MoveTo":
			return 0;
		default:
			return 0;
		}
	}

	private int TryToAffectFriendliness (string sign) {
		GameObject bear = the_world.NearestObjectToPlayer ("bear");
		if (sign.Equals ("negative")) {
			if (bear != null) {
				bear.GetComponent<BearRB> ().decreaseFriendliness ();
				return 1;
			}
		} else if (bear != null) {
			bear.GetComponent<BearRB> ().increaseFriendliness ();
			return 1;
		}
		return 0;
	}
}