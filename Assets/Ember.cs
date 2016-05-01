using UnityEngine;
using System.Collections;

public class Ember : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision c) {
		if (c.gameObject.tag.Equals ("Player")) {
			c.gameObject.GetComponent<PlayerMovementRB> ().receiveHit(GetComponent<Collider>(), 10, 600f, tag);
		}
	}
}
