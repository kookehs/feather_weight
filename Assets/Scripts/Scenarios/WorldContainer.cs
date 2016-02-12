using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldContainer : MonoBehaviour {

	public float viewableRadius = 14;
	public GameObject player;
	public GameObject m_camera;

	//transforms of Instantiable objects
	public Transform tf_tree;

	private Dictionary<string,GameObject[]> world_objects_2D;
	private Dictionary<string,GameObject[]> world_objects_3D;
	private System.Random rng;

	// Use this for initialization
	void Start () {
		//Ignoring collision between characters and collectables
		int player_layer = LayerMask.NameToLayer ("Character");
		int collectable_layer = LayerMask.NameToLayer ("Collectable");
		Physics.IgnoreLayerCollision (player_layer, collectable_layer);
		Physics.IgnoreLayerCollision (collectable_layer, collectable_layer);

		world_objects_2D = new Dictionary<string,GameObject[]> ();
		world_objects_3D = new Dictionary<string,GameObject[]> ();
		player = GameObject.Find ("Player");
		m_camera = GameObject.Find ("Camera");
		string[] object_types_2D = {"nut", "bear"};
		string[] object_types_3D = {"tree"};

		foreach (string type in object_types_2D) world_objects_2D.Add (type, GameObject.FindGameObjectsWithTag (type));
		foreach (string type in object_types_3D) world_objects_3D.Add (type, GameObject.FindGameObjectsWithTag (type));

		//Orient2DObjects ();

		rng = new System.Random ();
	}

	// Update is called once per frame
	void Update () {
	}

	//Input: 
	//   -string: tag of the object of interest
	//Output:
	//   -GameObject: the object of interest within the viewable radius that is nearest to the player
	public GameObject GetObjectNearestPlayer(string what) {
		return GetNearestObject (what, player, viewableRadius);
	}

	public List<GameObject> GetAllObjectsNearPlayer(string what) {
		return GetAllNearbyObjects (what, player, viewableRadius);
	}

	public GameObject GetRandomObjectNearPlayer(string what) {
		return GetRandomNearbyObject (what, player, viewableRadius);
	}

	//Input: 
	//   -string: tag of the object of interest
	//   -GameObject: the target object that you want to find the nearest object of interest to
	//   -float: the radius of the circular sweep to find the object of interest conducted with the target as the center
	//Output: 
	//   -GameObject: the object of interest within the viewable radius that is nearest to the player
	public GameObject GetNearestObject(string what, GameObject target, float radius) {
		GameObject result = null;
		GameObject[] things;

		float minDist = Mathf.Infinity;
		GameObject nearestThing = null;
		if (TryGetObject (what, out things)) {
			foreach (GameObject thing in things) {
				float dist = Vector3.Distance (thing.transform.position, target.transform.position);
				if (dist < minDist) {
					nearestThing = thing;
					minDist = dist;
				}
			}
		}
		if (minDist <= radius) result = nearestThing;

		return result;
	}

	//Input: 
	//   -string: tag of the object of interest
	//   -GameObject: the target object that will be the center of the circular sweep to find the objects of interest
	//   -float: the radius of the circular sweep to find the objects of interest
	//Output: 
	//   -List<GameObject>: the list of all GameObject instances of the object of interest within the given radius from the target
	public List<GameObject> GetAllNearbyObjects (string what, GameObject target, float radius) {
		List<GameObject> result = new List<GameObject> ();
		GameObject[] things;

		if (TryGetObject(what, out things)) {
			foreach (GameObject thing in things) {
				float dist = Vector3.Distance (thing.transform.position, target.transform.position);
				if (dist <= radius) result.Add (thing);
			}
		}

		return result;
	}

	//Input: 
	//   -string: tag of the object of interest
	//   -GameObject: the target object that will be the center of the circular sweep to find the object of interest
	//   -float: the radius of the circular sweep to find the objects of interest
	//Output: 
	//   -GameObject: a random object of interest selected from all nearby objects of interest within the given radius
	public GameObject GetRandomNearbyObject (string what, GameObject target, float radius) {
		List<GameObject> nearby_objects = GetAllNearbyObjects (what, target, radius);
		if (nearby_objects.Count != 0)
			return nearby_objects [rng.Next (nearby_objects.Count)];
		else
			return null;
	}

	public bool IsObjectNearPlayer (GameObject what, float bound) {
		return IsObjectNear (what, player, bound);
	}

	public bool IsObjectNear (GameObject what, GameObject target, float bound) {
		return Vector3.Distance (what.transform.position, target.transform.position) <= bound;
	}

	//Input:
	//   -string: tag of the object you want to create
	//   -Vector3: where you want to create the object
	//Outcome:
	//   -the object will be created at the given position
	public void Create(string what, Vector3 where) {
		UpdateWorldObjects (what);
	}

	//Input:
	//   -GameObject: the object you want to remove
	//Outcome:
	//   -the object will be removed
	public void Remove(GameObject what) {
		string what_tag = what.tag;
		GameObject.DestroyImmediate (what);
		UpdateWorldObjects (what_tag);
	}

	public void Orient2DObjects() {
		foreach (var things in world_objects_2D)
			foreach (GameObject thing in things.Value) {
				Vector3 target = new Vector3(m_camera.transform.position.x, thing.transform.position.y, m_camera.transform.position.z);
				thing.transform.LookAt (target);
			}
	}

	private bool TryGetObject(string what, out GameObject[] things) {
		return world_objects_2D.TryGetValue (what, out things) || world_objects_3D.TryGetValue (what, out things);
	}

	private void UpdateWorldObjects(string what) {
		if (world_objects_2D.ContainsKey (what)) {
			world_objects_2D.Remove (what);
			world_objects_2D.Add (what, GameObject.FindGameObjectsWithTag (what));
		} else if (world_objects_3D.ContainsKey (what)) {
			world_objects_3D.Remove (what);
			world_objects_3D.Add (what, GameObject.FindGameObjectsWithTag (what));
		}
	}
}