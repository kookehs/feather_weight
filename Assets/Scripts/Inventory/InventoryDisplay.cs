using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {

	public InventoryController intControl;

	//for drag/drop feature
	private Vector3 numOrigLoc;
	private bool mouseHeld = false;
	private Vector3 itemDefaultLoc;

	public Sprite defaultSprite;

	private GameObject displayInfoWindow;
	public GameObject itemDetails;

	// Use this for initialization
	void Start () {
		displayInfoWindow = transform.FindChild ("SelectedItemDetails").gameObject;
		displayInfoWindow.GetComponent<CanvasGroup> ().alpha = 0;
		displayInfoWindow.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		displayInfoWindow.GetComponent<CanvasGroup> ().interactable = false;

		itemDefaultLoc = itemDetails.transform.localPosition;

		defaultSprite = transform.GetChild (0).GetComponent<Image> ().sprite;
	}

	public void ResetDisplaySprites ()
	{
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild (i).gameObject;
			if (child.GetComponent<Image> () != null && child.name.Contains ("Num"))
				child.GetComponent<Image> ().sprite = defaultSprite;
		}
		itemDetails.transform.localPosition = itemDefaultLoc;
	}

	public void ForButtonHold(GameObject button){
		int num = int.Parse(button.transform.GetChild(0).GetComponentInChildren<Text>().text);
		num--;

		if (intControl.inventoryItems.Count > num && num != -1 && intControl.inventoryItems.Count != 0) {
			button.GetComponent<Image> ().color = Color.black;

			mouseHeld = true;

			button.transform.position = Input.mousePosition;
		}
	}

	public void StartDrag(GameObject button){
		int num = int.Parse(button.transform.GetChild(0).GetComponentInChildren<Text>().text);
		num--;

		if (intControl.inventoryItems.Count > num && num != -1 && intControl.inventoryItems.Count != 0) {
			numOrigLoc = button.transform.position;
		}
	}

	public void ForButtonPress(int num){
		num--;
		transform.GetChild (num).GetComponent<Image> ().color = Color.white;

		if (intControl.inventoryItems.Count > num && num != -1 && intControl.inventoryItems.Count != 0) {
			if (!mouseHeld) {
				intControl.mousePressed = true;

				if (intControl.currentlySelected != -1 && intControl.currentlySelected < intControl.inventoryItems.Count && !Application.loadedLevelName.Equals("ShopCenter"))
						intControl.UseEquip ();
			} else {
				mouseHeld = false;

				GameObject button = transform.GetChild (num).gameObject;
				button.transform.position = numOrigLoc;

				if (numOrigLoc.x + 50 > button.transform.position.x || numOrigLoc.y + 50 < button.transform.position.y) {
					intControl.currentlySelected = num;
					intControl.RemoveObject ();
				}
			}
		}
	}

	public void hoverItem(int num){
		num--;

		if (intControl.inventoryItems.Count > num && num != -1 && intControl.inventoryItems.Count!= 0) {
			intControl.currentlySelected = num;
			ShowItemInfo (num);
			itemDetails.GetComponent<RectTransform>().position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y + 10, Input.mousePosition.z);
		} else {
			itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 0;
		}
	}

	public void endHover(){
		itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 0;
		intControl.currentlySelected = -1;
	}

	public void ShowItemInfo(int numI){
		string info = "X - Discard";
		if (intControl.inventoryItems [numI].name == "EquipedWeapon")
			info = "Currently Equipped";

		itemDetails.GetComponentInChildren<Text> ().text = intControl.inventoryItems [numI].tag + "\n" + info;
		itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 1;
		itemDetails.transform.localPosition = itemDefaultLoc;
	}
}
