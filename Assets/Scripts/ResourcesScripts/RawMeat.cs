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
		while (player.GetComponent<FoodLevel> ().foodLevel < foodAmount) {
			player.GetComponent<FoodLevel> ().increaseFoodLevel ();
			player.GetComponent<Health> ().decreaseHealth (10);
		}
	}

	public void CampDistance(){
		GameObject[] campFire = GameObject.FindGameObjectsWithTag ("CampFire");

		foreach (GameObject obj in campFire) {
			float objDistance = Vector3.Distance (player.transform.position, obj.transform.position);
			if (objDistance < distance)
				distance = objDistance;
		}
	}
}
