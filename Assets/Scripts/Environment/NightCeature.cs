using UnityEngine;
using System.Collections;

public class NightCeature : MonoBehaviour {

	public GameObject weatherController;
	public string creatureTagName;
	public string dayTime;

	private GameObject[] creatures;

	void Start(){
		creatures = GameObject.FindGameObjectsWithTag (creatureTagName);
	}
	
	// Update is called once per frame
	void Update () {
		if (weatherController.GetComponent<WeatherController> ().GetTimeOfDay ().Equals (dayTime)) {
			for (int i = 0; i < creatures.Length; i++) {
				creatures [i].SetActive (true);
			}
		} else {
			for (int i = 0; i < creatures.Length; i++) {
				creatures [i].SetActive (false);
			}
		}
	}
}
