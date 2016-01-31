using UnityEngine;
using System.Collections;

public class BearRB : MonoBehaviour {

	private BearState state = BearState.UNAWARE;
	public GameObject player;
	public GameObject scenarioController;
	public GameObject target;
	public bool isPlayerNear;
	public float friendliness;
	private Vector3 forward;
	private Vector3 desiredAngle;
	float turnTimer;
	public float addSpeed = 250f;
	public float maxSpeed = 400f;
	public float rotateBy = 420f;
	Rigidbody rb;

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
				moveToward(target);
			}
			else {
				if (Time.time - stunTime >= stunLength) stunned = false;
			}
			if (friendliness > 0)
				state = BearState.FRIENDLY;
			break;
		case BearState.FRIENDLY:
			// Debug.Log (":)");
			if (friendliness <= 0)
				state = BearState.HOSTILE;
			break;
		case BearState.UNAWARE:
			// Debug.Log ("-___-");
			turnTimer -= Time.deltaTime;
			float randomNum = Random.Range (0, 100);
			//Debug.Log (turnTimer);
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
		if (collision.collider.tag.Equals ("sword")){
			Vector3 knockBackDirection = Vector3.Normalize (transform.position - collision.gameObject.transform.position);
			knockBackDirection.y = 1;
			rb.AddForce (knockBackDirection * 600);
			stunned = true;
			stunTime = Time.time;
		}
	}
	
	public void increaseFriendliness ()
	{
		friendliness += 1;
	}
	
	public void decreaseFriendliness ()
	{
		friendliness += 1;
	}
}
