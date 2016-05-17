using UnityEngine;
using System.Collections;

public class ChickenSpawner : MonoBehaviour
{
	static readonly int   max = 10;
	public static          int   count = 0;
	static readonly float timer = 1f;
        static GameObject chicken_collection;

	// Use this for initialization
	void Start ()
	{
                chicken_collection = GameObject.Find("ChickenCollection");
		InvokeRepeating ("Spawn", 0, timer);
	}

	void OnApplicationQuit() {
		CancelInvoke ();
	}

	public static void DecreaseCount() {
		if (count > 0) --count;
	}

	public static void DecreaseCount(int n) {
		if (count > 0) count -= n;
	}

	protected virtual void Spawn() {
		if (count < max) {
                        chicken_collection = GameObject.Find("ChickenCollection");
			string hex = WorldContainer.hexes [WorldContainer.RandomChance (WorldContainer.hexes.Length)];
			Vector3 position = GameObject.Find (hex).transform.position;
			GameObject chicken = WorldContainer.Create ("Chicken", position);
                        chicken.transform.SetParent(chicken_collection.transform);
                        chicken.name = TwitchController.RandomUser();
			position.y += 5;
			Vector3 euler = new Vector3 (90, -180, 0);
			Instantiate (Resources.Load ("Particle Effects/ChickenSpawn"), position, Quaternion.Euler(euler));
			++count;
		}
	}
}
