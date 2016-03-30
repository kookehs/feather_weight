using UnityEngine;
using System.Collections;

public class PlayerMovementRB : Strikeable
{
	public float addSpeed = 200f;
	public float maxSpeed = 10f;
	private Vector3 rotateVec;
	public float rotateBy = 200f;
	public bool mouseHovering = false;
	public bool isOnLadder = false;
	public float ladderSpeed = 5f;

	//	Stun and stun timer

	private bool can_jump = true;

	float distToGround;
	float height;

	//Animation
	private Animator anim;

	private LayerMask the_ground;

	private Vector3 spawn_pos;
	private bool _lightning_armor_on = false;

	public bool lightning_armor_on {
		get { return this._lightning_armor_on; }
		set { _lightning_armor_on = value; }
	}

	// Use this for initialization
	void Start ()
	{
		stunned = false;
		rb = GetComponent<Rigidbody> ();
		anim = GetComponentInChildren<Animator> ();
		if (the_world == null)
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
			if (Time.time - stun_time >= stun_length)
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
			if (other.transform.parent.gameObject.GetComponent<LadderController> ().usable == false)
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
			if (other.transform.parent.gameObject.GetComponent<LadderController> ().usable == false)
				return;

			rb.isKinematic = false;
			isOnLadder = false;
			other.gameObject.transform.parent.GetComponent<LadderController> ().Dismount (other.tag);
		}
	}

	protected override void DuringHit (Collider other, float damage, float knock_back_force, string hitter)
	{
		if (hitter.Equals ("BOSS_LIGHTNING") && _lightning_armor_on) {
			Health health = GetComponent<Health> ();
			if (health != null)
				health.Decrease (Mathf.Ceil (damage / 2));
		} else
			base.DuringHit (other, damage, knock_back_force, hitter);
	}

	protected override bool AfterHit (string hitter)
	{
		return false;
	}

	public bool isGrounded ()
	{
		return Physics.Raycast (transform.position, -Vector3.up, distToGround + 0.1f, the_ground);
	}

	public bool isMoving ()
	{
		return !isGrounded () && rb.velocity != Vector3.zero;
	}

	void DoMovement (float moveX, float moveZ)
	{
		if (GetComponent<Health> ().IsDead ())
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

                                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Ladder")) {
                                        Debug.Log("here: " + Vector3.Distance(transform.position, obj.transform.position));
                                        if (Vector3.Distance(transform.position, obj.transform.position) > 2.0f) {
                                                float min = Mathf.Infinity;
                                                GameObject nearest = null;

                                                foreach (GameObject lbd in GameObject.FindGameObjectsWithTag("LadderBottomDismount")) {
                                                        float dist = Vector3.Distance(transform.position, lbd.transform.position);
                                                        if (dist < min) {
                                                                min = dist;
                                                                nearest = lbd;
                                                        }
                                                }

                                                if (nearest != null) {
                                                        transform.position = nearest.transform.position;
                                                        isOnLadder = false;
                                                        rb.isKinematic = false;
                                                }
                                        }
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
				bool not_below_ground = Physics.Raycast (transform.position, Vector3.down, Mathf.Infinity, the_ground);
				if (not_below_ground && Physics.Raycast (d_pos, Vector3.down, out hit, Mathf.Infinity, the_ground)) {
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
