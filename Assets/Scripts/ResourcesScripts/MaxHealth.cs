using UnityEngine;
using System.Collections;

public class MaxHealth : MonoBehaviour {

	private GameObject player;
	
	public void PurchaseHealth(){
		player = GameObject.FindGameObjectWithTag ("Player");
		player.GetComponent<Health> ().maxHealth += 10;
	}
}
