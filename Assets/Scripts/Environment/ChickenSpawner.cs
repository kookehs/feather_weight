using UnityEngine;
using System.Collections;

public class ChickenSpawner : MonoBehaviour
{
	static readonly int   chickens_max = 10;
	static          int   chickens_cnt = 0;
	static readonly float chicken_timer = 1f;

	// Use this for initialization
	void Start ()
	{
		InvokeRepeating ("SpawnChicken", 0, chicken_timer);
	}

	void OnApplicationQuit() {
		CancelInvoke ();
	}
	
	public static void DecreaseCount() {
		if (chickens_cnt > 0) --chickens_cnt;
	}

	public static void DecreaseCount(int n) {
		if (chickens_cnt > 0) chickens_cnt -= n;
	}

	void SpawnChicken() {
		if (chickens_cnt < chickens_max) {
			string hex = WorldContainer.chickenhexes [WorldContainer.RandomChance (WorldContainer.chickenhexes.Length)];
			Vector3 position = GameObject.Find (hex).transform.position;
			WorldContainer.Create ("Chicken", position);
			position.y += 5;
			Vector3 euler = new Vector3 (90, -180, 0);
			Instantiate (Resources.Load ("ChickenSpawn"), position, Quaternion.Euler(euler));
			++chickens_cnt;
		}
	}
}

