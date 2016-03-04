using UnityEngine;
using System;
using System.Collections.Generic;

public class NearRabbitScenario: Scenario
{
	private GameObject the_animal = null;

	public NearRabbitScenario (DefaultScenario ds)
	{
		Initialize (ds, 3);
	}

	public override bool CheckTriggerConditions() {
		if (the_animal != null) {
			// If the current specified animal has moved outside the radius of consideration, remove it from consideration
			if(!the_world.IsObjectNearPlayer(the_animal, the_world.GetViewableRadius())) the_animal = null;
		}
		if (the_animal == null) {
			//If there is no specified animal currently being considered, find such an animal if it exists
			GameObject animal = the_world.GetObjectNearestPlayer ("Rabbit");
			if (animal != null) {
				the_animal = animal;
				//The specified animal is found within the radius of consideration
				return true;
			} else
				//The specified animal was not found within the radius of consideration
				return false;
		}
		//An animal of the specified type that is within the radius of consideration is being considered
		return true;
	}

	protected override void Reset() {
		the_animal = null;
	}

	public override int EffectCommand(string input) {
		string[] parameters = input.Split (separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (parameters[0]) {
		case "increaseHostility":
			return TryToAffectFriendliness ("negative", the_animal);
		case "decreaseHostility":
			return TryToAffectFriendliness ("positive", the_animal);
		case "MadRabbits":
			return TryToMassAffectFriendliness ("negative");
		case "HappyRabbits":
			return TryToMassAffectFriendliness ("positive");
		default:
			return default_scenario.EffectCommand(input);
		}
	}

	private int TryToMassAffectFriendliness(string sign) {
		int cost = 100;
		if (master.GetCurrentGI() > cost) {
		List<GameObject> rabbits = the_world.GetAllObjectsNearPlayer ("Rabbit");
			if (rabbits.Count != 0) {
				foreach (GameObject rabbit in rabbits)
					TryToAffectFriendliness (sign, rabbit);
				return cost;
			}
		}
		return MINCOMMANDCOST;
	}

	private int TryToAffectFriendliness(string sign, GameObject b) {
		int cost = 100;
		if (master.GetCurrentGI () > cost && b != null) {
			Rabbit rabbit = b.GetComponent<Rabbit> ();
			if (sign.Equals ("negative"))
				rabbit.decreaseFriendliness ();
			else
				rabbit.increaseFriendliness ();
			return cost;
		}
		return MINCOMMANDCOST;
	}
}

