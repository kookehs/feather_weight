using UnityEngine;
using System;
using System.Collections.Generic;

public class NearRabbitScenario: Scenario
{
	private GameObject the_animal = null;

	public NearRabbitScenario (DefaultScenario ds)
	{
		InitializeWorldContainer ();
		default_scenario = ds;
		clearance_level = 3;
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

	public override int EffectTwitchDesire(string input) {
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
			return default_scenario.EffectTwitchDesire(input);
		}
	}

	private int TryToMassAffectFriendliness(string sign) {
		List<GameObject> rabbits = the_world.GetAllObjectsNearPlayer ("Rabbit");
		if (rabbits.Count != 0) {
			foreach (GameObject rabbit in rabbits)
				TryToAffectFriendliness (sign, rabbit);
			return 1;
		} else
			return 0;
	}

	private int TryToAffectFriendliness(string sign, GameObject b) {
		Rabbit rabbit = b.GetComponent<Rabbit> ();
		if (sign.Equals("negative")) rabbit.decreaseFriendliness();
		else rabbit.increaseFriendliness();
		return 1;
	}
}

