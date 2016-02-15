using UnityEngine;
using System.Collections;

public class WaterSkin : MonoBehaviour {

	private GameObject player;
	public bool waterFull = false;
	private float waterAmount = 0;

	// Update is called once per frame
	public void DrinkWater () {
		player = GameObject.FindGameObjectWithTag ("Player");
		if (waterFull) {
			while (player.GetComponent<Hydration> ().hydration < waterAmount) {
				player.GetComponent<Hydration> ().increaseHydration ();
				SetEmpty ();
			}
		}
		Debug.Log (waterFull);
	}

	public void Fill(){
		Debug.Log ("fill");
		waterAmount = 100.0f;
		waterFull = true;
	}

	private void SetEmpty(){
		waterAmount = 0;
		waterFull = false;
	}
}

//current issues
//bridge spawns only Attribute last river point
//when trying to collect from river it says it's deleted
//sword is vary far away from player
//can' click on river to get water
//stop player from dropping in random places
//make it so player spawns outside of the area
//stop player from dropping bridge