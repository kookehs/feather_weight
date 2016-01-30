using UnityEngine;
using System.Collections;

public class SwordController : MonoBehaviour {
	
	public GameObject mainChar;
	public GameObject mySword;

	// Use this for initialization
	void Start () {

		mainChar = GameObject.Find ("Player");
		mySword = GameObject.Find ("Sword");
	
	}
	
	// Update is called once per frame
	void Update () {

		//	When I click, spawn the sword at the position of the swordSpawn
		if (Input.GetMouseButtonDown (0)) {
			mySword.SetActive (true);
		}

		//	Maintain the position of the swordSpawn
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
