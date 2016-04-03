using UnityEngine;
using System.Collections;

public class River : MonoBehaviour {
	// This script considers all bodies of water and water points

	private GameObject player;
	private float _min_dist = float.MaxValue;

	public float min_dist {
		get { return _min_dist; }
		set { _min_dist = value;}
	}

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {
        if (player == null) return;
		//	If the player goes near river determine which river point they are closest to
		Transform riverPoints = transform.FindChild("RiverPoints").transform;
		for (int i = 0; i < riverPoints.childCount; i++) {
			Transform child = riverPoints.GetChild (i);
			child.GetComponent<DistancePoints> ().SetPoint (Vector3.Distance (player.transform.position, child.position));
		}
	}

	public void DistanceToPlayer () {
		if (player == null) return;
		//	If the player goes near river determine which river point they are closest to
		Transform riverPoints = transform.FindChild("RiverPoints").transform;
		_min_dist = float.MaxValue;
		for (int i = 0; i < riverPoints.childCount; ++i) {
			Transform child = riverPoints.GetChild (i);
			float dist = Vector3.Distance (player.transform.position, child.position);
			child.GetComponent<DistancePoints> ().SetPoint (dist);
			if (_min_dist > dist) _min_dist = dist;
		}
	}

	public void OnCollisionEnter(Collision obj){
		if (obj.gameObject.tag.Equals ("Player")) {
			player.GetComponent<PlayerMovementRB> ().Reposition ();
		}
	}
}
