﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Strikeable : MonoBehaviour
{
	protected static QuestController quest_controller;

	protected Rigidbody rb;
	protected bool stunned;
	protected bool invincible;
	protected float stun_time;
	protected float stun_length = 1f;
	protected float invincible_length;
	protected float invincible_time;

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

	//	Precondition:	This function is called by weapon scripts (ie Sword.cs) during collisions
	//
	//	Postcondition:	Returns whether this gameObject is dead
	public virtual bool receiveHit (Collider other, float damage, float knock_back_force, string hitter)
	{
		if (!invincible) {
			BeforeHit (hitter);
			DuringHit (other, damage, knock_back_force, hitter);
		}
		return AfterHit (hitter);
	}

	protected virtual void BeforeHit (string hitter)
	{

	}

	//	Precondition: This function is called within ReceiveHit()
	//
	//	Postcondition:	Health of this gameObject is updated, this gameObject is knocked back and stunned.
	protected virtual void DuringHit (Collider other, float damage, float knock_back_force, string hitter)
	{
		if (sound_on_strike != null)
			other.gameObject.GetComponent<AudioSource> ().PlayOneShot (sound_on_strike);

		Health health = GetComponent<Health> ();
		if (health != null)
			health.Decrease (damage);

		if (knock_back_force > 0)
			KnockBack (other, knock_back_force);
		Stun (.3f);
		IFrames (.5f);
	}

	//	Precondition: This function is called within ReceiveHit()
	//
	//	Postcondition:	Returns whether this gameObject is dead
	protected virtual bool AfterHit (string hitter)
	{
		Health health = GetComponent<Health> ();
		if (health != null) {
			bool isDead = health.IsDead ();
			if (isDead) {
				DropCollectable (hitter);
				WorldContainer.the_world.Remove (gameObject);
			}
			return isDead;
		}
		return false;
	}

	protected virtual void DropCollectable (string hitter)
	{
		Vector3 drop_position = new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z);
		WorldContainer.the_world.Create (primary_drop, drop_position);
		if (secondary_drops != null && secondary_drops.Count > 0) {
			//the_world.Create (secondary_drops [the_world.RandomChance (secondary_drops.Count)], drop_position);
			foreach (string thing in secondary_drops) WorldContainer.the_world.Create(thing, drop_position);
		}
		DropSpecial (drop_position);
	}

	protected virtual void DropSpecial (Vector3 drop_position)
	{
		if (QUEST_IDS != null && quest_controller.QuestActivated (QUEST_IDS, QUEST_UNION)) {
			Debug.Log ("Dropping Special");
			foreach (string s in special_drops)
				WorldContainer.the_world.Create (s, drop_position);
		}
	}

	protected virtual void KnockBack (Collider other, float knock_back_force)
	{
		Vector3 knock_back_direction = Vector3.Normalize (transform.position - other.transform.position);
		knock_back_direction.y = 1;
		if (knock_back_direction.x == 0 && knock_back_direction.z == 0) {
			knock_back_direction.x = (float) WorldContainer.the_world.RandomChance ();
			knock_back_direction.z = (float) WorldContainer.the_world.RandomChance ();
		}
		rb.AddForce (knock_back_direction * knock_back_force);
	}

	protected virtual void Stun (float length) {
		stun_length = length;
		stunned = true;
		stun_time = Time.time;
	}

	protected virtual void Unstun(){
		stunned = false;
	}

	protected virtual void IFrames (float length){
		invincible_length = length;
		invincible = true;
		invincible_time = Time.time;
	}

	protected void InitializeWorldContainer() {
		if (WorldContainer.the_world == null) WorldContainer.the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
	}

	protected void InitializeQuestController() {
		if (quest_controller == null) {
                    GameObject monument = GameObject.Find ("Monument");

                    if (monument != null) {
                        quest_controller = monument.GetComponent<QuestController>();
                    }
                }
	}
}
