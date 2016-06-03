using UnityEngine;
using System.Collections;
using System;

public class PauseMenu : MonoBehaviour {

	public GameObject playerUI;

	public void Start(){
		gameObject.transform.parent.GetComponent<CanvasGroup> ().alpha = 0;
		gameObject.transform.parent.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		gameObject.transform.parent.GetComponent<CanvasGroup> ().interactable = false;
	}

	public void Update(){
		if (Input.GetKeyUp (KeyCode.Escape)) {
			gameObject.transform.parent.GetComponent<CanvasGroup> ().alpha = gameObject.transform.parent.GetComponent<CanvasGroup> ().alpha == 0 ? 1 : 0;
			gameObject.transform.parent.GetComponent<CanvasGroup> ().blocksRaycasts = !gameObject.transform.parent.GetComponent<CanvasGroup> ().blocksRaycasts;
			gameObject.transform.parent.GetComponent<CanvasGroup> ().interactable = !gameObject.transform.parent.GetComponent<CanvasGroup> ().interactable;
		}
	}

	public void Restart(){
		try{
			GameObject[] allobs = FindObjectsOfType<GameObject>() as GameObject[];
			foreach (GameObject o in allobs) {
				if(o != gameObject && !o.name.Equals("TwitchData"))
					Destroy(o);
			}
		}catch(Exception e){
			Debug.Log ("No EventSystem" + e.Message);
		}

		Application.LoadLevel("HexLayoutChickenroom");
	}

	public void MainMenu(){
		try{
			foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>()) {
				if(o != gameObject)
					Destroy(o);
			}
		}catch(Exception e){
			Debug.Log ("No EventSystem" + e.Message);
		}
		Application.LoadLevel("MenuScreen");
	}

	public void Quit(){
		Application.Quit ();
	}
}
