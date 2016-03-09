using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {

	public InventoryController intControl;
	public RecipesDisplay recDisp;

	public bool focus = false;
	public bool openClose = false; //toggle whether the inventory is already open or not
	private float tapTime = 0.5f;
	private float lastTap = 0;
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

			//set double tap timer
			if (!toggleHiddenInventory) {
				StartCoroutine ("doubleTap");
				lastTap = Time.time;
			}

			//turn off the focus or the hotkeys for the recipe controller
			if (recDisp.openClose)
				recDisp.focus = false;
		}

		//check if the double tap has occured
		if ((Time.time - lastTap) < tapTime) {
			lastTap = Time.time;
			toggleHiddenInventory = true;
		}

		//if((!openClose && !toggleHiddenInventory) || (openClose && toggleHiddenInventory)){
		if(openClose){
			openClose = true;

			//if(!toggleHiddenInventory){
				GetComponent<CanvasGroup> ().alpha = 1;
				GetComponent<CanvasGroup> ().blocksRaycasts = true;
				GetComponent<CanvasGroup> ().interactable = true;
				player.GetComponent<PlayerMovementRB>().mouseHovering = true;
			//}

			toggleHiddenInventory = false;
		}
		//Debug.Log (openClose);
		//close the inventory
		if(!openClose) {
			focus = false;
			if (recDisp.openClose)
				recDisp.focus = true;
			GetComponent<CanvasGroup> ().alpha = 0;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
			GetComponent<CanvasGroup> ().interactable = false;
			player.GetComponent<PlayerMovementRB>().mouseHovering = false;
		}
	}

	IEnumerable doubleTap(){
		yield return new WaitForSeconds (tapTime);
		if(toggleHiddenInventory) toggleHiddenInventory = false;
	}
}
