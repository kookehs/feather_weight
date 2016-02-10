using UnityEngine;
using System.Collections;

public class FoodLevel : MonoBehaviour {

	public float foodLevel;
	
	// Use this for initialization
	void Start () {
		foodLevel = 100;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void increasefoodLevel() {
		if (foodLevel >= 90)
			foodLevel = 100f;
		else
			foodLevel += 10f;
	}
	
	public void decreasefoodLevel() {
		if (foodLevel <= 10)
			foodLevel = 0f;
		else
			foodLevel -= 10f;
	}
		
}
