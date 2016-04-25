using UnityEngine;
using System.Collections;

public class CreatureSpawn : MonoBehaviour {

	public GameObject creatureToSpawn;
	public bool spawnCreatures = true;
	public GameObject target;
	public float spawnFreq = 5f;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("CreateCreature", spawnFreq, spawnFreq);
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
}
