using UnityEngine;
using System.Collections;

public class Hydration : MonoBehaviour
{

	public float hydration = 100;
	public float lossFrequency = 30;
	public float hydrationDecreaseTimer;
	public float near_water_dist = 8f;

	private River water;
	
	// Use this for initialization
	void Start ()
	{
		hydrationDecreaseTimer = lossFrequency;
		water = GameObject.Find ("River").GetComponent<River> ();;
	}
	
	// Update is called once per frame
	void Update ()
	{
		hydrationDecreaseTimer -= Time.deltaTime;
		water.DistanceToPlayer ();
		if (water.min_dist > near_water_dist) {
			if (hydrationDecreaseTimer <= 0) {
				Decrease ();
				hydrationDecreaseTimer = lossFrequency;
			}
			Debug.Log ("Not Near Water");
		} else {
			Increase (1f);
			Debug.Log ("Near Water");
		}
	}

	public void Increase ()
	{
		if (hydration >= 90)
			hydration = 100f;
		else {
			hydration += 10f;
			GetComponent<Health> ().thirsty = false;
		}
	}

	public void Increase (float v) {
		hydration = Mathf.Min (100f, hydration + v);
	}

	public void Decrease ()
	{
		if (hydration <= 10) {
			hydration = 0f;
			GetComponent<Health> ().thirsty = true;
		} else
			hydration -= 10f;
	}
}
