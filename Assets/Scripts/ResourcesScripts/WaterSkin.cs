using UnityEngine;
using System.Collections;

public class WaterSkin : MonoBehaviour {

	private GameObject player;
	public bool waterFull = false;
	private float waterAmount = 0;

	public void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	public void DrinkWater () {
		if (waterFull) {
			while (player.GetComponent<Hydration> ().value < waterAmount) {
				player.GetComponent<Hydration> ().Increase ();
				SetEmpty ();
			}
		}
	}

	public void Fill(){
		waterAmount = 100.0f;
		waterFull = true;
	}

	private void SetEmpty(){
		waterAmount = 0;
		waterFull = false;
	}
}
