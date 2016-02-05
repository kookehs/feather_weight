﻿using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {
	
	public GameObject mainChar;
	public GameObject mySwordPrefab;
	private GameObject mySword;
	private Vector3 spawnPos;

	//	Stun and stun timer
	private bool coolingDown = false;
	private float cooldownTime;
	public float cooldownLength = 1f;

	// Use this for initialization
	void Start () {

		mainChar = GameObject.Find ("Player");
		mySword = mainChar.GetComponent<Weapon>().myWeapon;
		spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + .80f);
		mySword = Instantiate (mySword, spawnPos, Quaternion.identity) as GameObject;
		mySword.transform.parent = gameObject.transform;
	
	}
	
	// Update is called once per frame
	void Update () {

		//	When I click, spawn the sword at the position of the swordSpawn
		if (Input.GetMouseButtonDown (0) && coolingDown == false) {
			mySword.SetActive (true);
			coolingDown = true;
			cooldownTime = Time.time;
		}

		//Deal with cooldown
		if (coolingDown == true) {
			if (Time.time - cooldownTime >= .5f)
				coolingDown = false;
		}

		//	Maintain the position of the swordSpawn
		RaycastHit hit;
		Vector3 whereHit = Vector3.zero;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
		if (Physics.Raycast (ray, out hit)) {
			whereHit = hit.point;
			whereHit.y = mainChar.transform.position.y;
		}
		Vector3 targetDirection = whereHit - mainChar.transform.position;
		transform.rotation = Quaternion.LookRotation (targetDirection);


	
	}
}
