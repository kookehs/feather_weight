using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rabbit : Animal {

	public override void performStateCheck(){
		//	If we are not running...
		Debug.Log (friendliness);
		if (state != AnimalState.HOSTILE && state != AnimalState.RUNNING) {
			//	If a rabbit is close enough to a player
			if (Vector3.Distance (player.transform.position, transform.position) < 10f) {
				//	It runs away
				target = player;
				if (friendliness > 0) {
					state = AnimalState.RUNNING;
					//	Or it becomes hostile.
				} else if (friendliness <= 0) {
					state = AnimalState.HOSTILE;
				}
				//	At a certain distance, it becomes unaware of the player.
			}
		} else {
			if (friendliness <= 0) {
				state = AnimalState.HOSTILE;
			}else {
				if (runTime < 150f) {
					runTime += Time.deltaTime;
				} else {
					runTime = 0;
					state = AnimalState.UNAWARE;
				}
			}
		}
	}

	public bool isKiller() {
		if (state == AnimalState.HOSTILE) return true;
		return false;
	}

	protected override void Initialize() {
		primary_drop = "Raw_Meat";
		special_drops = new List<string> ();
		special_drops.Add ("Special1");
		special_drops.Add ("Special3");
		QUEST_IDS = new int[] { 1, 2 };
		QUEST_UNION = false;

		friendliness = 1f;
	}

	protected override void DropSpecial(Vector3 drop_position) {
		if (isKiller())
			Instantiate (Resources.Load(special_drops[1]), drop_position, transform.rotation);
		else 
			Instantiate (Resources.Load(special_drops[0]), drop_position, transform.rotation);
	}
}
