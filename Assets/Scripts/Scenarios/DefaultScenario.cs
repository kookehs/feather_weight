using UnityEngine;

public class DefaultScenario: Scenario
{

	public DefaultScenario ()
	{
		Initialize (this, 0);
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
		// case "giveAcorn":
		//	return TryToDropNut ();
		case "Day":
			return TryToChangeTimeOfDay ("DAY");
		case "Night":
			return TryToChangeTimeOfDay ("NIGHT");
		default:
			return 0;
		}
	}

	protected override void Reset ()
	{
	}

	private int TryToGrowNut ()
	{
		int cost = 100;
		if (master.GetCurrentGI () > cost) {
			GameObject nut = the_world.GetObjectNearestPlayer ("Nut");
			if (nut != null) {
				the_world.Create ("PineTree", nut.transform.position);
				the_world.Remove (nut);
				return cost;
			}
		}
		return MINCOMMANDCOST;
	}

	/*private int TryToDropNut () {
		GameObject tree = the_world.GetObjectNearestPlayer("Tree");
		if (tree != null) {
			tree.GetComponent<Tree> ().DropNut ();
			return 1;
		}
		return 0;
	}*/

	private int TryToSmiteTree ()
	{
		int cost = 100;
		if (master.GetCurrentGI () > cost) {
			GameObject tree = the_world.GetObjectNearestPlayer ("Tree");
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
			GameObject tree = the_world.GetObjectNearestPlayer ("Tree");
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
		if (master.GetCurrentGI () > cost && the_world.GetComponent<WeatherController> ().ChangeTimeOfDay (t))
			return cost;
		return MINCOMMANDCOST;
	}
}