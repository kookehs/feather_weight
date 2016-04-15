using UnityEngine;
using System.Collections;

public class Pickaxe : Weapon
{
	private string me = "Weapon_Pickaxe";

	protected override void OnTriggerEnter (Collider other)
	{
		//Debug.Log ("Weapon Colliding");
		bool killed = false;

		switch (other.tag) {
		case "Bear":
			killed = other.gameObject.GetComponent<BearNMA> ().receiveHit (GetComponent<Collider> (), 5, 350, me);
			break;
		case "Wolf":
			killed = other.gameObject.GetComponent<BearNMA> ().receiveHit (GetComponent<Collider> (), 5, 350, me);
			break;
		case "MountainLion":
			killed = other.gameObject.GetComponent<MountainLion> ().receiveHit (GetComponent<Collider> (), 5, 350, me);
			break;
		case "Tree":
			transform.parent.transform.parent.gameObject.GetComponent<WeaponController> ().playBuzzer ();
			disableMe ();
			break;
		case "Bush":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), 1, 0, me);
			break;
		case "Rock3D":
		case "Tech":
		case "MetalScrap":
		case "Special_Antenna":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), 10, 0, me);
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
		GetComponent<Animator> ().Play ("sword_swing");
	}
}
