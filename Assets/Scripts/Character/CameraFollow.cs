using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Transform target;            // The position that that camera will be following.
	public float smoothing = 1f;        // The speed with which the camera will be following.
	private RaycastHit ray;
	private Collider inTheWay;
	private WorldContainer the_world;
	private float distanceToReturn = 0f;
	private GameObject camPoint;
	Vector3 offset;                     // The initial offset from the target.

	void Start ()
	{
		// Calculate the initial offset.
		offset = transform.position - target.position;
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
		camPoint = GameObject.Find ("camerapoint").gameObject;
	}

	void FixedUpdate ()
	{
		// Create a postion the camera is aiming for based on the offset from the target.
		//Vector3 targetCamPos = target.position + offset;

		// Smoothly interpolate between the camera's current position and it's target position.
		//transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);

		//if      (Input.GetKeyDown ("e")) RotateCamera (90);
		//else if (Input.GetKeyDown ("q")) RotateCamera (-90);

		//transform.LookAt (target.transform);
	}

	void LateUpdate() {
		if (!(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))) { 
			if      (Input.GetKey ("e")) SmoothRotateCamera (90);
			else if (Input.GetKey ("q")) SmoothRotateCamera (-90);
		}
		SmartCam ();
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

	private void SmartCam(){
		if (hitcheck ()) {
			Physics.Linecast (target.transform.position, camPoint.transform.position, out ray);
			//Debug.Log (ray.point);
			transform.position = ray.point;
			//distanceToReturn = ((camPoint.transform.position - ray.point).sqrMagnitude);
			//distanceToReturn = ((camPoint.transform.position - ray.point).sqrMagnitude);
			//transform.position = camPoint.transform.position + (transform.forward *distanceToReturn);
			//camPoint.transform.position = (transform.position - (camPoint.transform.forward * distanceToReturn));
		}else {
			transform.position = camPoint.transform.position;
			//transform.position -= (transform.forward * distanceToReturn);
			//distanceToReturn = 0;
		}
	}

	/*private void SmartCam (){
		if (!wallcheck2 () && distanceToReturn > 0f) {
			transform.position -= (transform.forward * .3f);
			distanceToReturn -= .3f;
		} else if(wallcheck()) {
			transform.position += (transform.forward * .3f);
			distanceToReturn += .3f;
		}
	}
*/
	private bool hitcheck(){
		return (Physics.Linecast (target.transform.position, camPoint.transform.position));
	}
	/*
	private bool wallcheck(){
		if (hitcheck ()) {
			Physics.Linecast (transform.position, target.transform.position, out ray);
			if (ray.collider.gameObject.tag == "wall" || ray.collider.gameObject.tag == "Tree")
				return true;
		}
		return false;
	}

	private bool wallcheck2(){
		if (hitcheck ()) {
			Physics.Linecast ((transform.position - transform.forward), target.transform.position, out ray);
			if (ray.collider.gameObject.tag == "wall" || ray.collider.gameObject.tag == "Tree")
				return true;
		}
		return false;
	}*/
}
