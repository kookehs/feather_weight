using UnityEngine;
using System.Collections;

public class RecipesDisplay : MonoBehaviour {

	public RecipesController recControl;
	public bool craftingOpen = false;

	private int openClose; //toggle whether the inventory is already open or not
	private float pauseTime = 5.0f;

	// Use this for initialization
	void Start () {
		GetComponent<CanvasGroup> ().alpha = 0;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = false;
		openClose=0;
	}

	// Update is called once per frame
	void FixedUpdate () {
		//Open the inventory
		if (Input.GetKeyDown ("c") && openClose == 0) {
			GetComponent<CanvasGroup> ().alpha = 1;
			GetComponent<CanvasGroup> ().blocksRaycasts = true;
			GetComponent<CanvasGroup> ().interactable = true;
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = true;
			openClose = 1;
			craftingOpen = true;
		}

		//close the inventory
		if (Input.GetKeyDown ("c") && openClose > 2) {
			GetComponent<CanvasGroup> ().alpha = 0;
			GetComponent<CanvasGroup> ().blocksRaycasts = false;
			GetComponent<CanvasGroup> ().interactable = false;
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = false;
			openClose=0;
			craftingOpen = false;
		}

		//change the toggle value
		if (openClose > 0)
			openClose++;
	}

	public void CloseGUI(){
		GetComponent<CanvasGroup> ().alpha = 0;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GetComponent<CanvasGroup> ().interactable = false;
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = false;
		openClose=0;
		craftingOpen = false;
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
