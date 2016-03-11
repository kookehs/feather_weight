using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Strikeable : MonoBehaviour
{
	protected static WorldContainer the_world;
	protected static QuestController quest_controller;

	protected Rigidbody rb;
	protected bool stunned;
	protected float stun_time;
	protected float stun_length = 1f;

	// set via the Inspecter
	public AudioClip sound_on_strike;

	// must be set by initialization via the Start() function
	protected string primary_drop;
	protected List<string> secondary_drops;
	protected List<string> special_drops;
	protected int[] QUEST_IDS;
	protected bool QUEST_UNION = false;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public virtual bool receiveHit (Collider other, float damage, float knock_back_force, string hitter)
	{
		BeforeHit (hitter);
		DuringHit (other, damage, knock_back_force, hitter);
		return AfterHit (hitter);
	}

	protected virtual void BeforeHit (string hitter)
	{
		
	}

	protected virtual void DuringHit (Collider other, float damage, float knock_back_force, string hitter)
	{
		if (sound_on_strike != null)
			other.gameObject.GetComponent<AudioSource> ().PlayOneShot (sound_on_strike);

		Health health = GetComponent<Health> ();
		if (health != null)
			health.Decrease (damage);

		if (knock_back_force > 0)
			KnockBack (other, knock_back_force);

		Stun (1f);
	}

	protected virtual bool AfterHit (string hitter)
	{
		Health health = GetComponent<Health> ();
		if (health != null) {
			bool isDead = health.IsDead ();
			if (isDead) {
				DropCollectable (hitter);
				the_world.Remove (gameObject);
			}
			return isDead;
		}
		return false;
	}

	protected virtual void DropCollectable (string hitter)
	{
		Vector3 drop_position = new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z);
		the_world.Create (primary_drop, drop_position);
		if (secondary_drops != null && secondary_drops.Count > 0) {
			the_world.Create (secondary_drops [the_world.RandomChance (secondary_drops.Count)], drop_position);
		}
		DropSpecial (drop_position);
	}

	protected virtual void DropSpecial (Vector3 drop_position)
	{
		if (QUEST_IDS != null && quest_controller.QuestActivated (QUEST_IDS, QUEST_UNION)) {
			Debug.Log ("Dropping Special");
			foreach (string s in special_drops)
				the_world.Create (s, drop_position);
		}
	}

	protected virtual void KnockBack (Collider other, float knock_back_force)
	{
		Vector3 knock_back_direction = Vector3.Normalize (transform.position - other.transform.position);
		knock_back_direction.y = 1;
		rb.AddForce (knock_back_direction * knock_back_force);
	}

	protected virtual void Stun(float length) {
		stunned = true;
		stun_time = Time.time;
		stun_length = length;
	}

	protected void InitializeWorldContainer() {
		if (the_world == null) the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
	}

	protected void InitializeQuestController() {
		if (quest_controller == null) quest_controller = GameObject.Find ("Monument").GetComponent<QuestController> ();
	}
}

