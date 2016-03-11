using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour
{
	public float true_damage = 10f;
	public float ieff_damage = 1f;
	public float strong_knockback = 1000f;
	public float weak_knockback = 0f;

	private string me = "Weapon_Sword";

	WorldContainer the_world;

	// Use this for initialization
	void Start ()
	{
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
	}

	// Update is called once per frame
	void Update ()
	{

	}

	void OnTriggerEnter (Collider other)
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
		case "Rabbit":
			killed = other.gameObject.GetComponent<Rabbit> ().receiveHit (GetComponent<Collider> (), true_damage, strong_knockback, me);
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
			other.gameObject.GetComponent<Tree> ().receiveHit (GetComponent<Collider> (), true_damage, weak_knockback, me);
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
			the_world.UpdateKillCount (other.tag);
		}
	}

	void OnEnable ()
	{
		GetComponent<Animator> ().Play ("sword_swing");
	}


	void disableMe ()
	{
		if (gameObject.layer.Equals (0))
			gameObject.SetActive (false);
	}
}
