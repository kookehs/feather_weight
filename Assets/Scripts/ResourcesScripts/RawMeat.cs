using UnityEngine;
using System.Collections;

public class RawMeat : MonoBehaviour {

	private GameObject player;
	private float foodAmount = 100f;
	public float distance = float.MaxValue;

	public void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	public void EatMeat () {
		while (player.GetComponent<FoodLevel> ().value < foodAmount) {
			player.GetComponent<FoodLevel> ().Increase ();
			player.GetComponent<Health> ().Decrease (10f);
		}
	}

	public void CampDistance(){
		GameObject[] campFire = GameObject.FindGameObjectsWithTag ("CampFire");

		foreach (GameObject obj in campFire) {
			obj.GetComponent<Campfire> ().CampDistance ();
			float objDistance = obj.GetComponent<Campfire> ().distance;
			if (objDistance < distance)
				distance = objDistance;
		}
	}
}
