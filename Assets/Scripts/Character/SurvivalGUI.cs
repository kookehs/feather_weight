using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SurvivalGUI : MonoBehaviour {

	Text myText;
	public GameObject player;
	
	// Use this for initialization
	void Start () {
		myText = GetComponent<Text> ();
		player = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () {
	
		myText.text = "Water " + player.GetComponent<Hydration> ().hydration + "\nFood " + player.GetComponent<FoodLevel> ().foodLevel + "\nHealth " + player.GetComponent<Health> ().health;
	}
}
