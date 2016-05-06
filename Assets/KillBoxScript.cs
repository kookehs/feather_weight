using UnityEngine;
using System.Collections;

public class KillBoxScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void onTriggerEnter(Collider other){
		if (other.gameObject.CompareTag("Player")) {
			Debug.Log ("boop");
			other.gameObject.transform.position = new Vector3 (0, 6, 0);
		}
		else
			Destroy (other.gameObject);
	}
}
