using UnityEngine;
using System.Collections;

public class ChickenSpawner : MonoBehaviour
{
	static readonly int   max = 10;
	public static          int   count = 0;
	static readonly float timer = 1f;
    static GameObject chicken_collection;
	static bool _spawning = false;

	public static bool spawning {
		set { _spawning = value; }
	}

	void Awake() {
		_spawning = false;
	}

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
		if (_spawning && count < max) {
                        chicken_collection = GameObject.Find("ChickenCollection");
			string hex = WorldContainer.hexes [WorldContainer.RandomChance (WorldContainer.hexes.Length)];
            GameObject obj = GameObject.Find(hex);

            if (obj == null)
            {
                return;
            }

            Vector3 position = obj.transform.position;
			GameObject chicken = WorldContainer.Create ("Chicken", position);
                        chicken.transform.SetParent(chicken_collection.transform);
                        chicken.name = TwitchController.RandomUser();
			position.y += 5;
			Vector3 euler = new Vector3 (90, -180, 0);
			Instantiate (Resources.Load ("ChickenSpawn"), position, Quaternion.Euler(euler));
			++count;
		}
	}
}
