using UnityEngine;
using System.Collections;

public class Torchick : Animal
{
	float firebreath_cd = 5f; // cooldown time
	bool  firebreath_cd_on = true;
	string _hex_loc;

	float y_extent;

	static readonly double BASEMOVECHANCE = 0.100001;
	double move_chance = BASEMOVECHANCE;

	public string hex_loc {
		get { return _hex_loc;  }
		set { _hex_loc = value; }
	}

	protected override void BeforeHit(string hitter) {
		move_chance += 0.01f;
	}

	protected override bool DamagePlayerOnCollision() {
		return true;
	}

	protected override void Initialize() {
		target = player;
		name = "Torchick";
		PhysicsOn ();
		StartCoroutine (WaitAndEnableFirebreath ());
		anim = GetComponentInChildren<Animator> ();
		y_extent = GetComponent<Collider> ().bounds.extents.y;

	}

	void OnDestroy() {
		StopAllCoroutines();
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Player") || other.CompareTag ("Bear") || other.CompareTag ("Wolf")) {
			move_chance += 0.01f;
		}
	}

	void OnTriggerStay(Collider other) {
		if (do_dropdamage) {
			if (other.CompareTag("Chicken")) return;
			other.GetComponent<Strikeable> ().receiveHit (GetComponent<Collider> (), 10f, 1000f, "ChickenButt"); //TODO: different damage to different creatures later
		}else if (other.CompareTag ("Player") || other.CompareTag ("Bear") || other.CompareTag ("Wolf")) {
			move_chance += 0.0001f;
		}
	}

	public override void performStateCheck() {
		if (!buttattack_cd_on && WorldContainer.RandomChance () < move_chance) {
			state = AnimalState.RUNNING;
			Rise ();
			if (move_chance > BASEMOVECHANCE)
				move_chance = BASEMOVECHANCE;
			else
				move_chance -= 0.05f;
		} else if (!PerformingSkydrop()){
			state = AnimalState.HOSTILE;
		}
	}

	public override void performHostile ()
	{
		if (!firebreath_cd_on) {
			//int hex_index = WorldContainer.RandomChance (WorldContainer.hexes.Length);
			//GameObject hex = GameObject.Find(WorldContainer.hexes[WorldContainer.RandomChance(WorldContainer.hexes.Length)]);
			GameObject fireball = (GameObject) Instantiate (Resources.Load ("Fireball"));
			fireball.GetComponent<Fireball> ().Initialize (transform.GetChild(0).GetChild(0).position, target.transform.position);
			firebreath_cd_on = true;
			StartCoroutine (WaitAndEnableFirebreath ());
		}
	}

	IEnumerator WaitAndEnableFirebreath() {
		yield return new WaitForSeconds (firebreath_cd);
		firebreath_cd_on = false;
	}

	// ==========================Butt Attack==========================
	bool rising = false;
	bool hovering = false;
	bool dropping = false;
	bool buttattack_cd_on = false;
	bool do_dropdamage = false;
	float buttattack_cd = 4f;
	Vector3 dy = new Vector3 (0f, 1f, 0);

	public override void performRunning() {
		if (rising) {
			transform.position += dy;
			if (transform.position.y > 7f) {
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
		rising = true;
		rb.isKinematic = true;
		buttattack_cd_on = true;
		anim.SetBool ("fly", true);
	}

	IEnumerator WaitAndDrop() {
		yield return new WaitForSeconds (2f);
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
	}
	// ===============================================================
}