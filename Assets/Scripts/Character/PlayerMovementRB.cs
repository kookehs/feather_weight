using UnityEngine;
using System.Collections;

public class PlayerMovementRB : MonoBehaviour
{

	public Rigidbody rb;
	public float addSpeed = 250f;
	public float maxSpeed = 400f;
	private Vector3 rotateVec;
	public float rotateBy = 200f;
	public bool mouseHovering = false;

	//	Stun and stun timer
	private bool stunned = false;
	private float stunTime;
	public float stunLength = 1f;

	//What is forward, what is right? These will later be accessed by the camera.
	public Vector3 myForward = Vector3.forward;
	public Vector3 myRight = Vector3.right;
	
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
		Vector3 movement = new Vector3 (0, 0, 0);

		//	If horizontal input and vertical input are nonzero
		if (moveX != 0 && moveZ != 0) {
			Vector3 direction = Vector3.Normalize (new Vector3 (moveX, 0, moveZ));
			Vector3 vwrtc = rb.velocity;
			vwrtc = Camera.main.transform.TransformDirection (vwrtc);
			float diagonalVel = Mathf.Sqrt (vwrtc.x * vwrtc.x + vwrtc.z * vwrtc.z);
			//	Make sure the velocity in that direction is within maxSpeed
			if (diagonalVel <= maxSpeed && diagonalVel >= -maxSpeed) {
				//	And if it is, add a force
				//rb.AddForce (addSpeed * direction);
				movement = addSpeed * direction;
			}

		}

		//	If horizontal input is nonzero
		else if (moveX != 0) {
			//	Make sure the velocity in that direction is within maxSpeed
			Vector3 vwrtc = rb.velocity;
			vwrtc = Camera.main.transform.TransformDirection (vwrtc);
			if (vwrtc.x <= maxSpeed && vwrtc.x >= -maxSpeed) {
				//	And if it is, add a force
				//moveX = Camera.main.transform.TransformDirection(moveX);
				//rb.AddForce(moveX * addSpeed * Vector3.right);
				movement = moveX * addSpeed * Vector3.right;
			}
		}
		
		//	If forward movement is nonzero
		else if (moveZ != 0) {
			//	Make sure the velocity in that direciton is less than maxSpeed
			Vector3 vwrtc = rb.velocity;
			vwrtc = Camera.main.transform.TransformDirection (vwrtc);
			if (vwrtc.z <= maxSpeed && vwrtc.z >= -maxSpeed)
				//	And if it is that is the only possible way this force can be added.
				//rb.AddForce(moveZ * addSpeed * Vector3.forward);
				movement =
					moveZ * addSpeed * Vector3.forward;
			//	And if the velocity in that direction is negative
		}


		movement = Camera.main.transform.TransformDirection (movement);
		movement.y = 0;
		Debug.Log (movement);
		rb.AddForce (movement);
		
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




