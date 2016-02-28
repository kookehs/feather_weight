using UnityEngine;
using System.Collections;

//	
//	This version of the Bear relies on calls to Unity's NavMeshAgent component.
//

public class BearNMA : Animal {

	public override void performStateCheck(){

		//Debug.Log (nma.destination);

		//	If we are not running...
		if (state != AnimalState.RUNNING) {

			//	Consider the distance between player and bear
			if (Vector3.Distance (player.transform.position, transform.position) < 15f) {
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

	protected override void Initialize() {
	}
}
