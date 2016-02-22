using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public GameObject target;            // The position that that camera will be following.
	public float smoothing = 1f;        // The speed with which the camera will be following.
	WorldContainer the_world;

	Vector3 offset;                     // The initial offset from the target.
	float yoffset;

	void Start ()
	{
		// Calculate the initial offset.
		offset = transform.position - target.transform.position;
		yoffset = offset.y;
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
	}

	void FixedUpdate ()
	{
		// Create a postion the camera is aiming for based on the offset from the target.
		Vector3 targetCamPos = target.transform.position + offset;

		// Smoothly interpolate between the camera's current position and it's target position.
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}

	void LateUpdate() {
		if (!(Input.GetKey ("w") || Input.GetKey ("a") || Input.GetKey ("s") || Input.GetKey ("d") || !target.GetComponent<PlayerMovementRB> ().isGrounded ())) { 
			if (Input.GetKey ("e"))
				SmoothRotateCamera (90);
			else if (Input.GetKey ("q"))
				SmoothRotateCamera (-90);
		}
	}

	private void SmoothRotateCamera (float angle) {
		transform.RotateAround (target.transform.position, Vector3.up, angle * Time.deltaTime);
		offset = transform.position - target.transform.position;
		offset.y = yoffset;
		the_world.Orient2DObjects ();
	}
}
