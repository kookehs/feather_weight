using UnityEngine;
using System.Collections;

public class Campfire : MonoBehaviour {

	public bool isActive = false;
	public float distance = float.MaxValue;

	private GameObject player;

	public void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	public void CampDistance(){
		if(isActive)
			distance = Vector3.Distance (player.transform.position, transform.position);
		else
			distance = float.MaxValue;
	}
}
