using UnityEngine;
using System.Collections;

public class River : MonoBehaviour {
	// This script considers all bodies of water and water points

	private GameObject player;
	private GameObject[] riverpoints;
	private float _min_dist = float.MaxValue;

	public float min_dist {
		get { return _min_dist; }
		set { _min_dist = value;}
	}

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		riverpoints = GameObject.FindGameObjectsWithTag ("RiverPoint");
	}

	// Update is called once per frame
	void Update () {
        if (player == null) return;
		//	If the player goes near river determine which river point they are closest to
		foreach (GameObject point in riverpoints)
			point.GetComponent<DistancePoints> ().SetPoint (Vector3.Distance (player.transform.position, point.transform.position));
	}

	public void DistanceToPlayer () {
		if (player == null) return;
		//	If the player goes near river determine which river point they are closest to
		_min_dist = float.MaxValue;
		foreach (GameObject point in riverpoints) {
			float dist = Vector3.Distance (player.transform.position, point.transform.position);
			point.GetComponent<DistancePoints> ().SetPoint (dist);
			if (_min_dist > dist) _min_dist = dist;
		}
	}

	public void OnCollisionEnter(Collision obj){
		if (obj.gameObject.tag.Equals ("Player")) {
			player.GetComponent<PlayerMovementRB> ().Reposition ();
		}
	}
}
