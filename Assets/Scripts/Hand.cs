using UnityEngine;
using System.Collections;

public class Hand : Strikeable {

	private string me = "BOSS_HAND";

	// Use this for initialization
	public void Start () {
		base.Start ();
		transform.GetComponent<Health> ().health = WaveController.hand_hp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual bool receiveHit (Collider other, float damage, float knockBackForce)
	{
		GetComponentInParent<Health> ().Decrease (damage);

		return GetComponent<Health> ().IsDead ();
	}

	void OnTriggerEnter (Collider collision)
	{
		if (collision.gameObject.tag.Equals ("Player")) {
			collision.gameObject.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), 10, 1000, me);
		}
	}
}
