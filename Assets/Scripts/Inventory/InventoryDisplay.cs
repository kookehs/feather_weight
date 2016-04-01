using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {

	public InventoryController intControl;
	public RecipesDisplay recDisp;

	public bool focus = false;
	public bool openClose = false; //toggle whether the inventory is already open or not
	private bool toggleHiddenInventory = false;

	//for drag/drop feature
	private Vector3 numOrigLoc;
	private bool mouseHeld = false;
	private bool initialHold = true;

	public GameObject inventoryButton;

	private GameObject player;

	// Use this for initialization
	void Start () {
		GetComponent<CanvasGroup> ().alpha = 0;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		inventoryButton = GameObject.Find ("InventoryButton");
		player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<PlayerMovementRB> ().mouseHovering = false;
		openClose = false;
	}
	
	// Update is called once per frame
	void Update () {
		//Open the inventory
		if (Input.GetKeyUp ("i")) {
			if (focus || Input.GetKey ("c"))
				toggleDisplay (); //toggle open close
			else if (!openClose || toggleHiddenInventory)
				toggleDisplay ();
			
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

	public void toggleDisplay(){
		openClose = !openClose;
		if (openClose == true) 
			inventoryButton.SetActive(false);
		else
			inventoryButton.SetActive(true);
	}

	public void ForButtonHold(GameObject button){
		int num = int.Parse(button.transform.GetChild(0).GetComponentInChildren<Text>().text);

		if (!intControl.category.Equals ("") && intControl.keyCodes.ContainsKey (num)) {
			button.GetComponent<Image> ().color = Color.black;

			mouseHeld = true;
			if (initialHold) {
				numOrigLoc = button.transform.position;
				initialHold = false;
			}

			button.transform.position = Input.mousePosition;
		}
	}

	public void ForButtonPress(int num){
		transform.GetChild (num - 1).GetComponent<Image> ().color = Color.white;

		if (intControl.keyCodes.ContainsKey (num) && !mouseHeld) {
			intControl.mousePressed = true;

			if (intControl.category.Equals ("")) {
				intControl.category = intControl.GetHotKeyValues (intControl.category, num);
				intControl.PrintOutObjectNames ();
			} else {
				if (intControl.currentlySelected != null)
					intControl.UseEquip ();
			}
		} else {
			mouseHeld = false;
			initialHold = false;

			GameObject button = transform.GetChild (num - 1).gameObject;
			button.transform.position = numOrigLoc;

			if (numOrigLoc.x + 50 > button.transform.position.x || numOrigLoc.y + 50 < button.transform.position.y) {
				intControl.currentlySelected = intControl.keyCodes [num];
				intControl.RemoveObject ();
			}
		}
	}

	public void hoverItem(int num){
		if (!intControl.category.Equals ("") && intControl.keyCodes.ContainsKey (num)) {
			intControl.currentlySelected = intControl.keyCodes [num];
			intControl.ShowItemInfo (num);
			intControl.itemDetails.GetComponent<RectTransform>().position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y - (intControl.itemDetails.GetComponent<RectTransform> ().rect.height), Input.mousePosition.z);
		} else {
			intControl.itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 0;
		}
	}
}
