using UnityEngine;
using System.Collections;

public class ChickenCage : MonoBehaviour {

	public GameObject forfeitChicken;

	public AudioSource squawk;
	public GameObject feathers;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag.Equals("Bear")) {
			Debug.Log ("A bear has collided with the cage.");
			WorldContainer.Remove (c.gameObject);
			Destroy (c.gameObject);
			DestroyForfeitChicken (); // Player is punished. One chicken dies.
			DisableAllSpawners();	// Spawning ceases
			TriggerBearsVsChickens ();	// Bears now chase chickens
			Destroy (gameObject, squawk.clip.length);	// Destroy this object
		}
	}

	void DestroyForfeitChicken() {
		squawk.Play ();
		Instantiate (feathers, transform.position, Quaternion.identity);
		Destroy (forfeitChicken);
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
