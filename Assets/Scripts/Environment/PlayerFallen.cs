﻿using UnityEngine;
using System.Collections;

public class PlayerFallen : MonoBehaviour {

	void OnTriggerEnter(Collider obj){
		if(obj.tag != "Ground")
			Destroy (obj.gameObject);

		//Reload game for now till a failed game scene is made
		if (obj.tag == "Player")
			Application.LoadLevel (Application.loadedLevel);
	}
}
