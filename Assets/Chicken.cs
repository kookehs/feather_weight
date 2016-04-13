using UnityEngine;
using System.Collections;

public class Chicken : Animal {

	public bool pickupStunned = false;
	public Collection iAmCollectable;
	public GameObject featherPoof;

        public void Name(string name) {
                gameObject.name = name;
        }

	public override void Start() {
		//	Perform Start() as specified in the parent class (Animal.cs)
		base.Start ();
		iAmCollectable = GetComponent<Collection> ();
		iAmCollectable.enabled = false;
		//Unstun ();
	}

	public override void performStateCheck(){
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

	protected override void Stun(float length) {
		//	Perform Stun() as specified in the parent class (Strikeable.cs)
		base.Stun (length);
		Instantiate (featherPoof, transform.position, Quaternion.identity);
		iAmCollectable.enabled = true;
	}

	//	It is worth noting that this function is called automatically in Start() of Animal.cs
	protected override void Unstun(){
		//	Perform Unstun() as specified in the parent class (Strikeable.cs)
		base.Unstun ();
		iAmCollectable.enabled = false;
	}

}
