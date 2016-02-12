using UnityEngine;
using System.Collections;

public class Hydration : MonoBehaviour
{

	public float hydration = 100;
	public float lossFrequency = 30;
	public float hydrationDecreaseTimer;
	
	// Use this for initialization
	void Start ()
	{
		hydrationDecreaseTimer = lossFrequency;
	}
	
	// Update is called once per frame
	void Update ()
	{
		hydrationDecreaseTimer -= Time.deltaTime;
		if (hydrationDecreaseTimer <= 0) {
			decreaseHydration ();
			hydrationDecreaseTimer = lossFrequency;
		}
	}

	public void increaseHydration ()
	{
		if (hydration >= 90)
			hydration = 100f;
		else {
			hydration += 10f;
			GetComponent<Health> ().thirsty = false;
		}
	}

	public void decreaseHydration ()
	{
		if (hydration <= 10) {
			hydration = 0f;
			GetComponent<Health> ().thirsty = true;
		} else
			hydration -= 10f;
	}
		
}
