using UnityEngine;
using System.Collections;

public class RecipesDisplay : MonoBehaviour {

	public RecipesController recControl;
	public InventoryDisplay intDisp;

	public bool focus = false;
	public bool openClose; //toggle whether the inventory is already open or not
	private float pauseTime = 0.5f;
	private bool toggleHiden = false;

	public GameObject craftingButton;

	private GameObject player;

	// Use this for initialization
	void Start () {
		GetComponent<CanvasGroup> ().alpha = 0;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		craftingButton = GameObject.Find ("CraftingButton");
		player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<PlayerMovementRB> ().mouseHovering = false;
		openClose = false;
	}

	// Update is called once per frame
	void Update () {
		//Open the inventory
		if (Input.GetKeyUp ("c")) {
			if (focus || Input.GetKey ("i"))
				toggleDisplay ();
			else if (!openClose)
				toggleDisplay ();
			
			focus = !focus;

			if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				toggleHiden = true;
			
			if (intDisp.openClose)
				intDisp.focus = false;
		}

		if(openClose){
			openClose = true;

			if(!toggleHiden){
				GetComponent<CanvasGroup> ().alpha = 1;
				GetComponent<CanvasGroup> ().blocksRaycasts = true;
				GetComponent<CanvasGroup> ().interactable = true;
				player.GetComponent<PlayerMovementRB>().mouseHovering = true;
			}
		}

		//close the inventory
		if(!openClose) {
			focus = false;
			recControl.mousePressed = false;

			if (intDisp.openClose)
				intDisp.focus = true;

			toggleHiden = false;
			GetComponent<CanvasGroup> ().alpha = 0;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
			GetComponent<CanvasGroup> ().interactable = false;
			player.GetComponent<PlayerMovementRB>().mouseHovering = false;
		}
	}

	//dialog popup that tells player they cannot craft when option is unavailable
	void OnGUI(){
		if (!recControl.isCraftable) {
			GUI.Box (new Rect (20, 10, 400, 20), "Not enough items in inventory to craft this item");
			StartCoroutine ("EndDisplayButton");
		}
	}

	//to keep the display dialog stay for a few seconds before closing
	IEnumerator EndDisplayButton(){
		yield return new WaitForSeconds(pauseTime);
		recControl.isCraftable = true;
	}

	public void toggleDisplay(){
		openClose = !openClose;
		if (openClose == true) 
			craftingButton.SetActive(false);
		else
			craftingButton.SetActive(true);
	}

	public void ForButtonPress(int num){
		if(recControl.keyCodes.ContainsKey (num)){
			recControl.mousePressed = true;
			if (recControl.category.Equals ("")) {
				recControl.category = recControl.GetHotKeyCategories (recControl.category, num);
				recControl.DisplayRecipeNames (recControl.category);
			} else {
				if (recControl.currentlySelected != null)
					recControl.CraftItem (recControl.currentlySelected);
			}
		}
	}

	public void hoverItem(int num){
		if (!recControl.category.Equals ("") && recControl.keyCodes.ContainsKey (num)) {
			recControl.currentlySelected = Resources.Load (recControl.keyCodes [num]) as GameObject;
			recControl.ShowItemRequirements (recControl.currentlySelected);
			recControl.requirements.GetComponent<RectTransform>().position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y - (recControl.requirements.GetComponent<RectTransform> ().rect.height), Input.mousePosition.z);
		} else {
			recControl.requirements.transform.GetComponent<CanvasGroup> ().alpha = 0;
		}
	}
}
