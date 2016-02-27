using UnityEngine;
using System.Collections;

public class Rabbit : Animal {

	public override void performStateCheck(){
		bool killerRabbit = false;

		//	If we are not running...
		if (state != AnimalState.RUNNING) {
			//	If a rabbit is close enough to a player
			if (Vector3.Distance (player.transform.position, transform.position) < 10f) {
				//	It runs away
				target = player;
				state = AnimalState.RUNNING;
				//	At a certain distance, it becomes unaware of the player.
			}
		} else if(state != AnimalState.HOSTILE){
			if (killerRabbit) {
				//transsform change sprite
				//	If a rabbit is close enough to a player
				if (Vector3.Distance (player.transform.position, transform.position) < 10f) {
					//	It attacks the player
					target = player;
					state = AnimalState.HOSTILE;
					//	At a certain distance, it becomes unaware of the player.
				}

				if (GetComponent<Health> ().isDead ()) {
					DropAntler ();
				}
			}
		}{
			if (runTime < 150f) {
				runTime += Time.deltaTime;
			} else {
				runTime = 0;
				state = AnimalState.UNAWARE;
			}
		}
	}

	public void DropAntler(){
		Instantiate (Resources.Load("Antler"), new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), transform.rotation);
	}
}
