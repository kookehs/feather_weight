using UnityEngine;
using System.Collections;

public class CameraRotate : MonoBehaviour {

	public GameObject pivot;
	private bool rotateStart = true;
	private int childIndex = 0;
	private Camera cam;
	private Vector3 startPos;

	void Start(){
		cam = Camera.main;
		startPos = cam.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		cam.transform.RotateAround (pivot.transform.position, Vector3.up, 20 * Time.deltaTime);
		if (rotateStart)
			StartCoroutine ("RotateToNewArt");
	}

	IEnumerator RotateToNewArt(){
		rotateStart = false;
		yield return new WaitForSeconds (5.0f);
		rotateStart = true;

		if (childIndex < transform.childCount - 1)
			childIndex++;
		else
			childIndex = 0;
		
		pivot = transform.GetChild (childIndex).gameObject;
		cam.transform.position = new Vector3 (pivot.transform.position.x , startPos.y,startPos.z);
		cam.transform.eulerAngles = new Vector3(0, 0, 0);
	}
}
