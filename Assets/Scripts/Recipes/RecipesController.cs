using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RecipesController : MonoBehaviour {

	public GameObject numSlot;
	public GameObject itemsDisplay;
	private List<GameObject> contents; //text object that will diaply the recipes list
	public GameObject requirements;
	public GameObject description;
	public Sprite defaultSprite;
	public bool isCraftable = true; //to determine whether or not to display the diolog telling user the item cannot be crafted
	public bool mousePressed = false;
	public Dictionary<int, string> keyCodes;

	//data variables for recipes and user's inventory
	private GameObject inventory;
	private ReadRecipeJSON jsonData;
	private Currency checkCurrency;
	private int preCurrency = 0;
	private Dictionary<string, GameItems> recipeItems;
	private int[] teirLevels = new int[3]{3, 6, 9}; //the teirs timeline waves end 3(4) teir1 items added at 0, 6(7) teir2 items added at 4, 9(10) teir3 items added at 7
	private int currentTeirLevel = 0;
	public bool alreadyBoughtHealth = false;

	private Vector3 requirementsDefaultLoc;
	public GameObject currentlySelected;

	// Use this for initialization
	public void Start () {
		recipeItems = new Dictionary<string, GameItems>();
		contents = new List<GameObject> ();

		checkCurrency = GameObject.Find ("ChickenInfo").GetComponent<Currency> ();
		preCurrency = checkCurrency.currency;
		inventory = GameObject.FindGameObjectWithTag ("InventoryUI");

		jsonData = new ReadRecipeJSON ();
		recipeItems = jsonData.GetRecipeItemsList ();

		if (WaveController.current_wave > teirLevels [currentTeirLevel] && currentTeirLevel < teirLevels.Length)
			currentTeirLevel += 1;
		
		DisplayRecipeNames ();

		requirements.transform.GetChild(1).GetComponent<CanvasGroup> ().alpha = 0;
		requirementsDefaultLoc = requirements.transform.position;
		description.transform.GetComponent<CanvasGroup> ().alpha = 0;
	}
		
	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			currentlySelected = null;
			mousePressed = false;
		}

		if (checkCurrency.currency != preCurrency) {
			preCurrency = checkCurrency.currency;
			UpdateItemAvailablity ();
		}

		//now determine and select the item through a new tear of hotkeys
		currentlySelected = GetHotKeyValues(currentlySelected, -1);
	}

	//determines if a key was pressed and determine the assosiated value for that button press
	public GameObject GetHotKeyValues(GameObject startName, int numB){
		GameObject itemName = startName;
		for (int i = 0; i < contents.Count; i++) {
			string num = contents [i].transform.GetChild(0).transform.GetChild(0).GetComponentInChildren<Text> ().text.ToString(); //get the number key set in the recipe gui
			int numI = int.Parse (num); //set the value to an int to find that key value in the keycodes dict

			if (numI == numB && keyCodes.Count >= numI && keyCodes.ContainsKey (numI)) {

				itemName = Resources.Load (keyCodes [numI]) as GameObject;

				if (recipeItems.ContainsKey (keyCodes [numI])) {
					ShowItemRequirements (itemName);
					requirements.transform.position = requirementsDefaultLoc;
				}
			}
		}
		return itemName;
	}

	//get the recipes from the dictionary and add the gui text object
	public void DisplayRecipeNames(){
		//ResetDisplaySprites ();
		keyCodes = new Dictionary<int, string> ();
		//List<string> recNames = FisherYatesShuffle (recipeItems.Keys.ToList());
		int count = 0;
		float numSlotSize = 0;
		float moveDown = 0f;
		currentTeirLevel = 2;
		foreach (KeyValuePair<string, GameItems> item in recipeItems) {
			if (item.Value.teir <= currentTeirLevel) {
				//create and put in the itemSlot that will exist in the shop window
				int slotNum = count + 1;
				GameObject numSlotObj = Instantiate (numSlot) as GameObject;
				numSlotObj.name = "Num" + slotNum.ToString();
				numSlotObj.transform.GetChild(0).GetComponentInChildren<Text>().text = slotNum.ToString();
				numSlotObj.transform.parent = itemsDisplay.transform;
				numSlotObj.transform.localScale = new Vector3 (1, 1, 1);
				numSlotSize = numSlotObj.GetComponent<RectTransform> ().rect.height;
				
				if(count % 2 == 0)
					numSlotObj.transform.localPosition = new Vector3 (-164, itemsDisplay.GetComponent<RectTransform>().rect.height/2 - (numSlotObj.GetComponent<RectTransform>().rect.height/2 + 20) - moveDown, 0);
				else
					numSlotObj.transform.localPosition = new Vector3 (128, itemsDisplay.GetComponent<RectTransform>().rect.height/2 - (numSlotObj.GetComponent<RectTransform>().rect.height/2 + 20) - moveDown, 0);
				numSlotObj.transform.GetChild(0).GetComponent<ShopButtons> ().keyCodeNum = slotNum;

				contents.Add (numSlotObj);

				GameObject recipeItemDisplay = Resources.Load (item.Key) as GameObject;

				if (recipeItemDisplay.GetComponentInChildren<SpriteRenderer> () != null && !recipeItemDisplay.tag.Contains("Hammer"))
					contents [count].transform.GetChild(0).GetComponent<Image> ().sprite = recipeItemDisplay.GetComponentInChildren<SpriteRenderer> ().sprite;
				else if (recipeItemDisplay.GetComponent<SpriteRenderer> () != null)
					contents [count].transform.GetChild(0).GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<SpriteRenderer> ().sprite;
				else
					contents [count].transform.GetChild(0).GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<Sprite3DImages> ().texture3DImages;

				//visually show item cannot be purchased
				if (item.Value.cost > checkCurrency.currency)
					contents [count].GetComponent<Image> ().color = new Color(255, 0, 0, 0.5f);
				else
					contents [count].GetComponent<Image> ().color = new Color(0, 0, 0, 0.5f);

				keyCodes.Add (slotNum, item.Key);
				count++;

				if (count % 2 == 0 && count > 1) moveDown += numSlotObj.GetComponent<RectTransform> ().rect.height;
			}
		}

		//to rescale and make sure that all the items fit in the scroll window
		if (currentTeirLevel != 0) {
			itemsDisplay.GetComponent<RectTransform> ().sizeDelta = new Vector2 (itemsDisplay.GetComponent<RectTransform> ().rect.width, itemsDisplay.GetComponent<RectTransform> ().rect.height + (numSlotSize * (count / 2)));
			itemsDisplay.transform.position = new Vector3 (itemsDisplay.transform.position.x, -itemsDisplay.transform.position.y, 0);

			float adjustVal = 19f; //for tier 2
			if (currentTeirLevel == 1)
				adjustVal = 25f;
			for (int i = 0; i < contents.Count; i++) {
				//visually show item cannot be purchased

				contents [i].transform.localPosition = new Vector3 (contents [i].transform.localPosition.x, contents [i].transform.localPosition.y + (itemsDisplay.GetComponent<RectTransform> ().rect.height / 2) - (adjustVal * count), 0);
			}
		}
	}

	//get the recipes from the dictionary and add the gui text object
	public void UpdateItemAvailablity(){
		//ResetDisplaySprites ()
		for (int i = 0; i < contents.Count; i++) {
			//visually show item cannot be purchased
			if (recipeItems [keyCodes [i + 1]].cost > checkCurrency.currency)
				contents[i].GetComponent<Image> ().color = new Color(255, 0, 0, 0.5f);
			else
				contents[i].GetComponent<Image> ().color = new Color(0, 0, 0, 0.5f);
			
			if (alreadyBoughtHealth) {
				string num = contents [i].transform.GetChild(0).transform.GetChild(0).GetComponentInChildren<Text> ().text.ToString(); //get the number key set in the recipe gui
				int numI = int.Parse (num); //set the value to an int to find that key value in the keycodes dict
				Debug.Log (contents [numI-1]);
				if (keyCodes [numI] == "Health_Increase") {
					Destroy(contents[numI-1]);
					contents.RemoveAt (numI - 1);
				}
			}
		}
	}

	//shuffle up the items so that they can be random each time regardless of tier
	private List<string> FisherYatesShuffle(List<string> recNames){
		for(int i = 0; i < recNames.Count; i++){
			int displayRecipeItem = Random.Range (0, recNames.Count - 1);

			string tempString = recNames[i];
			recNames[i] = recNames[displayRecipeItem];
			recNames[displayRecipeItem] = tempString;
		}

		return recNames;
	}

	public bool CanBuy(GameObject itemToCraft){
		return itemToCraft != null && recipeItems.ContainsKey (itemToCraft.name) && checkCurrency != null && checkCurrency.currency >= recipeItems [itemToCraft.name].cost;
	}

	//check if the player has enough chickens to buy the items if yes then add the new item
	public void CraftItem(GameObject itemToCraft){
		if (itemToCraft != null && recipeItems.ContainsKey(itemToCraft.name) && checkCurrency != null && checkCurrency.currency >= recipeItems[itemToCraft.name].cost) {
			checkCurrency.currency -= recipeItems [itemToCraft.tag].cost;

			//need to get item prefab based on name then create that an instance of that then add to inventory
			GameObject item = Instantiate(itemToCraft) as GameObject;

			if (item != null && item.tag != "Health_Increase") {
				GameObject playerItems = GameObject.Find ("PlayerItems");
				item.transform.parent = playerItems.transform;
				inventory.GetComponent<InventoryController> ().AddNewObject (item);
				isCraftable = true;
				playerItems.GetComponent<AudioSource> ().Play ();
				UpdateItemAvailablity ();
			} else if (item != null && item.tag == "Health_Increase" && item.GetComponent<MaxHealth> () != null) {
				item.GetComponent<MaxHealth> ().PurchaseHealth ();
				Destroy (item);
				alreadyBoughtHealth = true;
			}
		} else {
			isCraftable = false;
		}
	}

	//get the list of requirments or consumables needed then display them
	public void ShowItemRequirements(GameObject itemToCraft){
		//requirements.GetComponentInChildren<Text> ().text = "Item Requirements:\n";

		string info = (recipeItems[itemToCraft.name].cost + " Chickens");

		requirements.GetComponentInChildren<Text> ().text = itemToCraft.tag + " | " + info;
		description	.GetComponentInChildren<Text> ().text = recipeItems[itemToCraft.name].description;

		requirements.transform.GetChild(1).GetComponent<CanvasGroup> ().alpha = 1;
		description.transform.GetComponent<CanvasGroup> ().alpha = 1;
	}
}

//continue to flesh out the key code pressing
