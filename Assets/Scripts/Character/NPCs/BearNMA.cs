using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//
//	This version of the Bear relies on calls to Unity's NavMeshAgent component.
//

public class BearNMA : Animal
{

	public Transform cub;
	public float seeDistance = 40f;

	public override void performStateCheck ()
	{
		if (target == null)
			target = player;
		//	If we are not running...
		if (state != AnimalState.RUNNING) {
			//	Consider the distance between bear and target
			if (target!=null) {
				state = AnimalState.HOSTILE;
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

	protected override void OnCollisionStay (Collision collision)
	{
		base.OnCollisionStay (collision);
		/*if (collision.collider.tag.Equals ("Chicken")) {
			collision.gameObject.GetComponent<Chicken> ().receiveHit (GetComponent<Collider> (), base_damage * power, base_knockback * power, tag);
		}*/
	}

	protected override void Initialize ()
	{
		GetComponent<DropCollectable> ().collectables = new string[] {"Cooked_Meat"};
		GetComponent<DropCollectable> ().drop_rates = new double[]   { 0.1 };

		rage_duration = 5f;

		if (target == null)
			target = player;
	}
}
