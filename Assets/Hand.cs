using UnityEngine;
using System.Collections;

public class Hand : Strikeable {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual bool receiveHit (Collider other, float damage, float knockBackForce)
	{
		GetComponentInParent<Health> ().decreaseHealth (damage);

		return GetComponent<Health> ().isDead ();
	}

	void OnTriggerEnter (Collider collision)
	{
		if (collision.gameObject.tag.Equals ("Player")) {
			collision.gameObject.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), 10, 1000);
		}
	}
}
