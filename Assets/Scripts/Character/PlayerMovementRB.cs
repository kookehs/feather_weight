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
        public bool isOnLadder = false;
        public float ladderSpeed = 5f;

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
	private LayerMask the_ground;

	// Use this for initialization
	void Start ()
	{

		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
		the_ground = 1 << LayerMask.NameToLayer ("Ground");
		distToGround = GetComponent<Collider>().bounds.extents.y;

	}

	// Update is called once per frame
	void FixedUpdate () {
                /*
                if (the_world.GetNearestObject("Ladder", gameObject, 1.5f)) {
                        Debug.Log("On");
                        isOnLadder = true;
                } else {
                        Debug.Log("Off");
                        isOnLadder = false;
                }
                */

		if (!stunned) {
			//	Perform movement function by capturing input
			DoMovement (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		} else {
			if (Time.time - stunTime >= stunLength)
				stunned = false;
		}

		if (isGrounded ()) {
                        isOnLadder = false;
			rb.isKinematic = false;
		}
	}

	void OnTriggerEnter(Collider other) {
                /*
		bool killed = false;
		if (other.tag.Equals ("Bear")) {
			killed = other.gameObject.GetComponent<BearRB> ().receiveHit (GetComponent<Collider>(), 10, 1000);
		}
		if (killed) {
			the_world.UpdateKillCount (other.tag);
		}
                */
                if (other.tag == "LadderBottom") {
                        if (isOnLadder == false) {
                                isOnLadder = true;
                                rb.isKinematic = true;
                                Vector3 ladderPosition = other.gameObject.transform.position;
                                Vector3 climbPosition = new Vector3(ladderPosition.x, transform.position.y + 0.5f, ladderPosition.z);
                                climbPosition -= other.gameObject.transform.forward * 0.5f;
                                transform.position = climbPosition;
                        } else {
                                rb.isKinematic = false;
                                isOnLadder = false;
                                other.gameObject.transform.parent.GetComponent<LadderController>().Dismount(other.tag);
                        }

                        isOnLadder = true;
                } else if (other.tag == "LadderTop") {
                        rb.isKinematic = false;
                        isOnLadder = false;
                        other.gameObject.transform.parent.GetComponent<LadderController>().Dismount(other.tag);
                }
	}

	public void receiveHit (Collider other, float damage, float knockBackForce)
	{
		GetComponent<Health> ().decreaseHealth (damage);
		rb.velocity = Vector3.zero;
		Vector3 knockBackDirection = Vector3.Normalize (transform.position - other.transform.position);
		knockBackDirection.y = 1;
		stunned = true;
		stunTime = Time.time;
		rb.AddForce (knockBackDirection * knockBackForce);
	}

	public bool isGrounded() {
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f, the_ground);
	}

	void DoMovement (float moveX, float moveZ)
	{
                if (GetComponent<Health>().isDead())
                        return;

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

                        if (isOnLadder) {
                                if (moveZ > 0) {
                                        transform.Translate(Vector3.up * Time.deltaTime * ladderSpeed);
                                } else if (moveZ < 0) {
                                        transform.Translate(Vector3.up * -1 * Time.deltaTime * ladderSpeed);
                                }
                        } else {
                                movement = Camera.main.transform.TransformDirection (movement);
                                movement.y = 0;
                        }
		}

		//If all movement is zero
		else {
			anim.SetBool ("isRunning", false);
		}

		rb.AddForce (movement);

		if (Input.GetKeyDown (KeyCode.Space) && isGrounded()) {
			rb.AddForce (new Vector3 (0, 1500, 0));
			//rb.isKinematic = true;
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
