using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {

	public InventoryController intControl;
	//public RecipesDisplay recDisp;

	public bool focus = false;
	public bool openClose = false; //toggle whether the inventory is already open or not
	private bool toggleHiddenInventory = false;

	//for drag/drop feature
	private Vector3 numOrigLoc;
	private bool mouseHeld = false;
	private bool initialHold = true;

	public Sprite defaultSprite;

	public GameObject inventoryButton;

	private GameObject displayInfoWindow;

	private GameObject player;

    private GameObject plusOne;

	// Use this for initialization
	void Start () {
		displayInfoWindow = transform.FindChild ("SelectedItemDetails").gameObject;
		displayInfoWindow.GetComponent<CanvasGroup> ().alpha = 0;
		displayInfoWindow.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		displayInfoWindow.GetComponent<CanvasGroup> ().interactable = false;

		defaultSprite = transform.GetChild (0).GetComponent<Image> ().sprite;
	}

	/*public void toggleDisplay(){
		openClose = !openClose;
		if (openClose == true) {
			inventoryButton.SetActive(false);
                        GetComponents<AudioSource>()[1].Play();
		} else {
			inventoryButton.SetActive(true);
                        GetComponents<AudioSource>()[0].Play();
                }
	}*/

	public void ResetDisplaySprites ()
	{
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild (i).gameObject;
			if (child.GetComponent<Image> () != null && child.name.Contains ("Num"))
				child.GetComponent<Image> ().sprite = defaultSprite;
		}
	}

	public void ForButtonHold(GameObject button){
		int num = int.Parse(button.transform.GetChild(0).GetComponentInChildren<Text>().text);

		if (intControl.keyCodes.ContainsKey (num)) {
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

		if (intControl.keyCodes.ContainsKey (num)) {
			if (!mouseHeld) {
				intControl.mousePressed = true;

				if (intControl.currentlySelected != null)
						intControl.UseEquip ();
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
	}

	public void hoverItem(int num){
		if (intControl.keyCodes.ContainsKey (num)) {
			intControl.currentlySelected = intControl.keyCodes [num];
			intControl.ShowItemInfo (num);
			intControl.itemDetails.GetComponent<RectTransform>().position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y - (intControl.itemDetails.GetComponent<RectTransform> ().rect.height), Input.mousePosition.z);
		} else {
			intControl.itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 0;
		}
	}
}
