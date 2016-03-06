using UnityEngine;
using System.Collections;

public class Pickaxe : Weapon
{

	protected override void OnTriggerEnter (Collider other)
	{
		//Debug.Log ("Weapon Colliding");
		bool killed = false;

		switch (other.tag) {
		case "Bear":
			if (other.gameObject.GetComponent<Animal> () != null) {
				killed = other.gameObject.GetComponent<Animal> ().receiveHit (GetComponent<Collider> (), 5, 750);
			} else if (other.gameObject.GetComponent<BearRB> () != null) {
				killed = other.gameObject.GetComponent<BearRB> ().receiveHit (GetComponent<Collider> (), 5, 750);
			}
			break;
		case "MountainLion":
			if (other.gameObject.GetComponent<Animal> () != null) {
				killed = other.gameObject.GetComponent<Animal> ().receiveHit (GetComponent<Collider> (), 5, 750);
			}
			break;
		case "Tree":
			other.gameObject.GetComponent<Tree> ().hitBy (tag);
			transform.parent.transform.parent.gameObject.GetComponent<WeaponController> ().playBuzzer ();
			disableMe ();
			break;
		case "Rock3D":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), 10, 0);
			break;
		case "Bush":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), 1, 0);
			break;
		case "Tech":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), 10, 0);
			break;
		case "MetalScrap":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), 10, 0);
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
		GetComponent<Animator> ().Play ("sword_swing");
	}
}
