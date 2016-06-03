using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Wolf : Animal {

	private bool attacking = false;
	public WeaponController weaponController;

	public override void Start() {
		base.Start();
		weaponController = player.transform.Find ("WeaponHolder").GetComponent<WeaponController>();
		transform.GetComponent<Health> ().health = WaveController.wolf_hp;
		transform.GetComponent<NavMeshAgent> ().speed = WaveController.wolf_spd;
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
			changeTarget (WorldContainer.GetNearestUncagedObject ("Chicken", gameObject));
			state = AnimalState.HOSTILE;
		}

	}

	protected override void Initialize() {
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag.Equals ("Chicken") && !attacking) {
			attacking = true;
			StartCoroutine (attackanim ());
			transform.GetComponent<AudioSource> ().Play(0);
		}
	}
	IEnumerator attackanim(){
		anim.SetBool ("attacking", true);
		yield return new WaitForSeconds (.26f);
		anim.SetBool ("attacking", false);
		attacking = false;
	}
}
