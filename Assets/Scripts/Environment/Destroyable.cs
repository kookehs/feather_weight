using UnityEngine;
using System.Collections;

public class Destroyable : MonoBehaviour {

	public GameObject player;
	public GameObject collectable;

	public int totalDropNum = 2;
	public bool isTooSmall = false;

	void OnCollisionEnter(Collision obj){
		if (obj.collider.name.Equals ("EquipedWeapon")) {
			DropCollectable ();
		}
	}

    public bool receiveHit() {
		DropCollectable ();
        return false;
    }

	// Drop nuts on the ground
	public void DropCollectable () {
		if (!isTooSmall) {
			for (int i = 0; i < totalDropNum; i++) {
				Instantiate (collectable, new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), transform.rotation);
			}
			isTooSmall = !isTooSmall;
			Destroy (gameObject);
		}
	}
}
