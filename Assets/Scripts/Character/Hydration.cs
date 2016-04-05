using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hydration : SurvivalStat
{
	private WaterPoint[] water;
	// Use this for initialization
	void Start ()
	{
		_loss_over_time = true;
		water = GameObject.Find ("Water").GetComponentsInChildren<WaterPoint>();
		DisableBuffer ();
		Initialize ();
	}

	protected virtual void Initialize() {
	}

	// Update is called once per frame
	protected override void UpdateRoutine() {
		if (IsNearWater ()) {
			Increase (1f);
			_loss_over_time = false;
		} else {
			_loss_over_time = true;
		}
	}

	protected virtual bool IsNearWater() {
		foreach (WaterPoint w in water) 
			if (w.player_is_near)
				return true;
		return false;
	}
}
