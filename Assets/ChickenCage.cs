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
			DestroyForfeitChicken (); // Player is punished. One chicken dies.
			DisableAllSpawners();
		}
	}

	void DestroyForfeitChicken() {
		squawk.Play ();
		Instantiate (feathers, transform.position, Quaternion.identity);
		Destroy (forfeitChicken);
		Destroy (gameObject, squawk.clip.length);
	}

	void DisableAllSpawners(){
		CreatureSpawn[] spawnerComponents = FindObjectsOfType<CreatureSpawn>(); // Get all spawners in this scene
		foreach (CreatureSpawn s in spawnerComponents) {
			s.enabled = false; // Disable spawners in this scene
		}
	}
}
