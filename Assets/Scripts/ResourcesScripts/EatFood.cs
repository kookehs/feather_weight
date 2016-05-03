using UnityEngine;
using System.Collections;

public class EatFood : MonoBehaviour {

	private GameObject player;
	public bool foodFull = false;
	private float foodAmount = 100f;

	public void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	// Update is called once per frame
	public void EatMeat () {
		if (player.GetComponent<Health> ().health < 100f)
			player.GetComponent<Health> ().Increase (10f);
	}
}
