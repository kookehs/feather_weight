using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {

	public InventoryController intControl;
	public bool inventoryOpen = false;

	private int openClose; //toggle whether the inventory is already open or not
	private float pauseTime = 5.0f;

	// Use this for initialization
	void Start () {
		GetComponent<CanvasGroup> ().alpha = 0;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		//GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = false;
		openClose=0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Open the inventory
		if (Input.GetKeyDown ("i") && openClose == 0) {
			GetComponent<CanvasGroup> ().alpha = 1;
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
			GetComponent<CanvasGroup> ().interactable = true;
			//GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = true;
			openClose = 1;
			inventoryOpen = true;
		}

		//close the inventory
		if (Input.GetKeyDown ("i") && openClose > 2) {
			GetComponent<CanvasGroup> ().alpha = 0;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
			GetComponent<CanvasGroup> ().interactable = false;
			//GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = false;
			openClose=0;
			inventoryOpen = false;
		}

		//change the toggle value
		if (openClose > 0)
			openClose++;
	}

	public void CloseGUI(){
		GetComponent<CanvasGroup> ().alpha = 0;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		//GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = false;
		openClose=0;
		inventoryOpen = false;
	}
}
