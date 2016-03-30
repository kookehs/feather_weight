using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour {

	public bool addsomeForce = false;
	public float f = 10;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (addsomeForce == true) {
			GetComponent<Rigidbody> ().AddForce (new Vector3 (f, 0, 0));
		}
	
	}
}
