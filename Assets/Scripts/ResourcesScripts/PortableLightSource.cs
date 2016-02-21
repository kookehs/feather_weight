using UnityEngine;
using System.Collections;

public class PortableLightSource : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		transform.position = player.transform.position;
		transform.parent = player.transform;
	}
}
