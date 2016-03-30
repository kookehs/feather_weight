using UnityEngine;
using System.Collections;

public class CameraPoint : MonoBehaviour {
	public Transform target;            // The position that that camera will be following.
	public float smoothing = 1f;        // The speed with which the camera will be following.
	private WorldContainer the_world;
	private PlayerMovementRB player;
	Vector3 offset;                     // The initial offset from the target.

	/*void Awake () {
		transform.position = Camera.main.transform.position;
		transform.rotation = Camera.main.transform.rotation;
		offset = transform.position - target.position;
	}*/

	void Start ()
	{
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
		player = target.GetComponent<PlayerMovementRB> ();
	}

	void Update() {
		if (Input.GetKey ("e"))
			SmoothRotateCamera (45);
		else if (Input.GetKey ("q"))
			SmoothRotateCamera (-45);
		/*if (target) {
			if (NoMovementInput () && !player.isMoving ()) { 
				if (Input.GetKey ("e"))
					SmoothRotateCamera (90);
				else if (Input.GetKey ("q"))
					SmoothRotateCamera (-90);
			}
			Vector3 targetCamPos = target.position + offset;
			//transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
			transform.position = targetCamPos;
		}*/
	}

	private bool NoMovementInput() {
		return !(Input.GetKey ("w") || Input.GetKey ("a") || Input.GetKey ("s") || Input.GetKey ("d"));
	}

	private void SmoothRotateCamera (float angle) {
		transform.parent = null;
		transform.RotateAround (target.position, target.transform.up, angle * Time.deltaTime);
		transform.parent = target;
		//offset = transform.position - target.position;
		the_world.Orient2DObjects ();
	}
}
