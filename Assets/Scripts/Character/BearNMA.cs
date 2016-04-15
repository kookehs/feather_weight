using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//	
//	This version of the Bear relies on calls to Unity's NavMeshAgent component.
//

public class BearNMA : Animal {

	public Transform cub;

	public override void performStateCheck(){

		//Debug.Log (nma.destination);

		//	If we are not running...
		if (state != AnimalState.RUNNING) {

			//	Consider the distance between bear and target
			//  maybe I shouldn't be checking if target is null
			if (target != null && Vector3.Distance (target.transform.position, transform.position) < 15f) {
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

	protected override void Initialize() {
		primary_drop = "Raw_Meat";
		secondary_drops = new List<string> ();
		secondary_drops.Add ("Hide");
		secondary_drops.Add ("Teeth");
	}

	public void makeCub ()
	{
		WorldContainer.Create (cub, transform.position);
	}
}
