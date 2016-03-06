using UnityEngine;
using System.Collections;

public class CameraPoint : MonoBehaviour {
	public Transform target;            // The position that that camera will be following.
	public float smoothing = 1f;        // The speed with which the camera will be following.
	private WorldContainer the_world;
	Vector3 offset;                     // The initial offset from the target.

	void Awake () {
		transform.position = Camera.main.transform.position;
		transform.rotation = Camera.main.transform.rotation;
	}

	void Start ()
	{
		// Calculate the initial offset.
		offset = transform.position - target.position;
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
	}

	void FixedUpdate ()
	{
		// Create a postion the camera is aiming for based on the offset from the target.
		Vector3 targetCamPos = target.position + offset;

		// Smoothly interpolate between the camera's current position and it's target position.
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);

		//if      (Input.GetKeyDown ("e")) RotateCamera (90);
		//else if (Input.GetKeyDown ("q")) RotateCamera (-90);

		//transform.LookAt (target.transform);
	}

	void LateUpdate() {
		if (!(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))) { 
			if      (Input.GetKey ("e")) SmoothRotateCamera (90);
			else if (Input.GetKey ("q")) SmoothRotateCamera (-90);
		}
	}

	/*private void RotateCamera (float angle) {
		transform.RotateAround (target.position, Vector3.up, angle);
		offset = transform.position - target.position;
		target.GetComponent<PlayerMovementRB> ().myForward = Vector3.Normalize(transform.forward);
		target.GetComponent<PlayerMovementRB> ().myRight = Vector3.Normalize(transform.right);
		the_world.Orient2DObjects ();
	}*/

	private void SmoothRotateCamera (float angle) {
		transform.RotateAround (target.position, Vector3.up, angle * Time.deltaTime);
		offset = transform.position - target.position;
		the_world.Orient2DObjects ();
	}
}
