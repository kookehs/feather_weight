using UnityEngine;
using System.Collections;

public class Spear : Weapon
{
	public float true_damage = 10f;
	public float ieff_damage = 1f;
	public float strong_knockback = 400f;
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
		case "Wolf":
			killed = other.gameObject.GetComponent<Wolf> ().receiveHit (GetComponent<Collider> (), true_damage, strong_knockback, me);
			break;
        case "Rabbit":
			killed = other.gameObject.GetComponent<Rabbit> ().receiveHit (GetComponent<Collider> (), true_damage, strong_knockback, me);
			break;
		case "Chicken":
			//Debug.Log ("Weapon Colliding");
			killed = other.gameObject.GetComponent<Chicken> ().receiveHit (GetComponent<Collider> (), 0, strong_knockback, me);
			break;
		case "Tree":
			disableMe ();
			break;
		case "Rock3D":
			disableMe ();
			break;
		case "Bush":
			other.gameObject.GetComponent<Tree> ().receiveHit (GetComponent<Collider> (), true_damage, weak_knockback, me);
			Instantiate (Resources.Load ("Debris_Grass"), other.transform.position, Quaternion.identity);
			break;
		case "Tech":
		case "MetalScrap":
		case "Special_Antenna":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), ieff_damage, weak_knockback, me);
			break;
		case "Boss":
			other.gameObject.GetComponent<Hand> ().receiveHit (GetComponent<Collider> (), true_damage, weak_knockback, me);
			break;
		default:
			break;
		}

		if (killed) {
			WorldContainer.UpdateKillCount (other.tag);
		}
	}

	protected override void OnEnable ()
	{
		GetComponent<Animator> ().Play ("spear_swing");
	}
}
