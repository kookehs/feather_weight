using UnityEngine;
using System.Collections;

public class TwicthAction : MonoBehaviour {

	private bool activateTwich = false;

	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.P)) {
			for (int i = 0; i < gameObject.transform.childCount; i++) {
				Transform child = gameObject.transform.GetChild (i);
				child.gameObject.SetActive (activateTwich);
			}
			activateTwich = !activateTwich;
		}

		/*if (twitchWindow.activeSelf)
			twitchAction.SetActive (true);
		else
			twitchAction.SetActive (false);*/
	}
}
