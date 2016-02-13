using UnityEngine;
using System.Collections;

public class Spear : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable(){
		GetComponent<Animator> ().Play ("sword_swing"); //need spear stab
	}
	

	void disableMe(){
		if(gameObject.layer.Equals(0))
			gameObject.SetActive (false);
	}
}
