using UnityEngine;
using System.Collections;

public class MountainLion : Animal
{

	public override void performStateCheck ()
	{

		//	Consider the distance between player and mountain lion
		if (Vector3.Distance (player.transform.position, transform.position) < 15f) {
			Debug.Log ("Hostile");
			target = player;
			state = AnimalState.HOSTILE;
		} else {
			Debug.Log ("Unaware");
			state = AnimalState.UNAWARE;
		}

	}

	protected override void Initialize() {
	}
}
