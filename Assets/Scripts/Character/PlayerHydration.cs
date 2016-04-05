using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHydration : Hydration
{

	protected override void Initialize() {
		gui.maxValue = 100f;
		value = 100f;
		loss_timer = 30f;
		loss_frequency = 30f;
	}

	protected override void UpdateRoutine() {
		if (IsNearWater ()) {
			Increase (1f);
			_loss_over_time = false;
		} else {
			_loss_over_time = true;
		}
	}
}

