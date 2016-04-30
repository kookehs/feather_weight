using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {

	public AudioSource overworld;
	//AudioSource ambient;

	// Use this for initialization
	void Start () {
		overworld.Pause ();
		StartCoroutine ("DelayOverWorld");
	}
	
	IEnumerator DelayOverWorld(){
		yield return new WaitForSeconds (4.0f);
		overworld.Play ();
	}
}
