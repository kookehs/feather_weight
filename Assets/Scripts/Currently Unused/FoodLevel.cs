using UnityEngine;
using System.Collections;

public class FoodLevel : MonoBehaviour
{

	public float foodLevel = 100;
	public float lossFrequency = 60;
	private float foodLevelDecreaseTimer;
	
	// Use this for initialization
	void Start ()
	{
		foodLevelDecreaseTimer = lossFrequency;
	}
	
	// Update is called once per frame
	void Update ()
	{
		foodLevelDecreaseTimer -= Time.deltaTime;
		if (foodLevelDecreaseTimer <= 0) {
			decreaseFoodLevel ();
			foodLevelDecreaseTimer = lossFrequency;
		}
		
	}

	public void increaseFoodLevel ()
	{
		if (foodLevel >= 90)
			foodLevel = 100f;
		else {
			foodLevel += 10f;
			GetComponent<Health> ().hungry = false;
		}
	}

	public void decreaseFoodLevel ()
	{
		if (foodLevel <= 10) {
			foodLevel = 0f;
			GetComponent<Health> ().hungry = true;
		} else
			foodLevel -= 10f;
	}
		
}
