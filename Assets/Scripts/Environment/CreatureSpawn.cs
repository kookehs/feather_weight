using UnityEngine;
using System.Collections;

public class CreatureSpawn : MonoBehaviour {

	public GameObject creatureToSpawn;
	public bool spawnCreatures = true;
	public GameObject target;
	public float spawnFreq = 10f;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("CreateCreature", 5f, spawnFreq);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnDisable() {
		CancelInvoke ();
	}

	void CreateCreature() {
		GameObject creature = WorldContainer.Create (creatureToSpawn, transform.position, Quaternion.identity); //gets the bear cave location
		creature.transform.parent = GameObject.Find(creatureToSpawn.name + "Collection").transform;
		if (target != null) {
			creature.GetComponent<Animal> ().changeTarget (target);
		}
	}


	//CALL AT BEGINNING OF WAVE
	void UpdateSpawnFreq(float f) {
		CancelInvoke ();
		spawnFreq = f;
		InvokeRepeating ("CreateCreature", 5f, spawnFreq);
	}
}
