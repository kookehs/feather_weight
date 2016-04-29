using UnityEngine;
using System.Collections;

public enum AnimalState
{
	UNAWARE,
	HOSTILE,
	FRIENDLY,
	GUARDING,
	RUNNING,
	NULL
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
	public bool doesExist = true;
	public float friendliness = 0f;

	//	The following are variables used for rotating
	private Vector3 forward;
	private Vector3 desiredAngle;
	float turnTimer;

	protected float runTime = 0f;
	public float addSpeed = 100f;
	public float maxSpeed = 5f;
	public float rotateBy = 420f;

	Health health;
	public float base_damage = 10f,
	             base_knockback = 500f,
	             power = 1f,
	             rage_duration = 3f;
	public GameObject blood;

	//	Stun and stun timer
	public float stunLength = 1f;

	public NavMeshAgent nma;

	protected static LayerMask the_ground = 1 << LayerMask.NameToLayer("Ground");

	void Awake() {
		rb = GetComponent<Rigidbody> ();
		health = GetComponent<Health> ();
	}

	// Use this for initialization
	public virtual void Start ()
	{

		nma = GetComponent<NavMeshAgent> ();

		nma.autoTraverseOffMeshLink = true;

		gameObject.layer = LayerMask.NameToLayer ("Character");
		forward = transform.forward;
		desiredAngle = -forward;
		player = GameObject.Find ("Player");
		isPlayerNear = false;
		turnTimer = 2f;
		Unstun ();
	
		Initialize ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (player == null) return;

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
		if (invincible) {
			if (Time.time - invincible_time >= invincible_length)
				invincible = false;
		}
	}

	void OnApplicationQuit() {
		WorldContainer.UpdateUpdateList (tag);
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
		PhysicsOff();
		nma.SetDestination (target.transform.position);

		//	If we encounter an offmesh link...
		if (nma.autoTraverseOffMeshLink == false && nma.isOnOffMeshLink) {
			Vector3 targetPos = nma.nextOffMeshLinkData.endPos;
			Vector3 targetDir = targetPos - transform.position;

			//The y value of this vector will reflect the height we want for the jump
			Vector3 jumpForce = new Vector3 (targetDir.x, 100, targetDir.z);

			PhysicsOn ();
			rb.AddForce (jumpForce);
			PhysicsOff ();

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
		//Debug.Log ("Runnning away!");
		PhysicsOn ();
		if (runTime < 500f) {
			runTime += 1f;
			faceAwayTarget (target);
			addSpeed *= -1;
			moveToward(target);
			addSpeed *= -1;
		} else {
			runTime = 0f;
			state = AnimalState.UNAWARE;
		}

	}

	public virtual void performFriendly(){
		Debug.Log ("Friendly action.");
	}

	public virtual void performGuarding(){
		if (Vector3.Distance (guard.transform.position, transform.position) > 10f) {
			moveToward (guard);
			faceTarget (guard);
		} else {
			state = AnimalState.UNAWARE;
		}

	}

	public void changeTarget(GameObject t) {
		target = t;
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

	protected void faceAwayTarget (GameObject target)
	{
                if (target == null)
                    return;

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
                if (target == null)
                    return;
		//	Then, once it runs out of path, use the method below:

		//	Determine the direction to the target, normalize it, and discard y
		Vector3 targetDirection = target.transform.position - transform.position;
		targetDirection = Vector3.Normalize (targetDirection);
		targetDirection.y = 0;

		float velocity = Mathf.Sqrt (Mathf.Pow (rb.velocity.x, 2) + Mathf.Pow (rb.velocity.z, 2));
		//	If we have hit top speed in a direction, cap off that force
		Vector3 movement = Vector3.zero;
		if (Mathf.Abs (velocity) <= maxSpeed)
			movement = targetDirection * addSpeed;

		Vector3 previous_position = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
		if (isAboveGround (transform.position + movement/(addSpeed * 0.85f), Mathf.Infinity))
			rb.AddForce (targetDirection * addSpeed);
		else {
			rb.velocity = Vector3.zero;
			transform.position = previous_position;
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (player != null && other.gameObject.Equals (player)) {
			isPlayerNear = true;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (player != null && other.gameObject.Equals (player)) {
			isPlayerNear = false;
		}
	}

	protected virtual void OnCollisionStay (Collision collision)
	{
		if (collision.collider.tag.Equals ("Player") && DamagePlayerOnCollision()) {
			collision.gameObject.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), base_damage * power, base_knockback * power, tag);
		}
	}

	protected override IEnumerator WaitAndUnstun(float length) {
		yield return new WaitForSeconds (length);
		PhysicsOff ();
		stunned = false;
	}

	protected override void BeforeHit(string hitter) {
		PhysicsOn();
	}

	protected void PhysicsOff() {
		rb.isKinematic = true;
		if (nma != null) nma.enabled = true;
	}

	protected void PhysicsOn() {
		rb.isKinematic = false;
		if (nma != null) nma.enabled = false;
	}

	protected virtual bool DamagePlayerOnCollision() {
		return true;
	}

	public void increaseFriendliness ()
	{
		friendliness += 1;
	}

	public void increaseFriendliness (float d)
	{
		friendliness += d;
	}

	public void decreaseFriendliness ()
	{
		friendliness -= 1;
	}

	public void decreaseFriendliness (float d)
	{
		friendliness -= d;
	}

	public void setGuard (GameObject g)
	{
		guard = g;
		state = AnimalState.GUARDING;
	}

	public void Rage(string s) {
		Rage (1.5f, s);
		StartCoroutine (EndRage ());
	}

	protected virtual void Rage (float powerup, string s)
	{
		if (s.Equals("stronger") || s.Equals("all")) {
			power = powerup;
			GetComponent<Health>().Increase(100);
		}
		if (s.Equals("faster") || s.Equals("all")) {
			nma.speed = 5f;
		}

	}

	protected virtual void AfterRage () {
		power = 1f;
		nma.speed = 3.5f;
	}

	public IEnumerator EndRage() {
		yield return new WaitForSeconds (rage_duration);
		AfterRage ();
	}

	public void SkyDrop() {
		PhysicsOn ();
		StartCoroutine (WaitAndDeactivatePhysics ());
	}

	public IEnumerator WaitAndDeactivatePhysics() {
		yield return new WaitForSeconds(3f);
		PhysicsOff();
	}

	//	Precondition: Nothing
	//	Postcondition: The two directions that our animal can face are changed.
	//	Note: This will be called when the camera is rotated.
	public void updateForward(Vector3 newForward){
		forward = newForward;
		desiredAngle = -forward;
		transform.forward = forward;
	}

	protected bool isAboveGround(Vector3 p, float d) {
		return Physics.Raycast (p, Vector3.down, d, the_ground);
	}

}