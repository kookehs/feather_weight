using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Destroyable : Strikeable {

	public GameObject collectable;

	public int totalDropNum = 2;

	protected char[] separator = { '_' };

	void Start() {
		if (tag.Contains ("Special")) {
			string thing = tag.Split (separator, System.StringSplitOptions.RemoveEmptyEntries)[1];
			InitializeQuestController ();
			special_drops = new List<string> ();
			switch (thing) {
			case "Antenna":
				special_drops.Add ("Antenna");
				break;
			default:
				break;
			}
		}
	}

	protected override bool AfterHit(string hitter) {
		Health health = GetComponent<Health> ();
		DropCollectable (hitter);
		if (health != null)
			return health.IsZero ();
		return false;
	}

	// Drop collectables on the ground
	protected override void DropCollectable (string hitter) {
		Vector3 drop_position = new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z);
		//Quaternion rot = Quaternion.AngleAxis (0f, new Vector3 (0f, 0f, 0f));
		for (int i = 0; i < totalDropNum; i++)
			the_world.Create (collectable.transform, drop_position);
		if (tag.Contains ("Special")) {
			string thing = tag.Split (separator, System.StringSplitOptions.RemoveEmptyEntries)[1];
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