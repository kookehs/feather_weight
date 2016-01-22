using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	//	This script will record where I am with respect
	//	to the target, and then keep me at that same
	//	distance.

	public GameObject target;
	
	float zDistance;
	float yDistance;
	float xDistance;

	// Use this for initialization
	void Start () {

		zDistance = target.transform.position.z - transform.position.z;
		yDistance = target.transform.position.y - transform.position.y;
		xDistance = target.transform.position.x - transform.position.x;

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		transform.position = new Vector3 (target.transform.position.x - xDistance,target.transform.position.y - yDistance,target.transform.position.z - zDistance);

	}
}
