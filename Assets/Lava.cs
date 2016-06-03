using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour {
	private bool canhit = false;

	// Use this for initialization
	void Start () {
		
	}

	void Awake(){
		StartCoroutine (EnableLava());
	}
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider col){
		if (!canhit)
			return;
		switch (col.tag){
		case "Player":
			col.gameObject.GetComponent<PlayerMovementRB> ().receiveHit (GetComponent<Collider> (), 10, 350, "Lava");
			col.gameObject.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 150, 0));
			break;
		case "Bear":
			col.gameObject.GetComponent<BearNMA> ().receiveHit (GetComponent<Collider> (), 50, 350, "Lava");
			break;
		case "Wolf":
			col.gameObject.GetComponent<BearNMA> ().receiveHit (GetComponent<Collider> (), 50, 350, "Lava");
			break;
		case "Chicken":
			col.gameObject.GetComponent<Chicken> ().receiveHit (GetComponent<Collider> (), 0, 350, "Lava");
			col.gameObject.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 2000, 0));
			break;
		default:
			break;
		}
	}

	IEnumerator EnableLava(){
		yield return new WaitForSeconds (2f);
		canhit = true;
	}
}
