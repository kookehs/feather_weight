using UnityEngine;
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
		default:
			return 0;
		}
	}
}

