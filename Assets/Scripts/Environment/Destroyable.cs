using UnityEngine;
using System.Collections;

public class Destroyable : Strikeable {

	public GameObject collectable;

	public int totalDropNum = 2;

	protected override bool AfterHit() {
		Health health = GetComponent<Health> ();
		DropCollectable (health);
		if (health != null)
			return health.isDead ();
		return false;
	}

	// Drop collectables on the ground
	protected override void DropCollectable (Health health) {
		//Quaternion rot = Quaternion.AngleAxis (0f, new Vector3 (0f, 0f, 0f));
		for (int i = 0; i < totalDropNum; i++)
			Instantiate (collectable, new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z), Quaternion.identity);
		Destroy (gameObject);
	}
}