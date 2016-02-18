using UnityEngine;
using System.Collections;

public class Rabbit : Animal {

	public override void performStateCheck(){
		//	If a rabbit is close enough to a player
		if (Vector3.Distance (player.transform.position, transform.position) < 5f) {
			//	It runs away
			state = AnimalState.RUNNING;
			//	At a certain distance, it becomes unaware of the player.
		} else
			state = AnimalState.UNAWARE;
	}
}
