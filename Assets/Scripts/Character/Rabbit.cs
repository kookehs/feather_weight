using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rabbit : Animal {

	public override void performStateCheck(){
		//	If we are not running...
		if (friendliness > 0) {
			if (state == AnimalState.RUNNING) {
				if (runTime < 150f) {
					runTime += Time.deltaTime;
				} else {
					runTime = 0;
					state = AnimalState.UNAWARE;
				}
			}else if (Vector3.Distance (player.transform.position, transform.position) < 10f) {
				state = AnimalState.RUNNING;
				target = player;
			} else {
				state = AnimalState.UNAWARE;
			}
		} else {
			state = AnimalState.HOSTILE;
		}
	}

	public bool isKiller() {
		if (state == AnimalState.HOSTILE) return true;
		return false;
	}

	protected override void Initialize() {
		primary_drop = "Raw_Meat";
		special_drops = new List<string> ();
		special_drops.Add ("Rabbit's Feet");
		special_drops.Add ("Rabbit's Horn");
		QUEST_IDS = new int[1];

		if (WorldContainer.killer_bunny_world)
			friendliness = -10f;
		else {
			friendliness = 10f;
		}
	}

	protected override bool DamagePlayerOnCollision ()
	{
		if (state == AnimalState.HOSTILE)
			return true;
		else
			return false;
	}

	protected override void DropSpecial(Vector3 drop_position) {
		if (isKiller ()) {
			QUEST_IDS [0] = 2;
			if (QuestController.QuestActivated (QUEST_IDS, QUEST_UNION))
				WorldContainer.Create (special_drops [1], drop_position);
		} else {
			QUEST_IDS [0] = 1;
			if (QuestController.QuestActivated (QUEST_IDS, QUEST_UNION))
				WorldContainer.Create (special_drops [0], drop_position);
		}
	}
}
