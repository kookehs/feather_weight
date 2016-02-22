using UnityEngine;
using System.Collections;

public class Destroyable : Strikeable {

	public GameObject collectable;

	public int totalDropNum = 2;
	public bool isTooSmall = false;

	WorldContainer the_world;

	// Drop nuts on the ground
	protected override void DropCollectable () {
		for (int i = 0; i < totalDropNum; i++)
			Instantiate (collectable, new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), transform.rotation);
	}
}