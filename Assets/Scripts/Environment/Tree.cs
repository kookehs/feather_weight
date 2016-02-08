using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {

	public bool hasNut;
	public bool hasFallen;
	public bool isSmitten;
	public bool isPlayerNear;
	public GameObject nut;

	// Use this for initialization
	void Start () {
		hasNut = true;
		hasFallen = false;
		isSmitten = false;
	}

	// Update is called once per frame
	void Update () {

	}

	// Drop nuts on the ground
	public void DropNut () {
		if (hasNut) {
			Instantiate(nut);
			// Debug.Log ("Drop Nut");
			hasNut = false;
		}
	}

	public void GetSmitten() {
		if (!isSmitten) {
			//Do Something
			isSmitten = true;
			hasNut = false;
		}
	}

	public void Fall() {
		if (!hasFallen) {
			//Do Something
			hasFallen = true;
		}
	}
}
