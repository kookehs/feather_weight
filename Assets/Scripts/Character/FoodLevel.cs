using UnityEngine;
using System.Collections;

public class FoodLevel : SurvivalStat
{
	// Use this for initialization
	void Start ()
	{
		gui.maxValue = 100f;
		_loss_over_time = true;
	}
}
