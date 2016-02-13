using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour {

	public GameObject player;
	public GameObject rock;

	public bool isTooSmall = false;

	void OnCollisionEnter(Collision obj){
		if (obj.collider.name.Equals ("EquipedWeapon")) {
			DropRocks ();
		}
	}

	// Drop nuts on the ground
	public void DropRocks () {
		if (!isTooSmall) {
			for (int i = 0; i < 5; i++) {
				Instantiate (rock, new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), transform.rotation);
			}
			isTooSmall = !isTooSmall;
			Destroy (gameObject);
		}
	}
}
