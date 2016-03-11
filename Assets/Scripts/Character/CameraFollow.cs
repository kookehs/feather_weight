using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Transform target;            // The position that that camera will be following.
	public float smoothing = 1f;        // The speed with which the camera will be following.
	private RaycastHit ray;
	private Collider inTheWay;
	private WorldContainer the_world;
	private GameObject far_point;
	private LayerMask blockers;
	// The initial offset from the target.

	void Start ()
	{
		// Calculate the initial offset.
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
		far_point = GameObject.Find ("camerapoint");
		blockers = 1 << LayerMask.NameToLayer ("Ground") | 1 << LayerMask.NameToLayer("Tree") | Physics.IgnoreRaycastLayer;
	}

	void LateUpdate() {
		if (target == null) return;
		transform.position = far_point.transform.position;
		transform.rotation = far_point.transform.rotation;
		if (Physics.Linecast (target.transform.position, far_point.transform.position, out ray, blockers))
			transform.position = ray.point;
	}
}