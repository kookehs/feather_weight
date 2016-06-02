using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChickenCage : MonoBehaviour {

	public InventoryController inventory;
	public Currency curr;

	public GameObject tutorialText;

	public AudioSource jingle;
	public GameObject feathers;

	public Behaviour halo;

	// Use this for initialization
	void Start () {
		halo.enabled = false;
		inventory = GameObject.Find ("InventoryContainer").GetComponent<InventoryController>();
		curr = GameObject.Find ("ChickenInfo").GetComponent<Currency> ();
		tutorialText = GameObject.Find ("Tutorial_Text");
		tutorialText.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag.Equals("Player")) {
			CheckInventory ci = new CheckInventory ();
			int howManyChickens = ci.dealWithChickens (transform.FindChild("ChickenDumpSpot").gameObject, inventory);
			if (howManyChickens > 0) {
				//	If this is the first chicken ever being collected, flash tutorial text.
				if (WaveController.current_wave == 0 && curr.currency == 0) {
					tutorialText.SetActive (true);
					tutorialText.GetComponent<TutorialText> ().ActivateArrow ();
				}
				jingle.Play ();
			}
			curr.currency += howManyChickens;
			ChickenSpawner.DecreaseCount (howManyChickens);
		}
	}

	void DisableAllSpawners(){
		CreatureSpawn[] spawnerComponents = FindObjectsOfType<CreatureSpawn>(); // Get all spawners in this scene
		foreach (CreatureSpawn s in spawnerComponents) {
			s.enabled = false; // Disable spawners in this scene
		}
	}

	void TriggerBearsVsChickens() {
		GameObject[] bears = WorldContainer.GetAllInstances("Bear"); // Get all bears in this scene
		GameObject[] chickens = WorldContainer.GetAllInstances("Chicken"); // Get all chickens in this scene
		foreach (GameObject b in bears) {
			b.GetComponent<BearNMA>().changeTarget (WorldContainer.GetNearestObject("Chicken",b));
		}
	}

	public void ActivateGlow() {
		halo.enabled = true;
	}
}
