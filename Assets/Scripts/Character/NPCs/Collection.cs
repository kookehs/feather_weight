﻿using UnityEngine;
using System.Collections;

public class Collection : MonoBehaviour
{

	public float delay = 0.1f;
	public Color defaultCol;
	public Behaviour halo;

	private bool playerNearObject = false;
	public bool onMouseOver = false;

	private PlayerMovementRB player;
	private InventoryController inventoryController;
	private WeaponController wc;
	private Camera camera;

	void Start ()
	{
		GameObject playerObj = GameObject.FindGameObjectWithTag ("Player");

		if (playerObj != null) {
			player = playerObj.GetComponent<PlayerMovementRB> ();
			wc = playerObj.GetComponentInChildren<WeaponController> ();
		}

		if (GameObject.FindGameObjectWithTag ("InventoryUI") != null)
			inventoryController = GameObject.FindGameObjectWithTag ("InventoryUI").GetComponent<InventoryController> ();

		if (gameObject.tag != "River") {
			if (GetComponentInChildren<SpriteRenderer> () != null)
				defaultCol = GetComponentInChildren<SpriteRenderer> ().color;
			else
				defaultCol = GetComponent<Renderer> ().material.color;
		}

		camera = Camera.main;
	}

	public void OnTriggerStay (Collider other)
	{
		if (other.tag.Equals ("Player") && enabled == true) {
			//	If I am not a chicken, update count stuff
			if (gameObject.tag != "Chicken") {
				WorldContainer.UpdateCountCount (gameObject.tag);
			}
                        // Remove my name from used list of names
                        TwitchController.RemoveFromUsed(gameObject.name);
			//	Add me to inventory
			player.GetComponent<PlayerMovementRB> ().TriggerCollectAnim ();
			inventoryController.AddNewObject (gameObject); //collect the object in inventory
		}
	}

	void OnEnable ()
	{
		EnableAffordances ();
	}

	void OnDisable ()
	{
		DisableAffordances ();
	}

	public void EnableAffordances ()
	{
		//	Halo enabled
		if (halo != null)
			halo.enabled = true;
		//	Red enabled
		if (GetComponentInChildren<SpriteRenderer> () != null)
			GetComponentInChildren<SpriteRenderer> ().color = Color.red;
		else {
			GetComponent<Renderer> ().material.color = Color.red;//GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.005f);
		}
		//	Text enabled
		StartCoroutine ("DisplayObjectName"); //delay before showing the object name
	}

	public void DisableAffordances ()
	{
		//	Halo enabled
		if (halo != null)
			halo.enabled = false;
		//	Red enabled
		if (GetComponentInChildren<SpriteRenderer> () != null)
			GetComponentInChildren<SpriteRenderer> ().color = Color.white;
		else {
			GetComponent<Renderer> ().material.color = Color.white;//GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.005f);
		}
	}

	//to delay display of the object name
	IEnumerator DisplayObjectName ()
	{
		yield return new WaitForSeconds (delay);
		onMouseOver = true;
	}
}
//http://blogs.unity3d.com/2016/04/20/best-practices-for-rewarded-video-ads-2/
