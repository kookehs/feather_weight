using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {

	public InventoryController intControl;

	private bool openClose = false; //toggle whether the inventory is already open or not
	private float pauseTime = 5.0f;

	private GameObject player;

	// Use this for initialization
	void Start () {
		GetComponent<CanvasGroup> ().alpha = 0;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<PlayerMovementRB> ().mouseHovering = false;
		openClose = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Open the inventory
		if (Input.GetKeyUp ("i")) {
			openClose = !openClose; //toggle open close
		}

		if(openClose){
			GetComponent<CanvasGroup> ().alpha = 1;
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
			GetComponent<CanvasGroup> ().interactable = true;
			player.GetComponent<PlayerMovementRB>().mouseHovering = true;
		}

		//close the inventory
		if(!openClose) {
			GetComponent<CanvasGroup> ().alpha = 0;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
			GetComponent<CanvasGroup> ().interactable = false;
			player.GetComponent<PlayerMovementRB>().mouseHovering = false;
		}
	}
}
