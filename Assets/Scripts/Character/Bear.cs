using UnityEngine;
using System.Collections;

public enum BearState
{
	UNAWARE,
	HOSTILE,
	FRIENDLY
}

[RequireComponent (typeof(CharacterController))]
public class Bear : MonoBehaviour
{

	private BearState state = BearState.UNAWARE;
	public GameObject player;
	public GameObject scenarioController;
	public GameObject target;
	public bool isPlayerNear;
	public float friendliness;
	private Vector3 forward;
	private Vector3 desiredAngle;
	float turnTimer;
	public float rotateBy = 420f;
	float step;
	CharacterController controller;

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
		step = Time.deltaTime * 2f;
		controller = GetComponent<CharacterController> ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (state) {
		case BearState.HOSTILE:
			Debug.Log (">:(");
			faceTarget (target);
			moveToward (target);
			if (friendliness > 0)
				state = BearState.FRIENDLY; 
			break;
		case BearState.FRIENDLY:
			Debug.Log (":)");
			if (friendliness <= 0)
				state = BearState.HOSTILE;
			break;
		case BearState.UNAWARE:
			Debug.Log ("-___-");
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

	public void increaseFriendliness ()
	{
		friendliness += 1;
	}

	public void decreaseFriendliness ()
	{
		friendliness += 1;
	}

	private void moveToward(GameObject target){
		Debug.Log ("HERE");
		Vector3 ignoreTargetY = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
		//transform.position = Vector3.MoveTowards (transform.position, ignoreTargetY, step);
		if ((controller.collisionFlags & CollisionFlags.Below)==0)
		{
			ignoreTargetY.y = -1;
		}
		controller.Move ((ignoreTargetY - transform.position) * Time.deltaTime);
	}
}
