using UnityEngine;
using System.Collections;

public class PlayerMovementRB : MonoBehaviour
{

	public Rigidbody rb;
	public float addSpeed = 250f;
	public float maxSpeed = 400f;
	private Vector3 rotateVec;
	public float rotateBy = 200f;

	//	Stun and stun timer
	private bool stunned = false;
	private float stunTime;
	public float stunLength = 1f;
	
	// Use this for initialization
	void Start ()
	{
		
		rb = GetComponent<Rigidbody> ();
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

		if (!stunned) {
			//	Perform movement function by capturing input
			DoMovement (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		}
		else {
			if (Time.time - stunTime >= stunLength) stunned = false;
		}

	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag.Equals ("bear")) {
			rb.velocity = Vector3.zero;
			Vector3 knockBackDirection = Vector3.Normalize (transform.position - other.transform.position);
			knockBackDirection.y = 1;
			stunned = true;
			stunTime = Time.time;
			rb.AddForce (knockBackDirection * 1000);
		}
	}

	void DoMovement (float moveX, float moveZ)
	{

		//	If horizontal input is positive
		if (moveX > 0) {
			//	Make sure the velocity in that direction is less than maxSpeed
			if (rb.velocity.x <= maxSpeed)
				//	And if it is, add a force
				rb.AddForce (new Vector3 (moveX * addSpeed, 0, 0));
			//	If horizontal input is negative
		} else if (moveX < 0) {
			//	Make the sure the velocity in that direction is higher than -maxSpeed
			if (rb.velocity.x >= -maxSpeed)
				//	If it is, add a force
				rb.AddForce (new Vector3 (moveX * addSpeed, 0, 0));
		}
		
		//	If forward movement is positive
		if (moveZ > 0) {
			//	Make sure the velocity in that direciton is less than maxSpeed
			if (rb.velocity.z <= maxSpeed)
				//	And if it is that is the only possible way this force can be added.
				rb.AddForce (new Vector3 (0, 0, moveZ * addSpeed));
			//	And if the velocity in that direction is negative
		} else if (moveZ < 0) {
			//	Then check that our velocity is higher than -maxSpeed
			if (rb.velocity.z >= -maxSpeed)
				//	And only if it is, add a force.
				rb.AddForce (new Vector3 (0, 0, moveZ * addSpeed));
		}
		
		//	Now let's do some rotating
		//	First, which way are we trying to face?
		Vector3 targetDirection = new Vector3 (moveX, 0.0f, moveZ);
		//	Perform some rotations based on the targetDirection
		//	NOTE: The rigidbody is not rotated. Only the transform is rotated.
		if (targetDirection.x > 0)
			rotateVec = Vector3.RotateTowards (transform.forward, Vector3.forward, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
		else if (targetDirection.x < 0)
			rotateVec = Vector3.RotateTowards (transform.forward, -Vector3.forward, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
		if (rotateVec != Vector3.zero)
			transform.rotation = Quaternion.LookRotation (rotateVec);
		
		if (Input.GetKeyDown (KeyCode.Space)) {
			rb.AddForce (new Vector3 (0, 1000, 0));
		}

	}
	
}




