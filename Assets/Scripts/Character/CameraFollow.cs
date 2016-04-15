using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Transform target;            // The position that that camera will be following.
	public float smoothing = 1f;        // The speed with which the camera will be following.
	private RaycastHit ray;
	private Collider inTheWay;
	private GameObject far_point;
	private LayerMask blockers;
	// The initial offset from the target.

	void Awake() {
		transform.LookAt (target);
	}

	void Start ()
	{
		// Calculate the initial offset.
		far_point = GameObject.Find ("camerapoint");
		transform.position = far_point.transform.position;
		transform.rotation = far_point.transform.rotation;
		WorldContainer.the_world.Orient2DObjects ();
		//blockers = 1 << LayerMask.NameToLayer ("Ground") | 1 << LayerMask.NameToLayer("Tree") | Physics.IgnoreRaycastLayer;
	}

	void LateUpdate() {
		if (target == null) return;
		transform.position = far_point.transform.position;
		transform.rotation = far_point.transform.rotation;
		/*if (Physics.Linecast (target.transform.position, far_point.transform.position, out ray, blockers))
			transform.position = ray.point;
		*/
	}
}