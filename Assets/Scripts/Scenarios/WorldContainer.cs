using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldContainer : MonoBehaviour {

	private float viewableRadius = 30;
	private string[] object_types_2D = {"Nut", "Bear", "Stick", "Rock", "Hide"};
	private string[] object_types_3D = {"Tree"};
	private List<GameObject> destroyed_objects = new List<GameObject> ();
	private System.Random rng = new System.Random ();

	private GameObject player;
	private GameObject m_camera;
	public KillsTracker kills_tracker = new KillsTracker(new Dictionary<string, int>());
	private Dictionary<string,GameObject[]> world_objects_2D = new Dictionary<string,GameObject[]> ();
	private Dictionary<string,GameObject[]> world_objects_3D = new Dictionary<string,GameObject[]> ();

	// Use this for initialization
	void Start () {
		//Ignoring collision between characters and collectables
		IgnoreLayerCollisions ("Character", "Collectable");
		IgnoreLayerCollisions ("Character", "PassableTerrain");
		IgnoreLayerCollisions ("Collectable", "PassableTerrain");

		player = GameObject.Find ("Player");
		m_camera = GameObject.Find ("Camera");

		foreach (string type in object_types_2D) world_objects_2D.Add (type, GameObject.FindGameObjectsWithTag (type));
		foreach (string type in object_types_3D) world_objects_3D.Add (type, GameObject.FindGameObjectsWithTag (type));
		//Orient2DObjects ();

		SetKillTracker ("Bear");
	}

	// Update is called once per frame
	void LateUpdate () {
		UpdateWorldObjects ();
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

	public float GetViewableRadius() {
		return viewableRadius;
	}

	public void SetKillTracker(string[] bounties) {
		kills_tracker.bounties.Clear ();
		UpdateKillTracker (bounties);
	}

	public void SetKillTracker(string bounty) {
		kills_tracker.bounties.Clear ();
		kills_tracker.bounties.Add (bounty, 0);
	}

	public void UpdateKillTracker (string[] bounties) {
		foreach (string bounty in bounties)
			kills_tracker.bounties.Add (bounty, 0);
	}

	public void UpdateKillTracker (string bounty) {
		kills_tracker.bounties.Add (bounty, 0);
	}

	public void UpdateKillCount(string what) {
		if (kills_tracker.bounties.ContainsKey(what))
			++kills_tracker.bounties[what];
		Debug.Log (kills_tracker.KillCount(what));
	}

	public int GetKillCount() {
		return kills_tracker.KillCount();
	}

	public int GetKillCount(string what) {
		return kills_tracker.KillCount(what);
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
                                        if (what == "Tree")
                                                Debug.Log(nearestThing);
					minDist = dist;
				}
			}
		}
		if (minDist <= radius) result = nearestThing;

                if (what == "Tree") {
                        Debug.Log("Min: " + minDist + ", Radius: " + radius);
                        Debug.Log(result);
                }

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
	public void Create(Transform t, Vector3 where) {
		Instantiate (t, where, Quaternion.identity);
	}

	//Input:
	//   -GameObject: the object you want to remove
	//Outcome:
	//   -the object will be removed
	public void Remove(GameObject what) {
		destroyed_objects.Add (what);
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

	private void IgnoreLayerCollisions (string a, string b) {
		int layer_a = LayerMask.NameToLayer (a),
		    layer_b = LayerMask.NameToLayer (b);
		Physics.IgnoreLayerCollision (layer_a, layer_b);
		Physics.IgnoreLayerCollision (layer_b, layer_a);
	}

	private void UpdateWorldObjects() {
		DestroyWorldObjects();
		foreach (string thing in object_types_2D) {
			world_objects_2D.Remove (thing);
			world_objects_2D.Add (thing, GameObject.FindGameObjectsWithTag (thing));
		}
		foreach (string thing in object_types_3D) {
			world_objects_3D.Remove (thing);
			world_objects_3D.Add (thing, GameObject.FindGameObjectsWithTag (thing));
		}
	}

	private void DestroyWorldObjects() {
		foreach (var thing in destroyed_objects)
			DestroyImmediate (thing);
		destroyed_objects.Clear ();
	}
}
