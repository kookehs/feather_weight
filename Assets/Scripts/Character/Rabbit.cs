using UnityEngine;
using System.Collections;

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
		friendliness = 1f;
	}

	public void DropAntler(){
		Instantiate (Resources.Load("Antler"), new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), transform.rotation);
	}
}
