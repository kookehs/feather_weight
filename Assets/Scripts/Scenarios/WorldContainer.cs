using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldContainer : MonoBehaviour {

	public GameObject player;
	public GameObject[] trees;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		trees = GameObject.FindGameObjectsWithTag("tree");
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

}
