using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldContainer : MonoBehaviour {

	public GameObject player;
	public GameObject[] trees;
	public GameObject[] bears;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		trees = GameObject.FindGameObjectsWithTag("tree");
		bears = GameObject.FindGameObjectsWithTag ("bear");
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
                Debug.Log(trees);
		//We will return the nearest tree to the player
		GameObject nearestTree = null;
		float shortestDist = Mathf.Infinity;
		//For each tree, if the player is near that tree,
		foreach (GameObject t in trees){
			if (t.GetComponent<Tree>().isPlayerNear == true) {
				float distance = Vector3.Distance (t.transform.position, player.transform.position);
				if (distance < shortestDist)
				{
					nearestTree = t;
					shortestDist = distance;

				}
			}
		}
		return nearestTree;
	}

	public GameObject PlayerNearBearCheck(){
		Debug.Log(bears);
		//We will return the nearest bear to the player
		GameObject nearestBear = null;
		float shortestDist = Mathf.Infinity;
		//For each bear, if the player is near that bear,
		foreach (GameObject b in bears){
			if (b.GetComponent<Bear>().isPlayerNear == true) {
				float distance = Vector3.Distance (b.transform.position, player.transform.position);
				if (distance < shortestDist)
				{
					nearestBear = b;
					shortestDist = distance;

				}
			}
		}
		return nearestBear;
	}

}
