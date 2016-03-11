using UnityEngine;
using System.Collections;

public class River : MonoBehaviour {

	private GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	void Update () {
                if (player == null)
                    return;
		//	If the player goes near river determine which river point they are closest to
		Transform riverPoints = transform.FindChild("RiverPoints").transform;
		for (int i = 0; i < riverPoints.childCount; i++) {
			Transform child = riverPoints.GetChild (i);
			child.GetComponent<DistancePoints> ().SetPoint (Vector3.Distance (player.transform.position, child.position));
		}
	}

	public void OnCollisionEnter(Collision obj){
		if (obj.gameObject.tag.Equals ("Player")) {
			player.GetComponent<PlayerMovementRB> ().Reposition ();
		}
	}
}
