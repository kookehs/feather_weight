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
	
	IEnumerable DelayOverWorld(){
		yield return new WaitForSeconds (0.5f);
		Debug.Log ("cor");
		overworld.Play ();
	}
}
