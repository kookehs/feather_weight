using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SurvivalGUI : MonoBehaviour {

	Image icon;
	private float initalValue;
	public GameObject player;
	
	// Use this for initialization
	void Start () {
		icon = GetComponent<Image> ();
		player = GameObject.Find ("Player");

		switch(icon.name){
			case "HealthOverlay":
				initalValue = player.GetComponent<Health> ().health;
				break;
			case "HungerOverlay":
				initalValue = player.GetComponent<FoodLevel> ().foodLevel;
				break;
			case "ThirstOverlay":
				initalValue = player.GetComponent<Hydration> ().hydration;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch(icon.name){
			case "HealthOverlay":
				float curHealth = player.GetComponent<Health> ().health / initalValue; //this is to convert the value to be 0-1 for the fillAmound value
				if(player.GetComponent<Health> ().health.Equals(initalValue)) curHealth = 1;
				if (player.GetComponent<Health> ().health == 0) curHealth = 0;

				icon.fillAmount = curHealth;
				break;
			case "HungerOverlay":
				float curHunger = player.GetComponent<FoodLevel> ().foodLevel / initalValue; //this is to convert the value to be 0-1 for the fillAmound value
				if(player.GetComponent<Health> ().health.Equals(initalValue)) curHunger = 1;
				if (player.GetComponent<Health> ().health == 0) curHunger = 0;

				icon.fillAmount = curHunger;
				break;
			case "ThirstOverlay":
				float curHydration = player.GetComponent<Hydration> ().hydration / initalValue; //this is to convert the value to be 0-1 for the fillAmound value
				if(player.GetComponent<Health> ().health.Equals(initalValue)) curHydration = 1;
				if (player.GetComponent<Health> ().health == 0) curHydration = 0;

				icon.fillAmount = curHydration;
				break;
		}
		

		//myText.text = "Water " + player.GetComponent<Hydration> ().hydration + "\nFood " + player.GetComponent<FoodLevel> ().foodLevel + "\nHealth " + player.GetComponent<Health> ().health;
	}
}
