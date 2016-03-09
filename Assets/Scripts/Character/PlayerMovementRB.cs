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
	private bool can_jump = true;
	private float stunTime;
	public float stunLength = 1f;

	//What is forward, what is right? These will later be accessed by the camera.
	public Vector3 myForward = Vector3.forward;
	public Vector3 myRight = Vector3.right;

	float distToGround;
	float height;

	//Animation
	private Animator anim;

	private WorldContainer the_world;
	private LayerMask the_ground;

	private bool _lightning_armor_on = false;

	public bool lightning_armor_on {
		get { return this._lightning_armor_on; }
		set { _lightning_armor_on = value; }
	}

	// Use this for initialization
	void Start ()
	{

		rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
		the_ground = 1 << LayerMask.NameToLayer ("Ground");
		distToGround = GetComponent<Collider> ().bounds.extents.y;
		height = GetComponent<Collider> ().bounds.size.y;
	}

	// Update is called once per frame
	void Update ()
	{
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

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "LadderBottom") {
			if (other.transform.parent.gameObject.GetComponent<LadderController>().usable == false)
				return;

			if (isOnLadder == false) {
				isOnLadder = true;
				rb.isKinematic = true;
				Vector3 ladderPosition = other.gameObject.transform.position;
				Vector3 climbPosition = new Vector3 (ladderPosition.x, transform.position.y + 0.5f, ladderPosition.z);
				climbPosition -= other.gameObject.transform.forward * 0.5f;
				transform.position = climbPosition;
			} else {
				rb.isKinematic = false;
				isOnLadder = false;
				other.gameObject.transform.parent.GetComponent<LadderController> ().Dismount (other.tag);
			}

			isOnLadder = true;
		} else if ((other.tag == "LadderTop") && (isOnLadder == true)) {
			if (other.transform.parent.gameObject.GetComponent<LadderController>().usable == false)
				return;

			rb.isKinematic = false;
			isOnLadder = false;
			other.gameObject.transform.parent.GetComponent<LadderController> ().Dismount (other.tag);
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

	public bool isGrounded ()
	{
		return Physics.Raycast (transform.position, -Vector3.up, distToGround + 0.1f, the_ground);
	}

	public bool isMoving() {
		return !isGrounded () && rb.velocity != Vector3.zero;
	}

	void DoMovement (float moveX, float moveZ)
	{
		if (GetComponent<Health> ().isDead ())
			return;

		Vector3 movement = new Vector3 (0, 0, 0);

		//This is set to true by default and, later in this function, is set to false if no input detected
		anim.SetBool ("isRunning", true);

		//	If horizontal input and vertical input are nonzero
		if (moveX != 0 || moveZ != 0) {
			if (isOnLadder) {
				if (moveZ > 0) {
					transform.Translate (Vector3.up * Time.deltaTime * ladderSpeed);
				} else if (moveZ < 0) {
					transform.Translate (Vector3.up * -1 * Time.deltaTime * ladderSpeed);
				}
			} else {
                                Vector3 direction = Vector3.Normalize (new Vector3 (moveX, 0, moveZ));

                                Vector3 vwrtc = rb.velocity;
                                vwrtc = Camera.main.transform.TransformDirection (vwrtc);
                                float velocity = Mathf.Sqrt (vwrtc.x * vwrtc.x + vwrtc.z * vwrtc.z);

                                // Checking to see if we are walking into a passable elevation
                                Vector3 d_pos = transform.position + direction / 2;
                                d_pos.y = transform.position.y + height;
                                RaycastHit hit;
                                if (Physics.Raycast (d_pos, Vector3.down, out hit, Mathf.Infinity, the_ground)) {
                                        float height_difference = hit.point.y - (transform.position.y - height / 2 + 0.1f);
                                        //Debug.Log (height_difference);
                                        if (0 < height_difference && height_difference < 1f)
                                                transform.position = new Vector3 (hit.point.x, hit.point.y + height / 2 + 0.05f, hit.point.z);
                                }

                                //	Make sure the velocity in that direction is within maxSpeed
                                if (velocity <= maxSpeed && velocity >= -maxSpeed) {
                                        movement = addSpeed * direction;
                                }

				movement = Camera.main.transform.TransformDirection (movement);
				movement.y = 0;
			}

                        SetAnimation (moveX, moveZ);
		}

		//If all movement is zero
		else {
			anim.SetBool ("isRunning", false);
		}

		rb.AddForce (movement);

		if (can_jump) {
			if (Input.GetKeyDown (KeyCode.Space) && isGrounded ()) {
				rb.AddForce (new Vector3 (0, 1500, 0));
				can_jump = !can_jump;
				//rb.isKinematic = true;
			}
		} else {
			if (Input.GetKeyUp (KeyCode.Space)) {
				can_jump = !can_jump;
			}
		}


	}

	public void Reposition ()
	{
		GameObject[] respawnPoint = GameObject.FindGameObjectsWithTag ("RespawnPoint");

		for (int i = 0; i < respawnPoint.Length; i++) {
			respawnPoint [i].GetComponent<DistancePoints> ().SetPoint (Vector3.Distance (transform.position, respawnPoint [i].transform.position));
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

	private void SetAnimation (float moveX, float moveZ)
	{
		if (moveX < 0)
			anim.SetBool ("right", true);
		else if (moveX > 0)
			anim.SetBool ("right", false);
		if (moveZ > 0)
			anim.SetBool ("up", true);
		else if (moveZ < 0)
			anim.SetBool ("up", false);
	}

}
