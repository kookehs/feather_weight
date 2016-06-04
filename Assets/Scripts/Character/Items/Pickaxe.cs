using UnityEngine;
using System.Collections;

public class Pickaxe : Weapon
{
	private string me = "Weapon_Pickaxe";
	public float strong_knockback = 600f;

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
                case "Decoy_Chicken":
                case "Chicken":
			//Debug.Log ("Weapon Colliding");
			killed = other.gameObject.GetComponent<Chicken> ().receiveHit (GetComponent<Collider> (), 0, strong_knockback, me);
			break;
		case "Tree":
			disableMe ();
			break;
		case "Bush":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), 1, 0, me);
			Instantiate (Resources.Load ("Debris_Grass"), other.transform.position, Quaternion.identity);
			break;
		case "Rock3D":
                //disableMe ();
                Instantiate (Resources.Load("Particle Effects/Debris_Rock"), transform.position, Quaternion.identity);
                if (other.gameObject.transform.parent.name.Contains("Wall"))
                {
                    other.gameObject.transform.parent.GetComponent<Destroyable>().receiveHit(GetComponent<Collider>(), 1, 0, me);
                }
                else other.gameObject.GetComponent<Destroyable>().receiveHit(GetComponent<Collider>(), 1, 0, me);
			    break;
		case "Tech":
		case "MetalScrap":
		case "Special_Antenna":
			other.gameObject.GetComponent<Destroyable> ().receiveHit (GetComponent<Collider> (), 10, 0, me);
			break;
		case "Boss":
			other.gameObject.GetComponent<Hand> ().receiveHit (GetComponent<Collider> (), 35, 0, me);
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
