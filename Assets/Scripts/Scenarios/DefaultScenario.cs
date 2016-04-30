using UnityEngine;

public class DefaultScenario: Scenario
{
	WeatherController the_weather;

	public DefaultScenario ()
	{
		Initialize (this, 0);
		the_weather = the_world.GetComponent<WeatherController> ();
	}

	public override bool CheckTriggerConditions ()
	{
		return true;
	}

	public override int EffectCommand (string input)
	{
		string[] parameters = input.Split (separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (parameters [0]) {
		case "growTree":
			return TryToGrowNut ();
		case "setFire":
			return TryToSmiteTree ();
		case "fallOnPlayer":
			return TryToFallTree ();
		case "Day":
			return TryToChangeTimeOfDay ("DAY");
		case "Night":
			Debug.Log ("NIGHT TIME!");
			return TryToChangeTimeOfDay ("NIGHT");
		default:
			if (master.GetCurrentGI () < master.MAX_GI) return 0;
			return EffectMajorCommand (input);
		}
	}

	private int EffectMajorCommand (string input) {
		switch (input) {
		case "Poll 2x Day Speed":
			the_weather.SetTimeSpeed ("DAY", 2);
			the_weather.SetTimeSpeed ("NIGHT", 1);
			return (int) master.MAX_GI;
		case "Poll 2x Night Speed":
			the_weather.SetTimeSpeed ("DAY", 1);
			the_weather.SetTimeSpeed ("NIGHT", 2);
			return (int) master.MAX_GI;
		case "Poll All Bears Give Birth":
			return (int) master.MAX_GI;
		case "Poll Always Killer Bunnies":
			WorldContainer.KillerBunnies ();
			return (int) master.MAX_GI;
		case "Poll Permanent Day":
			TryToChangeTimeOfDay ("DAY");
			the_weather.SetTimeSpeed ("DAY", 0);
			return (int) master.MAX_GI;
		case "Poll Permanent Night":
			TryToChangeTimeOfDay ("NIGHT");
			the_weather.SetTimeSpeed ("NIGHT", 0);
			return (int) master.MAX_GI;
		default:
			return 0;
		}
	}

	protected override void Reset ()
	{
	}

	private int TryToGrowNut ()
	{
		Debug.Log ("Grow Tree");
		int cost = 100;
		if (master.GetCurrentGI () > cost) {
			Debug.Log ("Growing Tree");
			GameObject nut = WorldContainer.GetObjectNearestPlayer ("Nut");
			if (nut != null) {
				WorldContainer.Create ("PineTree", nut.transform.position);
				WorldContainer.Remove (nut);
				return cost;
			}
		}
		Debug.Log ("Did not grow tree.");
		return MINCOMMANDCOST;
	}

	private int TryToSmiteTree ()
	{
		return 0;
		int cost = 100;
		if (master.GetCurrentGI () > cost) {
			GameObject tree = WorldContainer.GetObjectNearestPlayer ("Tree");
			if (tree != null) {
				tree.GetComponent<Tree> ().GetSmitten ();
				return cost;
			}
		}
		return MINCOMMANDCOST;
	}

	private int TryToFallTree ()
	{
		int cost = 100;
		if (master.GetCurrentGI () > cost) {
			GameObject tree = WorldContainer.GetObjectNearestPlayer ("Tree");
			if (tree != null) {
				tree.GetComponent<Tree> ().Fall ();
				return cost;
			}
		}
		return MINCOMMANDCOST;
	}

	private int TryToChangeTimeOfDay (string t)
	{
		int cost = 100;
		if (master.GetCurrentGI () > cost && the_weather.ChangeTimeOfDay (t))
			return cost;
		return MINCOMMANDCOST;
	}
}
