using UnityEngine;
using System.Collections;

public class Chicken : Animal
{

	public bool pickupStunned = false;
	public Collection iAmCollectable;
	public GameObject featherPoof;
	public bool crazed = false;
	public bool crazyHopCoolDown = false;
	public Behaviour halo;

	public bool secondaryStunned = false;
	public float secondaryStunTime;
	public float secondaryStunLength = 1f;

	public AudioClip cluck;
	public AudioSource aSrc;
	public AudioClip sound_on_strike2;
	public AudioClip sound_on_strike3;
	public AudioClip secret_sound_on_strike;

	public float seeDistance;

	// For wandering mechanics
	static float WANDERCIRCLE_DISTANCE = 10f;
	static float WANDERCIRCLE_RADIUS = 5f * Mathf.Deg2Rad;
	static float WANDERANGLE_CHANGE = 5f * Mathf.Deg2Rad;
	float wander_angle = 30f * Mathf.Deg2Rad;
	float max_speed = 5f;
	static double WANDER_CHANCE = 0.2;
	float wander_duration = 5f;
	bool is_wandering = false;

	// For running mechanics
	int jumps = 3;
	static readonly float BASE_JUMP_REFRESH = 3f;
	float jump_refresh_timer = BASE_JUMP_REFRESH;
	float y_extent;

	Animator a;

        public bool quest_eligible = true;
	int hide_chance = 30;

	void
    OnCollisionEnter (Collision collider)
	{
		if (collider.gameObject.tag == "Bush") {
			int rand = WorldContainer.RandomChance (100);

			if (rand < hide_chance) {
				collider.gameObject.GetComponent<Destroyable> ().HideChicken (this.gameObject);
			}
		}
	}

	public override void Start ()
	{
		//	Perform Start() as specified in the parent class (Animal.cs)
		base.Start ();
		iAmCollectable.enabled = false;
		a = GetComponentInChildren<Animator> ();
		InvokeRepeating ("TriggerWander", 1f, 1f);
		y_extent = GetComponent<Collider> ().bounds.extents.y;
		//InvokeRepeating ("Stun", 5f, 5f);
	}

	void OnDestroy ()
	{
		CancelInvoke ();
	}

	void printAMessage ()
	{
		Debug.Log ("A Message");
	}

	protected override void ChildUpdate ()
	{
		if (transform.position.y < -10) {
			Pop ();
			ChickenSpawner.DecreaseCount ();
		}
		if (crazed && !crazyHopCoolDown)
			CrazyHop ();
		SetSprite ();
	}

	void SetSprite ()
	{
		//TODO: this need to be smarter; smoother transition
		if (rb.velocity.x < 0)
			transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
		else if (rb.velocity.x > 0)
			transform.GetChild (0).rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
		if (WorldContainer.isAboveGround (transform.position, y_extent)) {
			anim.SetBool ("fly", false);
		}
	}

	public bool IsPickupStunned ()
	{
		return pickupStunned;
	}

	public override void performStateCheck ()
	{
		if (Vector3.Distance (player.transform.position, transform.position) < seeDistance) {
			state = AnimalState.RUNNING;
			target = player;
		} else {
			state = AnimalState.UNAWARE;
		}
	}

	public override void performRunning ()
	{
		jump_refresh_timer -= Time.deltaTime;
		if (jump_refresh_timer <= 0) {
			jump_refresh_timer = BASE_JUMP_REFRESH;
			jumps = 3;
		}
		if (Vector3.Distance (transform.position, player.transform.position) < 3f) {
			if (WorldContainer.isAboveGround (transform.position, y_extent) && jumps > 0) {
				anim.SetBool ("fly", true);
				Vector3 run_vector = player.transform.position - transform.position;
				run_vector.y = 0;
				run_vector.Normalize ();

				float angle = (float)(WorldContainer.RandomChance () - 0.5) * 60f;
				run_vector.x += Mathf.Cos (angle) * 3;
				run_vector.z += Mathf.Sin (angle) * 3;
				run_vector.y = 5f;
				rb.AddForce (run_vector * 200f);
				JustJumped ();
			}
		}
	}

	void JustJumped ()
	{
		jump_refresh_timer = BASE_JUMP_REFRESH;
		--jumps;
	}
	// -------------------------------------------------------------------------------------------------------------------------
	// UNWARE + WANDERING
	public override void performUnaware ()
	{
		if (is_wandering) {
			Vector3 steering = Wander ();
			steering = WorldContainer.Truncate (steering, max_force);
			steering = steering / 2f;
			rb.velocity = WorldContainer.Truncate (rb.velocity + steering, max_speed);
		}
	}

	void TriggerWander ()
	{
		if (state == AnimalState.UNAWARE && !is_wandering && WorldContainer.RandomChance () < WANDER_CHANCE)
			StartCoroutine (WaitAndStartWandering ());
	}

	Vector3 Wander ()
	{
		Vector3 circle_center = rb.velocity + Vector3.zero;
		circle_center.Normalize ();
		circle_center *= WANDERCIRCLE_DISTANCE;

		Vector3 displacement = new Vector3 (0, 0, -1);
		displacement *= WANDERCIRCLE_RADIUS;

		float length = displacement.magnitude + 0f;
		displacement.x = Mathf.Cos (wander_angle) * length;
		displacement.z = Mathf.Sin (wander_angle) * length;
		wander_angle += (float)(WorldContainer.RandomChance () - 0.51) * WANDERANGLE_CHANGE;

		Vector3 wander_force = circle_center + displacement;
		return wander_force;
	}

	public IEnumerator WaitAndStartWandering ()
	{
		yield return new WaitForSeconds (3f);
		is_wandering = true;
		StartCoroutine (WaitAndStopWandering ());
	}

	public IEnumerator WaitAndStopWandering ()
	{
		yield return new WaitForSeconds (wander_duration);
		rb.velocity = Vector3.zero;
		wander_duration = 3f + (float)WorldContainer.RandomChance () * 4f;
		is_wandering = false;
	}
	// -------------------------------------------------------------------------------------------------------------------------

	protected override void Initialize ()
	{
		gameObject.layer = LayerMask.NameToLayer ("Chicken");
		PhysicsOn ();
	}

	//	Note: This function will be called from the grandparent class (Strikeable.cs)
	//	Precondition: Chicken receives hit from some sharp object, such as the player's weapon.
	//	Postcondition: Stun juice is displayed. There is a brief cooldown after which the chicken can be collected.
	protected override void Stun (float length)
	{
		//	Perform Stun() as specified in the grandparent class (Strikeable.cs)
		base.Stun (length);
		Instantiate (featherPoof, transform.position, Quaternion.identity);
		StartCoroutine (WaitAndEnableCollection ());
	}

	protected override bool DamagePlayerOnCollision ()
	{
		return false;
	}


	//	Note: This function is called in the Start() of the parent class (Animal.cs)
	//	Precondition: Stun() has been called and 'stunLength' seconds have passed (defined in the parent class, Animal.cs)
	//	Postcondition: The chicken is now collectable. The secondary cooldown begins, after which it will no longer be collectable.
	/*protected override void Unstun(){
		//	Perform Unstun() as specified in the parent class (Strikeable.cs)
		base.Unstun ();
		secondaryStun ();
	}*/

	public IEnumerator WaitAndEnableCollection ()
	{
		yield return new WaitForSeconds (.25f);
		pickupStunned = true;
		iAmCollectable.enabled = true;
		a.SetBool ("stunned", true);
		StartCoroutine (WaitAndDisableCollection ());
	}

	public IEnumerator WaitAndDisableCollection ()
	{
		yield return new WaitForSeconds (1f);
		pickupStunned = false;
		iAmCollectable.enabled = false;
		a.SetBool ("stunned", false);
	}

	//	Below are functions related to what Twitch can do to these chickens.

	public void DoubleSpeed ()
	{
		addSpeed *= 2;
		StartCoroutine (WaitAndEndDoubleSpeed ());
	}

	public IEnumerator WaitAndEndDoubleSpeed ()
	{
		yield return new WaitForSeconds (5f);
		addSpeed /= 2;
	}

	public void Craze ()
	{
		crazed = true;
		StartCoroutine (WaitAndEndCraze ());
	}

	public void CrazyHop ()
	{
		PhysicsOn ();
		int randomX = WorldContainer.RandomChance (200, 600);
		int randomY = WorldContainer.RandomChance (500, 750);
		int randomZ = WorldContainer.RandomChance (200, 600);
		if (randomX % 2 == 0)
			randomX = -randomX;
		if (randomZ % 2 == 0)
			randomZ = -randomZ;
		Vector3 randomForce = new Vector3 (randomX, randomY, randomZ);
		rb.AddForce (randomForce);
		crazyHopCoolDown = true;
		StartCoroutine (CrazyHopCoolDown ());
	}

	public IEnumerator CrazyHopCoolDown ()
	{
		yield return new WaitForSeconds ((float)WorldContainer.RandomChance ());
		crazyHopCoolDown = false;
	}

	public IEnumerator WaitAndEndCraze ()
	{
		yield return new WaitForSeconds (10f);
		crazed = false;
	}

	public void Shrink ()
	{
		transform.localScale *= .5f;
		StartCoroutine (WaitAndEndShrink ());
	}

	public IEnumerator WaitAndEndShrink ()
	{
		yield return new WaitForSeconds (10f);
		transform.localScale *= 2;
	}

	protected override IEnumerator WaitAndUnstun (float length)
	{
		yield return new WaitForSeconds (length);
		stunned = false;
	}

	public override IEnumerator WaitAndRemove ()
	{
		yield return new WaitForSeconds (.5f);
		Pop ();
	}

	public void Pop ()
	{
		Instantiate (Resources.Load ("FeatherPop"), transform.position, Quaternion.identity);
		WorldContainer.Remove (gameObject);
	}

	public void NmaPerformRunning ()
	{
		GameObject farthestNodeFromPlayer = null;
		float distance = 0;
		foreach (GameObject n in GameObject.FindGameObjectsWithTag("Node")) {
			float nDistanceFromPlayer = Vector3.Distance (player.transform.position, n.transform.position);
			if (nDistanceFromPlayer > distance) {
				distance = nDistanceFromPlayer;
				farthestNodeFromPlayer = n;
			}
		}
		if (farthestNodeFromPlayer != null)
			nma.SetDestination (farthestNodeFromPlayer.transform.position);
	}

	override protected void OnMouseEnter ()
	{
		if (GetComponent<Collection> ().enabled == true)
			Camera.main.GetComponent<CollectionCursor> ().SetHover ();
		else
			Camera.main.GetComponent<CollectionCursor> ().SetWeapon ();
	}

	protected void OnMouseOver ()
	{
		if (GetComponent<Collection> ().enabled == true)
			Camera.main.GetComponent<CollectionCursor> ().SetHover ();
		else {
			Camera.main.GetComponent<CollectionCursor> ().SetWeapon ();
		}
	}

	public override void PlaySound ()
	{
		int rollDice = WorldContainer.RandomChance (1, 1000);
		if (rollDice < 333)
			GetComponent<AudioSource> ().PlayOneShot (sound_on_strike);
		else if (rollDice >= 333 && rollDice < 666)
			GetComponent<AudioSource> ().PlayOneShot (sound_on_strike2);
		else if (rollDice >= 666 && rollDice <= 999)
			GetComponent<AudioSource> ().PlayOneShot (sound_on_strike3);
		else
			GetComponent<AudioSource> ().PlayOneShot (secret_sound_on_strike);
	}
}
