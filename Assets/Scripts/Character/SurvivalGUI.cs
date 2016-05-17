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
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch(icon.name){
			case "HealthOverlay":
				float curHealth = player.GetComponent<Health> ().health / initalValue; //this is to convert the value to be 0-1 for the fillAmound value
				if (player.GetComponent<Health> ().health.Equals (initalValue))
					curHealth = 1;
				if (player.GetComponent<Health> ().health == 0)
					curHealth = 0;

				icon.fillAmount = curHealth;
				break;
		}

		//myText.text = "Water " + player.GetComponent<Hydration> ().hydration + "\nFood " + player.GetComponent<FoodLevel> ().foodLevel + "\nHealth " + player.GetComponent<Health> ().health;
	}


}
