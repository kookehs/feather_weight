using UnityEngine;
using System.Collections;

public class TwicthAction : MonoBehaviour {

	private float startPos;

	public GameObject explosion;
	public GameObject ground;
	public float dropRate = 2.5f;

	void Awake(){
		startPos = transform.localPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.localPosition.y > ground.transform.position.y)
			transform.Translate ((Vector3.down) * dropRate * Time.deltaTime);
		else {
			explosion.SetActive (true);
			StartCoroutine ("EndExplosion");
		}
		/*if (Input.GetKey (KeyCode.P) && gameObject.name == "TwitchAction") {
			for (int i = 0; i < gameObject.transform.childCount; i++) {
				Transform child = gameObject.transform.GetChild (i);
				child.gameObject.SetActive (activateTwich);
			}
			activateTwich = !activateTwich;
		}*/
	}

	IEnumerator EndExplosion(){
		yield return new WaitForSeconds (0.5f);
		transform.parent.gameObject.SetActive (false);
	}

	void OnDisable() {
		transform.localPosition = new Vector3 (0, startPos, 0);
		explosion.SetActive (false);
	}
}
