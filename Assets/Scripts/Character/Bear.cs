using UnityEngine;
using System.Collections;

public class Bear : Animal
{

	public GameObject cub;

	public override void performStateCheck(){

		//	If we are not running...
		if (state != AnimalState.RUNNING) {

			//	Consider the distance between player and bear
			if (Vector3.Distance (player.transform.position, transform.position) < 10f) {
				if (GetComponent<Health> ().health > 20) {
					//	It either becomes friendly.
					if (friendliness > 0) {
						target = player;
						state = AnimalState.FRIENDLY;
						//	Or it becomes hostile.
					} else if (friendliness <= 0) {
						target = player;
						state = AnimalState.HOSTILE;
					}
				} else {
					state = AnimalState.RUNNING;
				}
				//	At a certain distance, the bear becomes unaware of the player.
			} else
				state = AnimalState.UNAWARE;

		} else {
			if (runTime < 150f) {
				runTime += Time.deltaTime;
			} else {
				runTime = 0;
				state = AnimalState.UNAWARE;
			}
		}

	}

	public void makeCub ()
	{
		Instantiate (cub, transform.position, Quaternion.identity);
	}
	
}
