using UnityEngine;
using System.Collections;

public class RecipesDisplay : MonoBehaviour {

	public RecipesController recControl;
	public InventoryDisplay intDisp;

	public bool focus = false;
	public bool openClose; //toggle whether the inventory is already open or not
	private float pauseTime = 0.5f;
	private bool toggleHiden = false;

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
		if (Input.GetKeyUp ("c")) {
			if (focus || Input.GetKey("i"))
				openClose = !openClose; //toggle open close
			else if (!openClose)
				openClose = !openClose;
			
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
}
