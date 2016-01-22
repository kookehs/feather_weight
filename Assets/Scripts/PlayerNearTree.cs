using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerNearTree : Scenario {

	public GameObject player;
	public GameObject theTree;
	public List<GameObject> adjTrees;
	bool isTriggered; 

	// Use this for initialization
	void Start () {
		isTriggered = false;
		player = GameObject.Find ("Player");
		adjTrees = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (adjTrees.Count != 0) {
			isTriggered = true;
			Outcome ();
		}
	}

	public override void IsTriggered() {
	}
	
	public override void Outcome(){
		foreach (GameObject tree in adjTrees) {
			Debug.Log ("Dropping Nuts");
			tree.GetComponent<Tree>().DropNut();
		}
	}

	public void addTree(GameObject tree) {
		adjTrees.Add (tree);
	}

	public void removeTree(GameObject tree) {
		adjTrees.Remove (tree);
	}
}