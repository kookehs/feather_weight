using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Strikeable : MonoBehaviour
{
	static Random rng;
	Rigidbody rb;
	protected bool stunned;
	protected float stunTime;
	public AudioClip sound_on_strike;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public bool receiveHit(Collider other, float damage, float knock_back_force) {
		if (sound_on_strike != null)
			other.gameObject.GetComponent<AudioSource> ().PlayOneShot (sound_on_strike);
		
		Health health = GetComponent<Health> ();
		if (health != null)
			health.decreaseHealth (damage);

		if (knock_back_force > 0)
			KnockBack (other, knock_back_force);

		DropItems (); 

		if (health != null)
			return health.isDead ();
		else
			return false;
	}
		
	protected abstract void DropItems ();

	private void KnockBack (Collider other, float knock_back_force) {
		Vector3 knock_back_direction = Vector3.Normalize (transform.position - other.transform.position);
		knock_back_direction.y = 1;
		rb.AddForce (knock_back_direction * 600);
		stunned = true;
		stunTime = Time.time;
	}
}

