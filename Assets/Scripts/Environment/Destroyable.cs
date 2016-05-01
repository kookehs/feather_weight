using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Destroyable : Strikeable {

	public GameObject collectable;

	public int totalDropNum = 2;

	protected char[] separator = { '_' };

	void Start() {
	}

	protected override bool AfterHit(string hitter) {
		Health health = GetComponent<Health> ();
		// DropCollectable (hitter);
		if (health != null)
			return health.IsDead ();
		return false;
	}
}
