using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldContainer : MonoBehaviour {

	public float viewableRadius = 14;
	public GameObject player;
	public GameObject m_camera;
	Dictionary<string,GameObject[]> world_objects;

	// Use this for initialization
	void Start () {
		//Ignoring collision between characters and collectables
		int player_layer = LayerMask.NameToLayer ("Character");
		int collectable_layer = LayerMask.NameToLayer ("Collectable");
		Physics.IgnoreLayerCollision (player_layer, collectable_layer);
		Physics.IgnoreLayerCollision (collectable_layer, collectable_layer);

		world_objects = new Dictionary<string,GameObject[]> ();
		player = GameObject.Find ("Player");
		m_camera = GameObject.Find ("Camera");
		string[] object_types = {"tree", "nut"};

		foreach (string type in object_types) {
			world_objects.Add (type, GameObject.FindGameObjectsWithTag (type));
		}
	}

	// Update is called once per frame
	void Update () {
	}

	//	Precondition:
	//		None
	//
	//	Postcondition:
	//		If there are trees near the player, return the nearest one.
	//		If there are no trees near the player, return null.
	public GameObject PlayerNearTreeCheck(){
		return NearestTo ("tree", player);
	}

	public GameObject NearestToPlayer(string what) {
		return NearestTo (what, player);
	}

	public GameObject NearestTo(string what, GameObject target) {
		GameObject result = null;
		GameObject[] things;

		float minDist = Mathf.Infinity;
		GameObject nearestThing = null;
		if (world_objects.TryGetValue (what, out things)) {
			foreach (GameObject thing in things) {
				float dist = Vector3.Distance (thing.transform.position, target.transform.position);
				if (dist < minDist) {
					nearestThing = thing;
					minDist = dist;
				}
			}
		}
		if (minDist <= viewableRadius) result = nearestThing;

		return result;
	}

	public void Update(string what) {
		GameObject[] things;
		world_objects.TryGetValue (what, out things);
		Debug.Log (things.GetLength (0));
		world_objects.Remove (what);
		world_objects.Add (what, GameObject.FindGameObjectsWithTag (what));
		world_objects.TryGetValue (what, out things);
		Debug.Log (things.GetLength(0));
	}
}
