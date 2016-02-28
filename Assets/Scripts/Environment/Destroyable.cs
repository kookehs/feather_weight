using UnityEngine;
using System.Collections;

public class Destroyable : Strikeable {

	public GameObject collectable;

	public int totalDropNum = 2;

	// Drop collectables on the ground
	protected virtual void DropCollectable () {
		//Quaternion rot = Quaternion.AngleAxis (0f, new Vector3 (0f, 0f, 0f));
		for (int i = 0; i < totalDropNum; i++)
			Instantiate (collectable, new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), Quaternion.identity);
		Destroy (gameObject);
	}
}