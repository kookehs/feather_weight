using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class WorldContainer : MonoBehaviour
{

	private float viewableRadius = 30;
	private string[] object_types_2D = { "Nut", "Bear", "Player", "Stick", "Rock", "Twine", "Rabbit", "Metal" };
	private string[] object_types_3D = { "Tree", "Rock3D", "Special_Antenna" };
	private List<GameObject> destroyed_objects = new List<GameObject> ();
	private System.Random rng = new System.Random ();

	private GameObject player;
	private Transform _camera;
	public KillsTracker kills_tracker = new KillsTracker (new Dictionary<string, int> ());
	private Dictionary<string,GameObject[]> world_objects_2D = new Dictionary<string,GameObject[]> ();
	private Dictionary<string,GameObject[]> world_objects_3D = new Dictionary<string,GameObject[]> ();
	private List<string> update2D = new List<string>();
	private List<string> update3D = new List<string>();

	public bool time_enabled = true;
	public float time_limit = 720.0f;
	public float time_elapsed = 0.0f;
	private GameObject boss;

	private bool _BOSS = false;

	public bool BOSS {
		get { return this._BOSS; }
		set { this._BOSS = value; }
	}

	// Use this for initialization
	void Start ()
	{
		boss = GameObject.Find ("like a boss");

		if (boss != null)
			boss.SetActive (false);

		player = GameObject.Find ("Player");
		GameObject camera = GameObject.Find ("Camera");
		if (camera != null)
			_camera = camera.transform;

		foreach (string type in object_types_2D)
			world_objects_2D.Add (type, GameObject.FindGameObjectsWithTag (type));
		foreach (string type in object_types_3D)
			world_objects_3D.Add (type, GameObject.FindGameObjectsWithTag (type));
		Orient2DObjects ();
	}

	void Update ()
	{
		GameObject TimeHUD = GameObject.Find ("TimeLimitHUD");

		if (TimeHUD == null)
			return;

		time_elapsed += Time.deltaTime;

		if (time_elapsed >= 1.0f && time_enabled) {
			time_limit -= time_elapsed;
			time_elapsed = 0.0f;
			int minutes = (int)(time_limit / 60);
			int seconds = (int)(time_limit % 60);
			string pad = (seconds.ToString ().Length == 1) ? "0" : "";
			TimeHUD.GetComponent<Text> ().text = minutes.ToString () + ":" + pad + seconds.ToString ();

			if (time_limit <= 0.0f) {
                                time_enabled = false;
                                UnityEngine.Debug.Log(GameObject.Find ("Monument").GetComponent<QuestController> ().landmark_discovered == false);
                                if (GameObject.Find ("Monument").GetComponent<QuestController> ().landmark_discovered == false) {
                                        UnityEngine.Debug.Log("here");
				        StartCoroutine ("GameOver", 5.0f);
                                } else {
                                        UnityEngine.Debug.Log("there");
                                        TimeHUD.SetActive(false);
                                        Vector3 spawn_point = GameObject.Find("BossLandSpawnPoint").transform.position;
                                        GameObject.Find("Player").transform.position = spawn_point;
                                        boss.SetActive(true);
                                        _BOSS = true;
                                }
                        }
		}
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		UpdateWorldObjects ();
	}

	//Input:
	//   -string: tag of the object of interest
	//Output:
	//   -GameObject: the object of interest within the viewable radius that is nearest to the player
	public GameObject GetObjectNearestPlayer (string what)
	{
		return GetNearestObject (what, player, viewableRadius);
	}

	public List<GameObject> GetAllObjectsNearPlayer (string what)
	{
		return GetAllNearbyObjects (what, player, viewableRadius);
	}

	public GameObject GetRandomObjectNearPlayer (string what)
	{
		return GetRandomNearbyObject (what, player, viewableRadius);
	}

	public float GetViewableRadius ()
	{
		return viewableRadius;
	}

	public void SetKillTracker (string[] bounties)
	{
		foreach (string bounty in bounties)
			kills_tracker.bounties [bounty] = 0;
	}

	public void SetKillTracker (string bounty)
	{
		kills_tracker.bounties [bounty] = 0;
	}

	public void UpdateKillCount (string what)
	{
		if (kills_tracker.bounties.ContainsKey (what))
			++kills_tracker.bounties [what];
	}

	public int GetKillCount ()
	{
		return kills_tracker.KillCount ();
	}

	public int GetKillCount (string what)
	{
		return kills_tracker.KillCount (what);
	}

	public double RandomChance ()
	{
		return rng.NextDouble ();
	}

	public int RandomChance (int max)
	{
		return rng.Next (max);
	}

	public int RandomChance (int min, int max)
	{
		return rng.Next (min, max);
	}

	//Input:
	//   -string: tag of the object of interest
	//   -GameObject: the target object that you want to find the nearest object of interest to
	//   -float: the radius of the circular sweep to find the object of interest conducted with the target as the center
	//Output:
	//   -GameObject: the object of interest within the viewable radius that is nearest to the player
	public GameObject GetNearestObject (string what, GameObject target, float radius)
	{
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
						// Debug.Log (nearestThing);
					minDist = dist;
				}
			}
		}
		if (minDist <= radius)
			result = nearestThing;

		if (what == "Tree") {
			// Debug.Log ("Min: " + minDist + ", Radius: " + radius);
			// Debug.Log (result);
		}

		return result;
	}

	//Input:
	//   -string: tag of the object of interest
	//   -GameObject: the target object that will be the center of the circular sweep to find the objects of interest
	//   -float: the radius of the circular sweep to find the objects of interest
	//Output:
	//   -List<GameObject>: the list of all GameObject instances of the object of interest within the given radius from the target
	public List<GameObject> GetAllNearbyObjects (string what, GameObject target, float radius)
	{
		List<GameObject> result = new List<GameObject> ();
		GameObject[] things;

		if (TryGetObject (what, out things)) {
			foreach (GameObject thing in things) {
				float dist = Vector3.Distance (thing.transform.position, target.transform.position);
				if (dist <= radius)
					result.Add (thing);
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
	public GameObject GetRandomNearbyObject (string what, GameObject target, float radius)
	{
		List<GameObject> nearby_objects = GetAllNearbyObjects (what, target, radius);
		if (nearby_objects.Count != 0)
			return nearby_objects [rng.Next (nearby_objects.Count)];
		else
			return null;
	}

	public bool IsObjectNearPlayer (GameObject what, float bound)
	{
		return IsObjectNear (what, player, bound);
	}

	public bool IsObjectNear (GameObject what, GameObject target, float bound)
	{
		return Vector3.Distance (what.transform.position, target.transform.position) <= bound;
	}

	//Input:
	//   -string: tag of the object you want to create
	//   -Vector3: where you want to create the object
	//Outcome:
	//   -the object will be created at the given position
	public void Create (Transform t, Vector3 where)
	{
		Instantiate (t, where, player.transform.rotation);
		UpdateUpdateList (t.tag);
	}

	public void Create (Transform t, Vector3 where, Quaternion rotation)
	{
		Instantiate (t, where, rotation);
		UpdateUpdateList (t.tag);
	}

	public void Create (string tag, Vector3 where)
	{
		Instantiate (Resources.Load (tag), where, player.transform.rotation);
		UpdateUpdateList (tag);
	}

	public void Create (string tag, Vector3 where, Quaternion rotation)
	{
		Instantiate (Resources.Load (tag), where, rotation);
		UpdateUpdateList (tag);
	}

	//Input:
	//   -GameObject: the object you want to remove
	//Outcome:
	//   -the object will be removed
	public void Remove (GameObject what)
	{
		destroyed_objects.Add (what);
		UpdateUpdateList (what.tag);
	}

	private void UpdateUpdateList (string tag) {
		if      (world_objects_2D.ContainsKey (tag)) { if (!update2D.Contains (tag)) update2D.Add (tag); }
		else if (world_objects_3D.ContainsKey (tag)) { if (!update3D.Contains (tag)) update3D.Add (tag); }
	}

	public void Orient2DObjects ()
	{
		foreach (var things in world_objects_2D)
			foreach (GameObject thing in things.Value) {
				Vector3 target_direction = new Vector3 (_camera.position.x, thing.transform.position.y, _camera.position.z);
				thing.transform.LookAt (target_direction);
			}
	}

	private bool TryGetObject (string what, out GameObject[] things)
	{
		return world_objects_2D.TryGetValue (what, out things) || world_objects_3D.TryGetValue (what, out things);
	}

	private void UpdateWorldObjects ()
	{
		DestroyWorldObjects ();
		foreach (string thing in update2D) {
			world_objects_2D.Remove (thing);
			world_objects_2D.Add (thing, GameObject.FindGameObjectsWithTag (thing));
		}
		foreach (string thing in update3D) {
			world_objects_3D.Remove (thing);
			world_objects_3D.Add (thing, GameObject.FindGameObjectsWithTag (thing));
		}
		update2D.Clear ();
		update3D.Clear ();
	}

	private void DestroyWorldObjects ()
	{
		foreach (var thing in destroyed_objects)
			DestroyImmediate (thing);
		destroyed_objects.Clear ();
	}

	IEnumerator GameOver (int duration)
	{
		yield return new WaitForSeconds (duration);
		Application.LoadLevel ("PlayerDeath");
	}

	// Major world changes
	private bool _killer_bunny_world = false;

	public bool killer_bunny_world { get { return this._killer_bunny_world; } }

	public void KillerBunnies ()
	{
		_killer_bunny_world = true;
		foreach (GameObject rabbit in world_objects_2D["Rabbit"])
			rabbit.GetComponent<Rabbit> ().decreaseFriendliness (10f);
	}
}
