using UnityEngine;
using System.Collections;

public class Spear : Weapon
{
	public float true_damage = 10f;
	public float ieff_damage = 1f;
	public float strong_knockback = 1000f;
	public float weak_knockback = 0f;

	private string me = "Weapon_Spear";

	protected override void OnTriggerEnter (Collider other)
	{ 
		//Debug.Log ("Weapon Colliding");
		bool killed = false;

		switch (other.tag) {
		case "Bear":
			killed = other.gameObject.GetComponent<BearNMA> ().receiveHit (GetComponent<Collider> (), true_damage, strong_knockback, me);
			break;
		case "MountainLion":
			killed = other.gameObject.GetComponent<MountainLion> ().receiveHit (GetComponent<Collider> (), true_damage, strong_knockback, me);
			break;
		case "Tree":
			transform.parent.transform.parent.gameObject.GetComponent<WeaponController> ().playBuzzer();
			disableMe ();
			break;
		case "Rock3D":
			transform.parent.transform.parent.gameObject.GetComponent<WeaponController> ().playBuzzer();
			disableMe ();
			break;
		case "Bush":
			other.gameObject.GetComponent<Destroyable>().receiveHit(GetComponent<Collider>(), 10,0);
			break;
		case "Tech":
		case "MetalScrap":
		case "Special_Antenna":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), ieff_damage, weak_knockback, me);
			break;
		case "Boss":
			other.gameObject.GetComponent<Hand> ().receiveHit (GetComponent<Collider> (), 10, 0);
			break;
		default:
			break;
		}

		if (killed) {
			the_world.UpdateKillCount (other.tag);
		}
	}

	protected override void OnEnable ()
	{
		GetComponent<Animator> ().Play ("spear_swing");
	}
}
