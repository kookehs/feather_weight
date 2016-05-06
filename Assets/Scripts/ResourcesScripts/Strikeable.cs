using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Strikeable : MonoBehaviour
{
	protected Rigidbody rb;
	protected bool stunned;
	protected bool invincible;
	protected float stun_time;
	protected float stun_length = 1f;
	protected float invincible_length;
	protected float invincible_time;

	protected Camera camera;
	protected CollectionCursor cc;

	// set via the Inspecter
	public AudioClip sound_on_strike;

	public Animator anim;

	// Use this for initialization
	public void Start ()
	{
		anim = GetComponentInChildren<Animator> ();
		camera = Camera.main;
		cc = camera.GetComponent<CollectionCursor> ();
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
			GetComponent<AudioSource> ().PlayOneShot (sound_on_strike);

		Health health = GetComponent<Health> ();
		if (health != null)
			health.Decrease (damage);

		if (knock_back_force > 0)
			KnockBack (other, knock_back_force);
		Stun (.5f);
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
				StartCoroutine (WaitAndRemove ());
			}
			return isDead;
		}
		return false;
	}

	public IEnumerator WaitAndRemove() {
		yield return new WaitForSeconds (.5f);
		WorldContainer.Remove (gameObject);
	}

	protected virtual void KnockBack (Collider other, float knock_back_force)
	{
		Vector3 knock_back_direction = Vector3.Normalize (transform.position - other.transform.position);
		knock_back_direction.y = 1;
		if (knock_back_direction.x == 0 && knock_back_direction.z == 0) {
			knock_back_direction.x = (float) WorldContainer.RandomChance ();
			knock_back_direction.z = (float) WorldContainer.RandomChance ();
		}
		rb.AddForce (knock_back_direction * knock_back_force);
	}

	protected virtual void Stun (float length) {
		stunned = true;
		StartCoroutine (WaitAndUnstun (length));
		if (anim != null) {
			// Debug.Log ("Begin stun anim.");
			anim.SetBool ("stun", true);
			StartCoroutine (WaitAndEndStunAnim (length));
		}
	}

	protected virtual void Unstun(){
		stunned = false;
		if (anim != null) {
			anim.SetBool ("stun", false);
			// Debug.Log ("End stun anim.");
		}
	}

	protected virtual IEnumerator WaitAndUnstun(float length) {
		yield return new WaitForSeconds (length);
		Unstun ();
	}

	public IEnumerator WaitAndEndStunAnim(float length){
		yield return new WaitForSeconds (length);
		anim.SetBool ("stun", false);
	}

	protected virtual void IFrames (float length){
		invincible_length = length;
		invincible = true;
		invincible_time = Time.time;
	}

	protected virtual void OnMouseEnter(){
		if (cc != null) {
			cc.SetWeapon ();
		}
	}

	protected virtual void OnMouseExit () {
		if (cc != null) {
			cc.SetDefault ();
		}
	}
}
