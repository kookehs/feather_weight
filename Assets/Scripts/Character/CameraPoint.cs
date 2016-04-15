using UnityEngine;
using System.Collections;

public class CameraPoint : MonoBehaviour {
	public Transform target;            // The position that that camera will be following.
	private PlayerMovementRB player;

	private float rotate_speed = 75f;

	void Start ()
	{
		player = target.GetComponent<PlayerMovementRB> ();
	}

	void Update() {
		if (Input.GetMouseButton (1)) {
			SmoothRotateCamera(Input.GetAxis("Mouse X") * rotate_speed);
		}
	}

	private void SmoothRotateCamera (float angle) {
		transform.parent = null;
		transform.RotateAround (target.position, target.transform.up, angle * Time.deltaTime);
		transform.parent = target;
		//offset = transform.position - target.position;
		WorldContainer.Orient2DObjects ();
	}
}
