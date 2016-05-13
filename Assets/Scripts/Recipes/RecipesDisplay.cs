using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RecipesDisplay : MonoBehaviour {

	public RecipesController recControl;

	private float pauseTime = 0.5f;

	private Camera camera;

	void Start () {
		camera = Camera.main;
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

	public void ForButtonPress(int num){
		if (recControl.currentlySelected != null) {
			recControl.CraftItem (recControl.currentlySelected);
			EventSystem.current.SetSelectedGameObject(null, null);
			camera.GetComponent<CollectionCursor> ().SetHold ();
			StartCoroutine ("ResetCursor");
			} else {
				recControl.requirements.transform.GetChild(0).GetComponent<CanvasGroup> ().alpha = 0;
			}
	}

	public void hoverItem(int num){
		if (recControl.keyCodes.ContainsKey (num)) {
			//GameObject tempSelected = recControl.currentlySelected;
			//Debug.Log (tempSelected);
			recControl.currentlySelected = Resources.Load (recControl.keyCodes [num]) as GameObject;
			recControl.ShowItemRequirements (recControl.currentlySelected);

			//recControl.currentlySelected = tempSelected;

			camera.GetComponent<CollectionCursor> ().SetHover ();
		}
	}

	public void ExitHoverItem(){
		camera.GetComponent<CollectionCursor> ().SetNone ();
	}

	IEnumerator ResetCursor(){
		yield return new WaitForSeconds (0.5f);
		camera.GetComponent<CollectionCursor> ().SetHover ();
	}
}
