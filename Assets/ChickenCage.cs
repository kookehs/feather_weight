using UnityEngine;
using System.Collections;

public class ChickenCage : MonoBehaviour {

	public GameObject forfeitChicken;

	public AudioSource squawk;
	public GameObject feathers;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c) {
		if (c.tag.Equals("Bear")) {
			squawk.Play ();
			Instantiate (feathers, transform.position, Quaternion.identity);
			Destroy (forfeitChicken);
			Destroy (gameObject, squawk.clip.length);
		}
	}
}
