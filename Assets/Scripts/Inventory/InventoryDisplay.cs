using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

	private Camera camera;
	private GameObject sellPopup;
	private int curNumForSell = -1;

	// Use this for initialization
	void Start () {
		displayInfoWindow = transform.FindChild ("SelectedItemDetails").gameObject;
		sellPopup = GameObject.Find ("ConfirmSell").gameObject;
		displayInfoWindow.GetComponent<CanvasGroup> ().alpha = 0;
		displayInfoWindow.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		displayInfoWindow.GetComponent<CanvasGroup> ().interactable = false;

		itemDefaultLoc = itemDetails.transform.localPosition;

		defaultSprite = transform.GetChild (1).GetComponent<Image> ().sprite;

		camera = Camera.main;
	}

	public void ResetDisplaySprites ()
	{
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild (i).gameObject;
			if (child.GetComponent<Image> () != null && child.name.Contains ("Num")) {
				child.GetComponent<Image> ().sprite = defaultSprite;
				child.transform.GetChild (0).gameObject.SetActive (false);
			}
		}
		itemDetails.transform.localPosition = itemDefaultLoc;
	}

	public void ForButtonHold(GameObject button){
		int num = int.Parse(button.transform.GetChild(1).GetComponentInChildren<Text>().text);
		num--;

		if (intControl.inventoryItems.Count > num && num != -1 && intControl.inventoryItems.Count != 0) {
			button.GetComponent<Image> ().color = Color.black;

			mouseHeld = true;

			button.transform.position = Input.mousePosition;

			Camera.main.GetComponent<CollectionCursor> ().SetHold ();
		}
	}

	public void StartDrag(GameObject button){
		int num = int.Parse(button.transform.GetChild(1).GetComponentInChildren<Text>().text);
		num--;

		if (intControl.inventoryItems.Count > num && num != -1 && intControl.inventoryItems.Count != 0) {
			numOrigLoc = button.transform.position;
			Camera.main.GetComponent<CollectionCursor> ().SetHold ();
		}
	}

	public void ForButtonPress(int num){
		num--;
		transform.GetChild (num).GetComponent<Image> ().color = Color.white;

		if (intControl.inventoryItems.Count > num && num != -1 && intControl.inventoryItems.Count != 0) {
			curNumForSell = num;
			if (!mouseHeld || intControl.inventoryItems[num].tag.Equals ("CampFire")) {
				intControl.mousePressed = true;

				//to equip or use your items (only equiping is possible in the shop)
				if (intControl.currentlySelected != -1 && intControl.currentlySelected < intControl.inventoryItems.Count &&
						(!Application.loadedLevelName.Equals ("ShopCenter") || (intControl.inventoryItems[num].tag.Contains ("Sword") || intControl.inventoryItems[num].tag.Contains ("Spear") ||
							intControl.inventoryItems[num].tag.Contains ("Hammer") || intControl.inventoryItems[num].tag.Contains ("Axe") || intControl.inventoryItems[num].tag.Contains ("Net")))) {
					intControl.UseEquip ();
					Camera.main.GetComponent<CollectionCursor> ().SetConfirm ();

					StartCoroutine ("ChangeCursorBack");
				}
			} else {
				mouseHeld = false;

				GameObject button = transform.GetChild (num).gameObject;
				button.transform.position = numOrigLoc;

				//remove items in the game
				if ((numOrigLoc.x + 50 > button.transform.position.x || numOrigLoc.y + 50 < button.transform.position.y) && !Application.loadedLevelName.Equals ("ShopCenter")) {
					intControl.currentlySelected = num;
					intControl.RemoveObject ();
				}

				//to sell items in the shop
				if ((numOrigLoc.x + 50 > button.transform.position.x || numOrigLoc.y + 50 < button.transform.position.y) && Application.loadedLevelName.Equals ("ShopCenter")) {
					intControl.currentlySelected = num;
					//open window
					sellPopup.GetComponent<CanvasGroup> ().alpha = 1;
					sellPopup.GetComponent<CanvasGroup> ().blocksRaycasts = true;
					sellPopup.GetComponent<CanvasGroup> ().interactable = true;
				}
			}
		}
	}

	IEnumerator ChangeCursorBack(){
		yield return new WaitForSeconds (0.5f);
		Camera.main.GetComponent<CollectionCursor> ().SetHover ();
	}

	public void hoverItem(int num){
		num--;

		if (intControl.inventoryItems.Count > num && num != -1 && intControl.inventoryItems.Count!= 0) {
			intControl.currentlySelected = num;
			ShowItemInfo (num);
			itemDetails.GetComponent<RectTransform>().position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y + 10, Input.mousePosition.z);
			if (WaveController.current_time > 0f)
				Camera.main.GetComponent<CollectionCursor> ().SetHover ();
		} else {
			itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 0;
		}
	}

	public void endHover(){
		itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 0;
		intControl.currentlySelected = -1;
		Camera.main.GetComponent<CollectionCursor> ().SetDefault ();
	}

	public void ShowItemInfo(int numI){
		string info = "Hold Shift - Discard";
		if (intControl.inventoryItems [numI].name == "EquipedWeapon")
			info = "Currently Equipped";

		itemDetails.GetComponentInChildren<Text> ().text = intControl.inventoryItems [numI].tag + "\n" + info;
		itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 1;
	}

	public void ResetItemDetailsLoc(){
		itemDetails.transform.localPosition = itemDefaultLoc;
	}

	public void ConfirmSell(){
		intControl.currentlySelected = curNumForSell;
		curNumForSell = -1;
		intControl.RemoveObject ();
		Camera.main.GetComponent<CollectionCursor> ().SetDefault ();

		sellPopup.GetComponent<CanvasGroup> ().alpha = 0;
		sellPopup.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		sellPopup.GetComponent<CanvasGroup> ().interactable = false;
	}

	public void CanelSell(){
		intControl.currentlySelected = -1;
		curNumForSell = -1;
		Camera.main.GetComponent<CollectionCursor> ().SetDefault ();
		sellPopup.GetComponent<CanvasGroup> ().alpha = 0;
		sellPopup.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		sellPopup.GetComponent<CanvasGroup> ().interactable = false;
	}

	void OnLevelWasLoaded(int level){
		camera = Camera.main;
		if(intControl.transform.parent.FindChild ("EventSystem").gameObject.activeSelf && EventSystem.current == null)
			EventSystem.current = intControl.transform.parent.FindChild ("EventSystem").GetComponent<EventSystem>();
	}
}
