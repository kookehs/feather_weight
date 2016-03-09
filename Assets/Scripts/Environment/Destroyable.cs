using UnityEngine;
using System.Collections;

public class Destroyable : Strikeable {

	public GameObject collectable;

	public int totalDropNum = 2;

	protected char[] separator = { '_' };

	protected override bool AfterHit() {
		Health health = GetComponent<Health> ();
		DropCollectable ();
		if (health != null)
			return health.isDead ();
		return false;
	}

	// Drop collectables on the ground
	protected override void DropCollectable () {
		Vector3 drop_position = new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z);
		//Quaternion rot = Quaternion.AngleAxis (0f, new Vector3 (0f, 0f, 0f));
		for (int i = 0; i < totalDropNum; i++)
			Instantiate (collectable, drop_position, Quaternion.identity);
		if (tag.Contains ("Special")) {
			string thing = tag.Split (separator, System.StringSplitOptions.RemoveEmptyEntries)[2];
			switch (thing) {
			case "Antenna":
				QUEST_IDS = new int[]{ 3 };
				DropSpecial (drop_position);
				break;
			default:
				break;
			}
		}
		Destroy (gameObject);
	}
}