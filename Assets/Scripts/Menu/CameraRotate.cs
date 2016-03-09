using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour {

	public GameObject pivot;
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (pivot.transform.position, Vector3.up, 5 * Time.deltaTime);
	}
}
