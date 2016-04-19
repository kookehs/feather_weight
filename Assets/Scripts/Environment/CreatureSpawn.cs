using UnityEngine;
using System.Collections;

public class CreatureSpawn : MonoBehaviour {

	public GameObject creatureToSpawn;
	public bool spawnCreatures = true;
	public GameObject target;
	private float preTimer = 0;
	private float currentTime = 0;

	// Use this for initialization
	void Start () {
		preTimer = Time.deltaTime + 5.0f;
		currentTime = Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;

		if (currentTime > preTimer && spawnCreatures) {
			GameObject creature = Instantiate (creatureToSpawn, transform.position, Quaternion.identity) as GameObject; //gets the bear cave location
			creature.transform.parent = GameObject.Find(creatureToSpawn.name + "Collection").transform;
			if (target != null) {
				Debug.Log ("Setting target now");
				creature.GetComponent<Animal> ().changeTarget (target);
			}
			preTimer = Time.deltaTime + 5.0f;
			currentTime = Time.deltaTime;
		}
	}
}
