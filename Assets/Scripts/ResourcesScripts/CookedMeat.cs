using UnityEngine;
using System.Collections;

public class CookedMeat : MonoBehaviour {

	private GameObject player;
	public bool foodFull = false;
	private float foodAmount = 100f;

	public void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	public void EatMeat () {
		while (player.GetComponent<FoodLevel> ().foodLevel < foodAmount) {
			player.GetComponent<FoodLevel> ().increaseFoodLevel ();
			if (player.GetComponent<FoodLevel> ().foodLevel >= 100f)
				player.GetComponent<Health> ().Increase (10f);
		}
	}
}
