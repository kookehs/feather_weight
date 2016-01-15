using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNearTree : Scenario {

	public GameObject player;
	public GameObject theTree;
	List<GameObject> adjTrees;
	bool isTriggered; 

	// Use this for initialization
	void Start () {
		isTriggered = false;
		player = GameObject.Find ("Player");
		adjTrees = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	void IsTriggered() {
	}
	
	void Outcome(){
	}

	public void addTree(GameObject tree) {
		adjTrees.Add (tree);
	}

	public void removeTree(GameObject tree) {
		adjTrees.Remove (tree);
	}
}