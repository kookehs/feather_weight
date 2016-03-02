using UnityEngine;
using System.Collections;

public enum AnimalState
{
	UNAWARE,
	HOSTILE,
	FRIENDLY,
	GUARDING,
	RUNNING
}

//
//	Animal.cs provides baseline functionality for animal NPCs. It is
//	abstract and therefore meant to be inherited, not directly used.
//

[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Rigidbody))]
public abstract class Animal : Strikeable
{

	//	Set initial state
	public AnimalState state = AnimalState.UNAWARE;

	//	Should be aware of player and scenarioController
	public GameObject player;
	public GameObject scenarioController;

	//	The following are variables that affect state
	public GameObject target;
	public GameObject guard;
	public bool isPlayerNear;
	public float friendliness;

	//	The following are variables used for rotating
	private Vector3 forward;
	private Vector3 desiredAngle;
	float turnTimer;

	protected float runTime = 0f;
	public float powerUp = 1f;
	private bool checkPower = true;
	public int powerStrikes = 3;
	public float addSpeed = 100f;
	public float maxSpeed = 5f;
	public float rotateBy = 420f;
	public GameObject blood;

	public AudioClip growl;
	protected AudioSource audio;

	//	Stun and stun timer
	public float stunLength = 1f;

	public NavMeshAgent nma;

	// Use this for initialization
	void Start ()
	{
		nma = GetComponent<NavMeshAgent> ();

		nma.autoTraverseOffMeshLink = true;

		forward = transform.forward;
		desiredAngle = -forward;
		player = GameObject.Find ("Player");
		target = null;
		isPlayerNear = false;
		friendliness = 0f;
		turnTimer = 2f;
		rb = GetComponent<Rigidbody> ();
		audio = GetComponent<AudioSource> ();
		stunned = false;

		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
		Initialize ();
	}

	// Update is called once per frame
	void Update ()
	{
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
				Debug.Log ("Run func called");
				performRunning ();
				break;
			}
		}
		else {
			if (Time.time - stunTime >= stunLength) {
				physicsOff ();
				stunned = false;
			}
		}
	}

	//	performStateCheck() must be overridden by child classes,
	//	and specifies when transitions between states should occur.
	public abstract void performStateCheck();
	protected abstract void Initialize();

	void forceStateChange(AnimalState newState){
		state = newState;
	}

	public virtual void performHostile(){

		//faceTarget (target);
		physicsOff();
		nma.SetDestination (target.transform.position);

		//	If we encounter an offmesh link...
		if (nma.autoTraverseOffMeshLink == false && nma.isOnOffMeshLink) {
			Debug.Log ("OFFMESHLINK");
			Vector3 targetPos = nma.nextOffMeshLinkData.endPos;
			Vector3 targetDir = targetPos - transform.position;
			Debug.Log ("TargetDir" + targetDir);

			//The y value of this vector will reflect the height we want for the jump
			Vector3 jumpForce = new Vector3 (targetDir.x, 100, targetDir.z);

			physicsOn ();
			rb.AddForce (jumpForce);
			//stunned = true;
			//stunTime = Time.time;
			physicsOff ();

		}

	}

	public virtual void performUnaware(){
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
	}

	public virtual void performRunning(){
		physicsOn ();
		if (runTime < 500f) {
			runTime += 1f;
			faceAwayTarget (target);
			addSpeed *= -1;
			moveToward(target);
			addSpeed *= -1;
			if (friendliness > 0)
				state = AnimalState.FRIENDLY;
		} else {
			runTime = 0f;
			state = AnimalState.UNAWARE;
		}

	}

	public virtual void performFriendly(){

	}

	public virtual void performGuarding(){
		if (Vector3.Distance (guard.transform.position, transform.position) > 10f) {
			moveToward (guard);
			faceTarget (guard);
		} else {
			state = AnimalState.UNAWARE;
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

	protected void moveToward (GameObject target)
	{
		//	Then, once it runs out of path, use the method below:

		//	Determine the direction to the target, normalize it, and discard y
		Vector3 targetDirection = target.transform.position - transform.position;
		targetDirection = Vector3.Normalize (targetDirection);
		targetDirection.y = 0;

		//	If we have hit top speed in a direction, cap off that force
		if (Mathf.Abs (rb.velocity.x) > maxSpeed) {
			targetDirection.x = 0;
		}
		if (Mathf.Abs (rb.velocity.z) > maxSpeed) {
			targetDirection.z = 0;
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

	void OnCollisionEnter (Collision collision)
	{
		if (collision.collider.tag.Equals ("Player")) {
			collision.gameObject.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), 10, 1000);
		}
	}

	protected override void BeforeHit() {
		physicsOn();
	}

	/*public virtual bool receiveHit (Collider other, float damage, float knockBackForce)
	{
		physicsOn ();
		audio.PlayOneShot (growl);
		GetComponent<Health> ().decreaseHealth (damage);
		Vector3 knockBackDirection = Vector3.Normalize (transform.position - other.transform.position);
		knockBackDirection.y = 1;
		rb.AddForce (knockBackDirection * knockBackForce * powerUp);
		if (powerStrikes > 1) {
			powerStrikes -= 1;
			if (powerStrikes == 0)
				powerUp = 1f;
		}
		stunned = true;
		stunTime = Time.time;

		if (GetComponent<Health> ().isDead ()) {
			MakeHide ();
			MakeTeeth ();
			MakeMeat ();
		}

		return GetComponent<Health> ().isDead ();
	}*/

	protected void physicsOff() {
		rb.isKinematic = true;
		if (nma != null) nma.enabled = true;
	}

	protected void physicsOn() {
		rb.isKinematic = false;
		if (nma != null)
			nma.enabled = false;
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

	public void setGuard (GameObject g)
	{
		guard = g;
		state = AnimalState.GUARDING;
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