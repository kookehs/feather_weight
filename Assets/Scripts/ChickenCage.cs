using UnityEngine;
using System.Collections;

public class ChickenCage : MonoBehaviour {

	public InventoryController inventory;
	public Currency curr;

	public AudioSource jingle;
	public GameObject feathers;

	// Use this for initialization
	void Start () {
		inventory = GameObject.Find ("InventoryContainer").GetComponent<InventoryController>();
		curr = GameObject.Find ("ChickenInfo").GetComponent<Currency> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag.Equals("Player")) {
			jingle.Play ();
			CheckInventory ci = new CheckInventory ();
			int howManyChickens = ci.dealWithChickens (transform.FindChild("ChickenDumpSpot").gameObject, inventory);
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
}
