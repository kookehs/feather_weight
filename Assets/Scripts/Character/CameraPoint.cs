using UnityEngine;
using System.Collections;

public class CameraPoint : MonoBehaviour {
	public Transform target;            // The position that that camera will be following.
	public float smoothing = 1f;        // The speed with which the camera will be following.
	private WorldContainer the_world;
	private PlayerMovementRB player;
	Vector3 offset;                     // The initial offset from the target.

	void Awake () {
		transform.position = Camera.main.transform.position;
		transform.rotation = Camera.main.transform.rotation;
		offset = transform.position - target.transform.position;
		//offset = transform.position - new Vector3(-15, 17, 15);
	}

	void Start ()
	{
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
		player = target.GetComponent<PlayerMovementRB> ();
	}

	void FixedUpdate() {
		if (player == null) return;
		Vector3 targetCamPos = target.position + offset;
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
		if (NoMovementInput() && !player.isMoving()) { 
			if      (Input.GetKey ("e")) SmoothRotateCamera (90);
			else if (Input.GetKey ("q")) SmoothRotateCamera (-90);
		}
	}

	private bool NoMovementInput() {
		return !(Input.GetKey ("w") || Input.GetKey ("a") || Input.GetKey ("s") || Input.GetKey ("d"));
	}

	private void SmoothRotateCamera (float angle) {
		transform.RotateAround (target.position, Vector3.up, angle * Time.deltaTime);
		offset = transform.position - target.position;
		the_world.Orient2DObjects ();
	}
}
