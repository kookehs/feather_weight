using UnityEngine;
using System.Collections;

public class Torchick : Animal
{
	float firebreath_cd = 2f; // cooldown time
	bool  firebreath_cd_on = true;
	string _hex_loc;

	float y_extent;

	public string hex_loc {
		get { return _hex_loc;  }
		set { _hex_loc = value; }
	}

	protected override void BeforeHit(string hitter) {
		anim.SetBool ("stunned", true);
		StartCoroutine (WaitAndUnstun (Time.deltaTime));
	}

	protected override bool DamagePlayerOnCollision() {
		return false;
	}

	protected override void Initialize() {
		target = player;
		name = "Torchick";
		PhysicsOn ();
		StartCoroutine (WaitAndEnableFirebreath ());
		anim = GetComponentInChildren<Animator> ();
		y_extent = GetComponent<Collider> ().bounds.extents.y;
		state = AnimalState.HOSTILE;

		DropCollectable drops = GetComponent<DropCollectable> ();
		drops.collectables = new string[] { "TorchickCollectable" };
		drops.drop_rates   = new double[] { 1.0 };
	}

	void OnDestroy() {
		StopAllCoroutines();
		CancelInvoke ();
	}

	void OnTriggerStay(Collider other) {
		if (do_dropdamage) {
			if (other.CompareTag("Chicken")) return;
			Strikeable s = other.GetComponent<Strikeable> ();
			if (s == null || s.rb == null) return;
			s.receiveHit (GetComponent<Collider> (), 10f, 1000f, "ChickenButt"); //TODO: different damage to different creatures later
		}
	}

	protected override void OnCollisionStay (Collision collision)
	{
		if (collision.collider.tag.Equals ("Player") && DamagePlayerOnCollision()) {
			collision.gameObject.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), base_damage * power, base_knockback * power, tag);
		}
	}

	public override void performStateCheck() {
	}

	public override void performHostile ()
	{
		if (Vector3.Distance(transform.position, target.transform.position) >= 5f && !firebreath_cd_on && !PerformingSkydrop ()) {
			GameObject fireball = (GameObject)Instantiate (Resources.Load ("Fireball"));
			fireball.GetComponent<Fireball> ().Initialize (transform.GetChild (0).GetChild (0).position, target.transform.position);
			firebreath_cd_on = true;
			StartCoroutine (WaitAndEnableFirebreath ());
		} else if (!buttattack_cd_on) {
			buttattack_cd_on = true;
			StartCoroutine (WaitAndStartSkydrop ());
		}
	}

	IEnumerator WaitAndEnableFirebreath() {
		yield return new WaitForSeconds (firebreath_cd);
		firebreath_cd_on = false;
	}

	// ==========================Butt Attack==========================
	public bool rising = false;
	public bool hovering = false;
	public bool dropping = false;
	public bool buttattack_cd_on = false;
	bool do_dropdamage = false;
	float buttattack_cd = 4f;
	Vector3 dy = new Vector3 (0f, 1f, 0);
	float max_height;

	public override void performRunning() {
		if (rising) {
			transform.position += dy;
			if (transform.position.y > max_height) {
				rising = false;
				hovering = true;
			}
		} else if (hovering) {
			StartCoroutine (WaitAndDrop ());
			hovering = false;
			dropping = true;
		} else if (dropping) {
			if (WorldContainer.isAboveGround(transform.position, y_extent)) {
				dropping = false;
				do_dropdamage = true;
				GameObject particles = (GameObject) Instantiate (Resources.Load ("TorchickSkydrop"));
				particles.transform.position = new Vector3 (transform.position.x, 0.1f, transform.position.z);
				state = AnimalState.HOSTILE;
				StartCoroutine (WaitAndDisableDropDamage ());
			}
		}
	}

	bool PerformingSkydrop() {
		return rising || hovering || dropping;
	}

	protected void Rise() {
		max_height = transform.position.y + 10f;
		rising = true;
		rb.isKinematic = true;
		buttattack_cd_on = true;
		anim.SetBool ("fly", true);
	}

	IEnumerator WaitAndStartSkydrop() {
		yield return new WaitForSeconds (1.5f);
		if (Vector3.Distance (target.transform.position, transform.position) <= 5f) {
			state = AnimalState.RUNNING;
			invincible = true;
			Rise ();
		} else buttattack_cd_on = false;
	}

	IEnumerator WaitAndDrop() {
		yield return new WaitForSeconds (1f);
		anim.SetBool ("fly", false);
		rb.isKinematic = false;
		StartCoroutine (WaitAndEnableSkydrop ());
	}

	IEnumerator WaitAndEnableSkydrop() {
		yield return new WaitForSeconds (buttattack_cd);
		buttattack_cd_on = false;
	}

	IEnumerator WaitAndDisableDropDamage() {
		yield return new WaitForSeconds (Time.deltaTime);
		do_dropdamage = false;
		invincible = false;
	}
	// ===============================================================

	protected override IEnumerator WaitAndUnstun(float length) {
		yield return new WaitForSeconds (length);
		anim.SetBool ("stunned", false);
		stunned = false;
	}
}
