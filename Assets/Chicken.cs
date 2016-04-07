using UnityEngine;
using System.Collections;

public class Chicken : Animal {

	public override void performStateCheck(){
		//	If we are not running...
		if (friendliness > 0) {
			if (state == AnimalState.RUNNING) {
				if (runTime < 150f) {
					runTime += Time.deltaTime;
				} else {
					runTime = 0;
					state = AnimalState.UNAWARE;
				}
			}else if (Vector3.Distance (player.transform.position, transform.position) < 10f) {
				state = AnimalState.RUNNING;
				target = player;
			} else {
				state = AnimalState.UNAWARE;
			}
		} else {
			state = AnimalState.HOSTILE;
		}
	}

	protected override void Initialize(){

	}
}
