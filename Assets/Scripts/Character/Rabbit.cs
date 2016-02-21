using UnityEngine;
using System.Collections;

public class Rabbit : Animal {

	public override void performStateCheck(){

		//	If we are not running...
		if (state != AnimalState.RUNNING) {
			//	If a rabbit is close enough to a player
			if (Vector3.Distance (player.transform.position, transform.position) < 10f) {
				//	It runs away
				target = player;
				state = AnimalState.RUNNING;
				//	At a certain distance, it becomes unaware of the player.
			}
		} else {
			if (runTime < 150f) {
				runTime += Time.deltaTime;
			} else {
				runTime = 0;
				state = AnimalState.UNAWARE;
			}
		}
	}
}
