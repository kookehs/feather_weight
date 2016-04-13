using UnityEngine;
using System.Collections;

public class Chicken : Animal {

	public bool pickupStunned = false;
	public Collection iAmCollectable;
	public GameObject featherPoof;
	public Animator a;

	public bool secondaryStunned = false;
	public float secondaryStunTime;
	public float secondaryStunLength = 1f;

        public void Name(string name) {
                gameObject.name = name;
        }

	public override void Start() {
		//	Perform Start() as specified in the parent class (Animal.cs)
		base.Start ();
		iAmCollectable.enabled = false;
	}

	public override void performStateCheck(){
		if (secondaryStunned && Time.time - secondaryStunTime >= secondaryStunLength)
			secondaryUnstun ();
		//	If we are not running...
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
	}

	//	Note: This function is called in the Start() of the parent class (Animal.cs)
	//	Precondition: Stun() has been called and 'stunLength' seconds have passed (defined in the parent class, Animal.cs)
	//	Postcondition: The chicken is now collectable. The secondary cooldown begins, after which it will no longer be collectable.
	protected override void Unstun(){
		//	Perform Unstun() as specified in the parent class (Strikeable.cs)
		base.Unstun ();
		secondaryStun ();
	}

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
