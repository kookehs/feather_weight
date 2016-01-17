﻿using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {

	public bool containsNut;
	public bool isPlayerNear;
	public GameObject player;
	public GameObject nut;

	public PlayerNearTree playerNearTreeScript;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		playerNearTreeScript = GameObject.Find ("PlayerNearTree").GetComponent<PlayerNearTree>();
		containsNut = true;
	}

	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter (Collider other) {
		Debug.Log ("Trigger\t");
		if (other.gameObject.Equals (player)) {
			isPlayerNear = true;
			Debug.Log ("Player is near");
			playerNearTreeScript.addTree (gameObject);
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.gameObject.Equals (player)) {
			isPlayerNear = false;
			playerNearTreeScript.removeTree (gameObject);
		}
	}

	// Drop nuts on the ground
	public void DropNut () {
		if (containsNut) {
			Instantiate (nut);
			Debug.Log ("Drop Nut");
			containsNut = !containsNut;
		}
	}
}