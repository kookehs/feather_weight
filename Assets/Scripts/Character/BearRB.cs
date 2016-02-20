using UnityEngine;
using System.Collections;

public enum BearState
{
	UNAWARE,
	HOSTILE,
	FRIENDLY,
	GUARDING,
	RUNNING
}

public class BearRB : MonoBehaviour {

	public BearState state = BearState.UNAWARE;
	public GameObject player;
	public GameObject scenarioController;
	public GameObject target;
	public GameObject guard;
	public bool isPlayerNear;
	public float friendliness;
	private Vector3 forward;
	private Vector3 desiredAngle;
	float turnTimer;
	private float runTime = 0f;
	public float powerUp = 1f;
	private bool checkPower = true;
	public int powerStrikes = 3;
	public float addSpeed = 250f;
	public float maxSpeed = 400f;
	public float rotateBy = 420f;
	Rigidbody rb;
	public GameObject blood;
	public GameObject cub;

	public AudioClip growl;
	AudioSource audio;

	//	Stun and stun timer
	private bool stunned = false;
	private float stunTime;
	public float stunLength = 1f;

	// Use this for initialization
	void Start ()
	{
		forward = transform.forward;
		desiredAngle = -forward;
		player = GameObject.Find ("Player");
		target = null;
		isPlayerNear = false;
		friendliness = 0f;
		turnTimer = 2f;
		rb = GetComponent<Rigidbody> ();
		audio = GetComponent<AudioSource>();

	}

	// Update is called once per frame
	void Update ()
	{
		switch (state) {
		case BearState.HOSTILE:
			// Debug.Log (">:(");
			faceTarget (target);
			if (!stunned) {
				//	Perform movement function by capturing input
				moveToward (target);
			} else {
				if (Time.time - stunTime >= stunLength)
					stunned = false;
			}
			if (friendliness > 0)
				state = BearState.FRIENDLY;
			if (GetComponent<Health> ().health <= 20) {
				state = BearState.RUNNING;
			}
			break;
		case BearState.FRIENDLY:
			if (friendliness <= 0)
				state = BearState.HOSTILE;
			break;
		case BearState.UNAWARE:
			turnTimer -= Time.deltaTime;
			float randomNum = Random.Range (0, 100);
			//	The turn timer triggers a new time being set, and a new desired angle
			if (turnTimer <= 0f) {
				turnTimer = Random.Range (2, 4);
				desiredAngle = -desiredAngle;
			}
			//	I will appear to be looking around arbitrarily
			Vector3 lookAround = Vector3.RotateTowards (transform.forward, desiredAngle, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
			if (lookAround != Vector3.zero)
				transform.rotation = Quaternion.LookRotation (lookAround);

			//Vector3 lookAround = Vector3.RotateTowards (transform.forward, -transform.forward, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
			//if (lookAround != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookAround);

			//	If the player goes near me I will change states
			if (Vector3.Distance (player.transform.position, transform.position) < 5f) {
				if (friendliness > 0) {
					target = player;
					state = BearState.FRIENDLY;
				} else if (friendliness <= 0) {
					target = player;
					state = BearState.HOSTILE;
				}
			}
			break;
		case BearState.GUARDING:
			if (Vector3.Distance (guard.transform.position, transform.position) > 10f) {
				moveToward (guard);
				faceTarget (guard);
			} else {
				state = BearState.UNAWARE;
			}
			break;
		case BearState.RUNNING:
			if (runTime < 150f){
				runTime += 1f;
                                target = player;
				faceAwayTarget (target);
				if (!stunned) {
					addSpeed *= -1;
					moveToward(target);
					addSpeed *= -1;
				}
				else {
					if (Time.time - stunTime >= stunLength) stunned = false;
				}
				if (friendliness > 0)
					state = BearState.FRIENDLY;
			}else {
				runTime = 0f;
				state = BearState.UNAWARE;
			}
			break;
		}
	}

	void faceTarget (GameObject target)
	{
		if (transform.position.x < target.transform.position.x) {
			desiredAngle = forward;
		} else
			desiredAngle = -forward;
		Vector3 faceTarget = Vector3.RotateTowards (transform.forward, desiredAngle, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
		if (faceTarget != Vector3.zero)
			transform.rotation = Quaternion.LookRotation (faceTarget);
	}

	void faceAwayTarget (GameObject target)
	{
		if (transform.position.x > target.transform.position.x) {
			desiredAngle = forward;
		} else
			desiredAngle = -forward;
		Vector3 faceTarget = Vector3.RotateTowards (transform.forward, desiredAngle, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
		if (faceTarget != Vector3.zero)
			transform.rotation = Quaternion.LookRotation (faceTarget);
	}

	private void moveToward(GameObject target){
		Vector3 targetDirection = target.transform.position - transform.position;
		targetDirection = Vector3.Normalize (targetDirection);
		targetDirection.y = 0;
		//	If we have hit top speed in a direction, cap off that force
		if (Mathf.Abs (rb.velocity.x) > maxSpeed) {
			targetDirection.x = 0;
		}
		if (Mathf.Abs (rb.velocity.y) > maxSpeed) {
			targetDirection.y = 0;
		}
		rb.AddForce (targetDirection * addSpeed);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.Equals (player)) {
			isPlayerNear = true;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.Equals (player)) {
			isPlayerNear = false;
		}
	}

	void OnCollisionEnter(Collision collision) {
		/*if (collision.collider.name.Equals ("EquipedWeapon")){

			foreach (ContactPoint contact in collision.contacts) {
				Instantiate (blood, contact.point, Quaternion.identity);
			}

			audio.PlayOneShot (growl);
			GetComponent<Health>().decreaseHealth ();
			Vector3 knockBackDirection = Vector3.Normalize (transform.position - collision.gameObject.transform.position);
			knockBackDirection.y = 1;
			rb.AddForce (knockBackDirection * 600 * powerUp);
			if (powerStrikes > 1) {
				powerStrikes -= 1;
				if (powerStrikes == 0)
					powerUp = 1f;
			}
			stunned = true;
			stunTime = Time.time;
			MakeHide ();
		}*/
		if (collision.collider.tag.Equals ("Player")) {
			collision.gameObject.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider>(), 10, 1000);
		}
	}

	public bool receiveHit (Collider other, float damage, float knockBackForce) {
		audio.PlayOneShot (growl);
		GetComponent<Health>().decreaseHealth ();
		Vector3 knockBackDirection = Vector3.Normalize (transform.position - other.transform.position);

		knockBackDirection.y = 1;
		rb.AddForce (knockBackDirection * 600 * powerUp);

		if (powerStrikes > 1) {
			powerStrikes -= 1;
			if (powerStrikes == 0)
				powerUp = 1f;
		}

		stunned = true;
		stunTime = Time.time;
		Debug.Log ("Dead");
		if (GetComponent<Health> ().isDead ()) {
			DropHide ();
			DropMeat ();
			DropTeeth ();
		}

		return GetComponent<Health> ().isDead ();
	}

	public void increaseFriendliness ()
	{
		friendliness += 1;
	}

	public void decreaseFriendliness ()
	{
		audio.PlayOneShot (growl);
		friendliness -= 1;
	}

	public void makeCub() {
		Instantiate (cub, transform.position, Quaternion.identity);
	}

	public void DropHide(){
		Instantiate (Resources.Load("Hide"), new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), transform.rotation);
	}

	public void DropTeeth(){
		Instantiate (Resources.Load ("Teeth"), new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), transform.rotation);
	}

	public void DropMeat(){
		Instantiate (Resources.Load("Raw_Meat"), new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), transform.rotation);
	}

	public void setGuard (GameObject g)
	{
		guard = g;
		state = BearState.GUARDING;
	}

	public void setRun()
	{
		runTime = 0f;
		state = BearState.RUNNING;
	}

	public void rage ()
	{
		if (checkPower) {
			checkPower = false;
			powerUp = 1.5f;
			powerStrikes = 3;
		}
	}
}
