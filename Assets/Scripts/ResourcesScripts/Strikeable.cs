using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Strikeable : MonoBehaviour
{
	protected Rigidbody rb;
	protected bool stunned;
	protected float stunTime;
	protected WorldContainer the_world;
	protected QuestController quest_controller;

	// set via the Inspecter
	public AudioClip sound_on_strike;

	// must be set by initialization via the Start() function
	protected string primary_drop;
	protected List<string> secondary_drops;
	protected List<string> special_drops;
	protected bool special_activated = false;
	protected int[] QUEST_IDS;
	protected bool QUEST_UNION;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public bool receiveHit (Collider other, float damage, float knock_back_force)
	{
		BeforeHit ();
		DuringHit (other, damage, knock_back_force);
		return AfterHit ();
	}

	protected virtual void BeforeHit ()
	{
		
	}

	protected virtual void DuringHit (Collider other, float damage, float knock_back_force)
	{
		if (sound_on_strike != null)
			other.gameObject.GetComponent<AudioSource> ().PlayOneShot (sound_on_strike);

		Health health = GetComponent<Health> ();
		if (health != null)
			health.decreaseHealth (damage);

		if (knock_back_force > 0)
			KnockBack (other, knock_back_force);
	}

	protected virtual bool AfterHit ()
	{
		Health health = GetComponent<Health> ();
		if (health != null) {
			bool isDead = health.isDead ();
			if (isDead) {
				DropCollectable ();
				the_world.Remove (gameObject);
			}
			return isDead;
		}
		return false;
	}

	protected virtual void DropCollectable ()
	{
		Vector3 drop_position = new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z);
		Instantiate (Resources.Load (primary_drop), drop_position, transform.rotation);
		if (secondary_drops != null && secondary_drops.Count > 0) {
			Instantiate (Resources.Load (secondary_drops [the_world.RandomChance (secondary_drops.Count)]), drop_position, transform.rotation);
		}
		DropSpecial (drop_position);
	}

	protected virtual void DropSpecial (Vector3 drop_position)
	{
		if (quest_controller.QuestActivated(QUEST_IDS, QUEST_UNION)) {
			foreach (string s in special_drops)
				Instantiate (Resources.Load (s), drop_position, transform.rotation);
		}
	}

	protected virtual void KnockBack (Collider other, float knock_back_force)
	{
		Vector3 knock_back_direction = Vector3.Normalize (transform.position - other.transform.position);
		knock_back_direction.y = 1;
		rb.AddForce (knock_back_direction * 600);
		stunned = true;
		stunTime = Time.time;
	}
}

