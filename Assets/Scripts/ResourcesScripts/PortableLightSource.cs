using UnityEngine;
using System.Collections;

public class PortableLightSource : MonoBehaviour {
	
	public bool directionalLight = true;

	// Use this for initialization
	void Start () {
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		transform.position = player.transform.position;
		transform.parent = player.transform;
	}

	void Update(){
		if (directionalLight) {
			if (Input.GetKey (KeyCode.A))
				transform.rotation = Quaternion.Euler (0, 180, 0);
			else if (Input.GetKey (KeyCode.D))
				transform.rotation = Quaternion.Euler (0, 0, 0);
			else if (Input.GetKey (KeyCode.W))
				transform.rotation = Quaternion.Euler (270, 270, 0);
			else if (Input.GetKey (KeyCode.S))
				transform.rotation = Quaternion.Euler (270, 90, 0);
		}
	}
}
