using UnityEngine;
using System;
using System.Collections;

public class BossFightScenario : Scenario
{
	public BossFightScenario (DefaultScenario ds) {
		Initialize (ds, 100);
	}

	public override bool CheckTriggerConditions ()
	{
		return the_world.BOSS;
	}

	protected override void Reset ()
	{
		the_world.BOSS = false;
	}

	public override int EffectCommand (string input)
	{
		string[] parameters = input.Split (separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (parameters [0]) {
		case "FireLightning":
			return TryToFireLightning (parameters[1]);
		default:
			return 0;
		}
	}

	private int TryToFireLightning (string input) {
		int cost = 50;
		if (master.GetCurrentGI () >= cost) {
			GameObject.Find ("like a boss").GetComponent<Boss> ().FireLightningTwitchHelper (Int32.Parse(input));
			return cost;
		}
		return MINCOMMANDCOST;
	}
}

