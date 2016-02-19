using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WayPointContainer : MonoBehaviour {

	List<Transform> waypoints;

	// Use this for initialization
	void Start () {
		foreach (Transform t in transform) {
			waypoints.Add (t);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
