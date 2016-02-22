using UnityEngine;
using System.Collections;

public class PlayerMovementRB : MonoBehaviour
{

	public Rigidbody rb;
	public float addSpeed = 75f;
	public float maxSpeed = 10f;
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

	float distToGround;

	//Animation
	private Animator anim;

	private WorldContainer the_world;
	
	// Use this for initialization
	void Start ()
	{
		
		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
		distToGround = GetComponent<Collider>().bounds.extents.y;
		
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{

		if (!stunned) {
			//	Perform movement function by capturing input
			DoMovement (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		} else {
			if (Time.time - stunTime >= stunLength)
				stunned = false;
		}

	}

	/*void OnTriggerEnter(Collider other) {
		bool killed = false;
		if (other.tag.Equals ("Bear")) {
			killed = other.gameObject.GetComponent<BearRB> ().receiveHit (GetComponent<Collider>(), 10, 1000);
		}
		if (killed) {
			the_world.UpdateKillCount (other.tag);
		}
	}*/

	public void receiveHit (Collider other, float damage, float knockBackForce)
	{
		Debug.Log (damage);
		GetComponent<Health> ().decreaseHealth (damage);
		rb.velocity = Vector3.zero;
		Vector3 knockBackDirection = Vector3.Normalize (transform.position - other.transform.position);
		knockBackDirection.y = 1;
		stunned = true;
		stunTime = Time.time;
		rb.AddForce (knockBackDirection * knockBackForce);
	}

	private bool isGrounded() {
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void DoMovement (float moveX, float moveZ)
	{
		Vector3 movement = new Vector3 (0, 0, 0);

		//This is set to true by default and, later in this function, is set to false if no input detected
		anim.SetBool ("isRunning", true);

		//	If horizontal input and vertical input are nonzero
		if (moveX != 0 || moveZ != 0) {
			Vector3 direction = Vector3.Normalize (new Vector3 (moveX, 0, moveZ));
			Vector3 vwrtc = rb.velocity;
			vwrtc = Camera.main.transform.TransformDirection (vwrtc);
			float velocity = Mathf.Sqrt (vwrtc.x * vwrtc.x + vwrtc.z * vwrtc.z);
			//Debug.Log (velocity);
			//	Make sure the velocity in that direction is within maxSpeed
			if (velocity <= maxSpeed && velocity >= -maxSpeed) {
				//	And if it is, add a force
				//rb.AddForce (addSpeed * direction);
				movement = addSpeed * direction;
			}
			if (moveX < 0)
				anim.SetBool ("right", true);
			else
				anim.SetBool ("right", false);
			if (moveZ > 0)
				anim.SetBool ("up", true);
			else
				anim.SetBool ("up", false);	
		}

		//If all movement is zero
		else {
			anim.SetBool ("isRunning", false);
		}


		movement = Camera.main.transform.TransformDirection (movement);
		movement.y = 0;
		rb.AddForce (movement);
		//Debug.Log (rb.velocity);
		
		/*	Now let's do some rotating
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
		*/
		
		if (Input.GetKeyDown (KeyCode.Space) && isGrounded()) {
			rb.AddForce (new Vector3 (0, 1500, 0));
		}

	}

	public void Reposition(){
		GameObject[] respawnPoint = GameObject.FindGameObjectsWithTag ("RespawnPoint");

		for (int i = 0; i < respawnPoint.Length; i++) {
			respawnPoint[i].GetComponent<DistancePoints> ().SetPoint (Vector3.Distance (transform.position, respawnPoint[i].transform.position));
		}

		//find the river point closest
		GameObject closestObj = null;
		float min = float.MaxValue;
		foreach (GameObject obj in respawnPoint) {
			if (obj.GetComponent<DistancePoints> ().isNearest < min) {
				min = obj.GetComponent<DistancePoints> ().isNearest;
				closestObj = obj;
			}
		}

		transform.position = closestObj.transform.position;
	}
	
}
