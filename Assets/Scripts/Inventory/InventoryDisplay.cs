using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {

	public InventoryController intControl;
	public RecipesDisplay recDisp;

	public bool focus = false;
	public bool openClose = false; //toggle whether the inventory is already open or not
	private bool toggleHiddenInventory = false;

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
			if (focus || Input.GetKey("c"))
				openClose = !openClose; //toggle open close
			else if (!openClose || toggleHiddenInventory)
				openClose = !openClose;
			
			focus = !focus; //toggle the focus

			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				toggleHiddenInventory = true;

			//turn off the focus or the hotkeys for the recipe controller
			if (recDisp.openClose)
				recDisp.focus = false;
		}

		if(openClose){
			openClose = true;

			if(!toggleHiddenInventory){
				GetComponent<CanvasGroup> ().alpha = 1;
				GetComponent<CanvasGroup> ().blocksRaycasts = true;
				GetComponent<CanvasGroup> ().interactable = true;
				player.GetComponent<PlayerMovementRB>().mouseHovering = true;
			}
		}
		
		//close the inventory
		if(!openClose) {
			focus = false;
			intControl.mousePressed = false;

			if (recDisp.openClose)
				recDisp.focus = true;
			
			toggleHiddenInventory = false;
			GetComponent<CanvasGroup> ().alpha = 0;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
			GetComponent<CanvasGroup> ().interactable = false;
			player.GetComponent<PlayerMovementRB>().mouseHovering = false;
		}
	}
}
