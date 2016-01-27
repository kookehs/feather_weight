using UnityEngine;
using System.Collections;

public class PlayerMovementRB : MonoBehaviour {

	private Rigidbody rb;
	private Vector3 rotateVec;
	public float rotateBy = 200f;
	public float speed = 10f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rotateVec = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {

		//	Get inputs
		float horizontalInput = Input.GetAxisRaw("Horizontal");
		float verticalInput = Input.GetAxisRaw("Vertical");

		//	Perform movements
		Vector3 movement = new Vector3 (horizontalInput, 0.0f, verticalInput);
		rb.velocity = movement * speed;

		/*Perform some rotations based on the targetDirection
		if ( targetDirection.x > 0 ) rotateVec = Vector3.RotateTowards(transform.forward, Vector3.forward, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
		else if ( targetDirection.x < 0 ) rotateVec = Vector3.RotateTowards(transform.forward, -Vector3.forward, rotateBy * Mathf.Deg2Rad * Time.deltaTime, 1000);
		if (rotateVec != Vector3.zero) transform.rotation = Quaternion.LookRotation(rotateVec);*/
	
	}
}
