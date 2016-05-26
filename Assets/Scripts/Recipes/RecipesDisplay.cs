using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class RecipesDisplay : MonoBehaviour {

	public RecipesController recControl;

	private float pauseTime = 0.5f;
	private GameObject buyPopup;

	private Camera camera;

	void Start () {
		camera = Camera.main;
		buyPopup = GameObject.Find ("ConfirmPurchase").gameObject;
	}

	//to keep the display dialog stay for a few seconds before closing
	IEnumerator EndDisplayButton(){
		yield return new WaitForSeconds(pauseTime);
		recControl.isCraftable = true;
	}

	public void ForButtonPress(){
		if (recControl.currentlySelected != null && recControl.CanBuy(recControl.currentlySelected)) {
			buyPopup.GetComponent<CanvasGroup> ().alpha = 1;
			buyPopup.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			buyPopup.GetComponent<CanvasGroup> ().interactable = true;
			camera.GetComponent<CollectionCursor> ().SetDefault ();
		} else {
			recControl.requirements.transform.GetChild(1).GetComponent<CanvasGroup> ().alpha = 0;
			recControl.description.transform.GetComponent<CanvasGroup> ().alpha = 0;
		}
	}

	public void ConfirmPurchase(){
		recControl.CraftItem (recControl.currentlySelected);
		EventSystem.current.SetSelectedGameObject(null, null);
		camera.GetComponent<CollectionCursor> ().SetDefault ();

		buyPopup.GetComponent<CanvasGroup> ().alpha = 0;
		buyPopup.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		buyPopup.GetComponent<CanvasGroup> ().interactable = false;
	}

	public void CancelPurchase(){
		recControl.currentlySelected = null;
		camera.GetComponent<CollectionCursor> ().SetDefault ();
		buyPopup.GetComponent<CanvasGroup> ().alpha = 0;
		buyPopup.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		buyPopup.GetComponent<CanvasGroup> ().interactable = false;
	}

	public void hoverItem(int num){
		if (recControl.keyCodes.ContainsKey (num) && buyPopup.GetComponent<CanvasGroup> ().alpha == 0) {
			//GameObject tempSelected = recControl.currentlySelected;
			//Debug.Log (tempSelected);
			recControl.currentlySelected = Resources.Load (recControl.keyCodes [num]) as GameObject;
			recControl.ShowItemRequirements (recControl.currentlySelected);

			//recControl.currentlySelected = tempSelected;

			camera.GetComponent<CollectionCursor> ().SetHover ();
		}
	}

	public void HoverButton(){
		camera.GetComponent<CollectionCursor> ().SetHover ();
	}

	public void ExitHoverItem(){
        if (camera == null)
        {
            return;
        }
		camera.GetComponent<CollectionCursor> ().SetDefault ();
	}

	IEnumerator ResetCursor(){
		yield return new WaitForSeconds (0.5f);
		camera.GetComponent<CollectionCursor> ().SetHover ();
	}
}
