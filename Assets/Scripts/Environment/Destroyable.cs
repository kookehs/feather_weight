using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Destroyable : Strikeable {

	public GameObject collectable;

	public int totalDropNum = 2;

	protected char[] separator = { '_' };

	protected override bool AfterHit(string hitter) {
		invincible = false;
		Health health = GetComponent<Health> ();
		// DropCollectable (hitter);
		if (health != null && health.IsDead ())
			Destroy (gameObject);
		return false;
	}
}
