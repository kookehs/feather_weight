using UnityEngine;
using System.Collections;

public class Chicken_Tutorial : Chicken
{

	private bool beenHit = false;
	public GameObject chickenCage;

	// Use this for initialization
	void Start ()
	{
		base.Start ();
		if (WaveController.current_wave != 0)
			Destroy (gameObject);
		gameObject.name = "HIT ME";
		chickenCage = GameObject.Find ("ChickenCage");
	}

	public override IEnumerator WaitAndEnableCollection ()
	{
		if (beenHit == false) {
			Destroy (transform.FindChild ("Mouse").gameObject);
			beenHit = true;
		}
		gameObject.name = "COLLECT ME";
		return base.WaitAndEnableCollection ();
	}

	public override void DisableCollection ()
	{
		base.DisableCollection ();
		gameObject.name = "HIT ME";
	}

	public void OnTriggerEnter (Collider other)
	{
		if (other.tag.Equals ("Player") && iAmCollectable != null && iAmCollectable.enabled == true) {
			chickenCage.GetComponent<ChickenCage> ().ActivateGlow ();
		}
		/*if (other.tag.Equals ("Player"))
			Debug.Log ("Hit by player.");
		if (other.tag.Equals ("Sword_Stone"))
			Debug.Log ("Hit by a sword, m8.");*/
	}


}
