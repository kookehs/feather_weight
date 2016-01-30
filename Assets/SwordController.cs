using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {
	
	public GameObject mainChar;

	// Use this for initialization
	void Start () {

		mainChar = GameObject.Find ("Player");
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0)) {

		}

		RaycastHit hit;
		Vector3 whereHit = Vector3.zero;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
		if (Physics.Raycast (ray, out hit)) {
			whereHit = hit.point;
			whereHit.y = mainChar.transform.position.y;
		}
		Vector3 targetDirection = whereHit - mainChar.transform.position;
		transform.rotation = Quaternion.LookRotation (targetDirection);


	
	}
}
