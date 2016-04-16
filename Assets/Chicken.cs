using UnityEngine;
using System.Collections;

public class Chicken : Animal {

	public bool pickupStunned = false;
	public Collection iAmCollectable;
	public GameObject featherPoof;
	public bool crazed = false;
	public bool crazyHopCoolDown = false;

	public bool secondaryStunned = false;
	public float secondaryStunTime;
	public float secondaryStunLength = 1f;

	Animator a;

        public void Name(string name) {
                gameObject.name = name;
        }

	public override void Start() {
		//	Perform Start() as specified in the parent class (Animal.cs)
		base.Start ();
		iAmCollectable.enabled = false;
		a = GetComponentInChildren<Animator> ();
	}

	// Update is called once per frame
	public void Update ()
	{
		if (player == null) return;

		if (Input.GetKeyDown (KeyCode.J)) {
			Craze ();
		}

		performStateCheck ();
		//If not stunned, let's examine the state and do something
		if (!stunned) {
			switch (state) {
			case AnimalState.HOSTILE:
				performHostile ();
				break;
			case AnimalState.FRIENDLY:
				performFriendly ();
				break;
			case AnimalState.UNAWARE:
				performUnaware ();
				break;
			case AnimalState.GUARDING:
				performGuarding ();
				break;
			case AnimalState.RUNNING:
				// Debug.Log ("Run func called");
				performRunning ();
				break;
			}
		}
		else {
			if (Time.time - stun_time >= stunLength) {
				physicsOff ();
				Unstun ();
			}
		}
		if (invincible) {
			if (Time.time - invincible_time >= invincible_length)
				invincible = false;
		}
		if (crazed && !crazyHopCoolDown) {
			CrazyHop ();
		}
	}

	public override void performStateCheck(){
		if (friendliness > 0) {
			if (state == AnimalState.RUNNING) {
				if (runTime < 150f) {
					runTime += Time.deltaTime;
				} else {
					runTime = 0;
					state = AnimalState.UNAWARE;
				}
			}else if (Vector3.Distance (player.transform.position, transform.position) < 10f) {
				state = AnimalState.RUNNING;
				target = player;
			} else {
				state = AnimalState.UNAWARE;
			}
		} else {
			state = AnimalState.HOSTILE;
		}
	}

	protected override void Initialize(){

	}

	//	Note: This function will be called from the grandparent class (Strikeable.cs)
	//	Precondition: Chicken receives hit from some sharp object, such as the player's weapon.
	//	Postcondition: Stun juice is displayed. There is a brief cooldown after which the chicken can be collected.
	protected override void Stun(float length) {
		//	Perform Stun() as specified in the grandparent class (Strikeable.cs)
		base.Stun (length);
		Instantiate (featherPoof, transform.position, Quaternion.identity);
		GetComponent<AudioSource> ().Play ();
		StartCoroutine (WaitAndEnableCollection ());
	}

	//	Note: This function is called in the Start() of the parent class (Animal.cs)
	//	Precondition: Stun() has been called and 'stunLength' seconds have passed (defined in the parent class, Animal.cs)
	//	Postcondition: The chicken is now collectable. The secondary cooldown begins, after which it will no longer be collectable.
	/*protected override void Unstun(){
		//	Perform Unstun() as specified in the parent class (Strikeable.cs)
		base.Unstun ();
		secondaryStun ();
	}*/

	public IEnumerator WaitAndEnableCollection(){
		yield return new WaitForSeconds(.25f);
		iAmCollectable.enabled = true;
		a.SetBool ("stunned", true);
		StartCoroutine (WaitAndDisableCollection());
	}

	public IEnumerator WaitAndDisableCollection(){
		yield return new WaitForSeconds (1f);
		iAmCollectable.enabled = false;
		a.SetBool ("stunned", false);
	}

	//	Below are functions related to what Twitch can do to these chickens.

	public void DoubleSpeed() {
		addSpeed *= 2;
		StartCoroutine (WaitAndEndDoubleSpeed ());
	}

	public IEnumerator WaitAndEndDoubleSpeed(){
		yield return new WaitForSeconds (5f);
		addSpeed /= 2;
	}

	public void Craze(){
		crazed = true;
		StartCoroutine (WaitAndEndCraze ());
	}

	public void CrazyHop(){
		physicsOn ();
		int randomX = WorldContainer.RandomChance(200,600);
		int randomY = WorldContainer.RandomChance(500,750);
		int randomZ = WorldContainer.RandomChance (200, 600);
		if (randomX % 2 == 0) randomX = -randomX;
		if (randomZ % 2 == 0) randomZ = -randomZ;
		Vector3 randomForce = new Vector3 (randomX, randomY, randomZ);
		rb.AddForce (randomForce);
		crazyHopCoolDown = true;
		StartCoroutine (CrazyHopCoolDown ());
	}

	public IEnumerator CrazyHopCoolDown(){
		yield return new WaitForSeconds((float) WorldContainer.RandomChance());
		crazyHopCoolDown = false;
	}

	public IEnumerator WaitAndEndCraze(){
		yield return new WaitForSeconds (10f);
		crazed = false;
	}

	//	The below functions are no longer in use:

	public void secondaryStun() {
		secondaryStunned = true;
		secondaryStunTime = Time.time;
		iAmCollectable.enabled = true;
		a.SetBool ("stunned", true);
	}

	public void secondaryUnstun() {
		iAmCollectable.enabled = false;
		a.SetBool ("stunned", false);
		secondaryStunned = false;
	}

}
