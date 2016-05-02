using UnityEngine;
using System.Collections;

public class ChickenSpawner : MonoBehaviour
{
	static readonly int   max = 10;
	static          int   count = 0;
	static readonly float timer = 1f;

	// Use this for initialization
	void Start ()
	{
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
			string hex = WorldContainer.chickenhexes [WorldContainer.RandomChance (WorldContainer.chickenhexes.Length)];
			Vector3 position = GameObject.Find (hex).transform.position;
			WorldContainer.Create ("Chicken", position);
			position.y += 5;
			Vector3 euler = new Vector3 (90, -180, 0);
			Instantiate (Resources.Load ("ChickenSpawn"), position, Quaternion.Euler(euler));
			++count;
		}
	}
}

