using UnityEngine;
using System.Collections;

public class NightCeature : MonoBehaviour {

	public GameObject weatherController;
	private GameObject[] wolves;

	void Start(){
		wolves = GameObject.FindGameObjectsWithTag ("Wolf");
	}
	
	// Update is called once per frame
	void Update () {
		if (weatherController.GetComponent<WeatherController> ().GetTimeOfDay ().Equals ("NIGHT")) {
			for (int i = 0; i < wolves.Length; i++) {
				wolves [i].SetActive (true);
			}
		} else {
			for (int i = 0; i < wolves.Length; i++) {
				wolves [i].SetActive (false);
			}
		}
	}
}
