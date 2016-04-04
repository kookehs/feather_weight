using UnityEngine;
using System.Collections;

public class Hydration : MonoBehaviour
{

	public float hydration = 100;
	public float lossFrequency = 30;
	public float hydrationDecreaseTimer;

	private WaterPoint[] water;
	// Use this for initialization
	void Start ()
	{
		hydrationDecreaseTimer = lossFrequency;
		water = GameObject.Find ("Water").GetComponentsInChildren<WaterPoint>();
	}

	// Update is called once per frame
	void Update ()
	{
		hydrationDecreaseTimer -= Time.deltaTime;
		bool player_near_water = false;
		foreach (WaterPoint w in water) {
			if (w.player_is_near) {
				player_near_water = true;
				break;
			}
		}
		if (player_near_water) {
			Increase (1f);
			hydrationDecreaseTimer = lossFrequency;
		} else {
			if (hydrationDecreaseTimer <= 0) {
				Decrease ();
				hydrationDecreaseTimer = lossFrequency;
			}
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
