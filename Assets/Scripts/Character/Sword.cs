﻿using UnityEngine;
using System.Collections;

public class Sword : Weapon
{
	public float true_damage = 10f;
	public float ieff_damage = 1f;
	public float strong_knockback = 1000f;
	public float weak_knockback = 0f;

	protected override void OnTriggerEnter (Collider other)
	{
		//Debug.Log ("Weapon Colliding");
		bool killed = false;

		switch (other.tag) {
		case "Bear":
			killed = other.gameObject.GetComponent<Animal> ().receiveHit (GetComponent<Collider> (), true_damage, strong_knockback);
			break;
		case "MountainLion":
			killed = other.gameObject.GetComponent<Animal> ().receiveHit (GetComponent<Collider> (), true_damage, strong_knockback);
			break;
		case "Tree":
			other.gameObject.GetComponent<Tree> ().hitBy (tag);
			transform.parent.transform.parent.gameObject.GetComponent<WeaponController> ().playBuzzer();
			disableMe ();
			break;
		case "Rock3D":
			transform.parent.transform.parent.gameObject.GetComponent<WeaponController> ().playBuzzer();
			disableMe ();
			break;
		case "Bush":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), ieff_damage, weak_knockback);
			break;
		case "Tech":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), ieff_damage, weak_knockback);
			break;
		case "MetalScrap":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), ieff_damage, weak_knockback);
			break;
		default:
			break;
		}

		if (killed) {
			the_world.UpdateKillCount (other.tag);
		}
	}

}
