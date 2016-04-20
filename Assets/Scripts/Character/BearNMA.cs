using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//	
//	This version of the Bear relies on calls to Unity's NavMeshAgent component.
//

public class BearNMA : Animal {

	public Transform cub;
	public float seeDistance = 40f;

	public override void performStateCheck(){

		//Debug.Log (nma.destination);

		//	If we are not running...
		if (state != AnimalState.RUNNING) {

			//	Consider the distance between bear and target
			if (Vector3.Distance (target.transform.position, transform.position) < 15f) {
				if (GetComponent<Health> ().health > 20) {
					//	It either becomes friendly.
					if (friendliness > 0) {
						state = AnimalState.FRIENDLY;
						//	Or it becomes hostile.
					} else if (friendliness <= 0) {
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

	protected override void Rage(float powerup) {
		// TODO juice
		power = powerup;
		GetComponent<NavMeshAgent> ().speed = 5f;
	}

	protected override void AfterRage() {
		power = 1f;
		GetComponent<NavMeshAgent> ().speed = 3.5f;
	}

	protected override void Initialize() {
		primary_drop = "Raw_Meat";

		rage_duration = 5f;

		if (target == null)
			target = player;
	}
}
