﻿using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable(){
		GetComponent<Animator> ().Play ("sword_swing");
	}
	

	void disableMe(){
		if(gameObject.layer.Equals(0))
			gameObject.SetActive (false);
	}
}
