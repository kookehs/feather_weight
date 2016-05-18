using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Wolf : Animal {

	public WeaponController weaponController;

	public override void Start() {
		base.Start();
		weaponController = player.transform.Find ("WeaponHolder").GetComponent<WeaponController>();
	}

	public override void performStateCheck ()
	{
		//	If the player is nearby and has a torch equipped...
		if ((weaponController.myWeapon.tag.Equals ("Torch") || weaponController.myWeapon.tag.Equals("Flashlight")) 
			&& Vector3.Distance (player.transform.position, transform.position) < 20f) {
			//	...Run!
			target = player;
			state = AnimalState.RUNNING;
		} else { 
			//	Always target nearest chicken
			changeTarget (WorldContainer.GetNearestObject ("Chicken", gameObject));
			state = AnimalState.HOSTILE;
		}

	}

	protected override void Initialize() {
	}
}
