using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RecipesController : MonoBehaviour {

	public GameObject[] contents; //text object that will diaply the recipes list
	public Text requirements;
	public Sprite defaultSprite;
	public GameObject inventory;
	public bool isCraftable = true; //to determine whether or not to display the diolog telling user the item cannot be crafted
	public bool craftingMode = false;

	//data variables for recipes and user's inventory
	private ReadRecipeJSON jsonData;
	private CheckInventory checkInventory;
	private Dictionary<string, List<string>> recipeItems;
	private Dictionary<int, string> keyCodes;
	private Dictionary<string, string> categories;

	private string category = "";
	private GameObject currentlySelected;

	// Use this for initialization
	public void Start () {
		recipeItems = new Dictionary<string, List<string>>();
		checkInventory = new CheckInventory ();
		jsonData = new ReadRecipeJSON ();
		InsertRecipeData ();
		DisplayCategory ();
	}

	//using categories we lock in onto certain items that then let the number craft and show the item
	//1-9 set as category buttons starts
	//once inside a category then nums work for item selection
	void FixedUpdate(){
		if (Input.GetKeyUp ("c")) {
			craftingMode = !craftingMode;
			inventory.GetComponent<InventoryController>().inventoryMode = false;
		}

		//make sure we are in the inventory first before doing anything
		if (!inventory.GetComponent<InventoryDisplay> ().inventoryOpen && craftingMode) {
			//first use a hotkey to select a category to work with
			if (category == "") {
				category = GetHotKeyCategories (category);
				DisplayRecipeNames (category);
			}

			//confirm your selction to craft the item
			if (Input.GetKeyDown (KeyCode.Return)) {
				CraftItem (currentlySelected);
			}
			Debug.Log (category);
			//undo selection of category
			if (Input.GetKeyDown (KeyCode.Escape)) {
				category = "";
				currentlySelected = null;
				DisplayCategory ();
			}

			//now determine and select the item through a new tear of hotkeys
			if (category != "") {
				currentlySelected = Resources.Load(GetHotKeyValues (currentlySelected.tag)) as GameObject;
			}
		} else {
			category = "";
			currentlySelected = null;
			craftingMode = false;
			DisplayCategory ();
		}
	}

	//determines if a key was pressed and determine the assosiated value for that button press based on category and item keycode
	private string GetHotKeyValues(string startName){
		string itemName = startName;
		for (int i = 0; i < contents.Length; i++) {
			string num = contents [i].transform.GetChild(0).GetComponentInChildren<Text> ().text.ToString(); //get the number key set in the inventory gui
			int numI = int.Parse (num); //set the value to an int to find that key value in the keycodes dict

			if (Input.GetKeyUp (num) && keyCodes.Count >= numI && keyCodes.ContainsKey(numI)) {
				
				if (category != "" && recipeItems.ContainsKey(keyCodes [numI])) {
					ShowItemRequirements (currentlySelected);
				}
				
				itemName = keyCodes[numI];
			}
		}
		return itemName;
	}

	//determines if a key was pressed and determine the assosiated value for that button press based on category and item keycode
	private string GetHotKeyCategories(string category){
		string itemName = category;
		foreach (KeyValuePair<int, string> hotkey in keyCodes) {
			foreach(KeyValuePair<string, string> type in categories){
				if (hotkey.Value.Equals(type.Key) && Input.GetKey (hotkey.Key.ToString())) {
					itemName = hotkey.Value;
				}
			}
		}
		return itemName;
	}


	//insert into the recipes dictionary the list of recipes from the json file
	public void InsertRecipeData(){
		string[] recipeNames = jsonData.GetRecipeNames ("Recipes");
		for (int i = 0; i < recipeNames.Length; i++) {
			recipeItems.Add (recipeNames [i], new List<string> ());
		}

		categories = jsonData.GetRecipeItemsCategories();
		keyCodes = jsonData.GetRecipeItemsKeyCode ();
	}

	//get the recipes from the dictionary and add the gui text object
	//only display those that are in the category
	public void DisplayRecipeNames(string category){
		int size = categories.Count;
		string[] temp = new string[size];
		keyCodes = new Dictionary<int, string> ();
		int count = 0;

		foreach (KeyValuePair<string, List<string>> obj in recipeItems) {
			if (categories [obj.Key] == category) {
				GameObject recipeItemDisplay = Resources.Load(obj.Key) as GameObject;
				if (recipeItemDisplay.GetComponentInChildren<SpriteRenderer> () != null)
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponentInChildren<SpriteRenderer> ().sprite;
				else
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<Sprite3DImages> ().texture3DImages;
				keyCodes.Add (count + 1, recipeItemDisplay.tag);
			}

			if (count + 1 >= contents.Length)
				break;
		}

		//ShowItemRequirements (currentlySelected);
	}

	public void DisplayCategory(){
		int size = categories.Count;
		string[] temp = new string[size];
		int count = 0;

		foreach (KeyValuePair<string, string> obj in categories) {
			if (!temp.Contains (obj.Value)) {
				GameObject recipeItemDisplay = Resources.Load(obj.Key) as GameObject;
				if (recipeItemDisplay.GetComponentInChildren<SpriteRenderer> () != null)
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponentInChildren<SpriteRenderer> ().sprite;
				else
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<Sprite3DImages> ().texture3DImages;
				temp [count] = obj.Value;
				if(count < contents.Length && count < temp.Length) count++;
			}
		}

		keyCodes = jsonData.GetRecipeItemsKeyCode ();
	}

	private void ResetDisplaySprites(){
		for (int i = 0; i < contents.Length; i++) {
			contents [i].GetComponent<Image> ().sprite = defaultSprite;
		}
	}

	//check if the player has enough items to craft with and if so then remove the items from the inventory and world then add the new item
	public void CraftItem(GameObject itemToCraft){
		Dictionary<string, int> consumableItems = jsonData.GetRecipeItemsConsumables(itemToCraft.tag);
		Dictionary<string, List<GameObject>> inventoryItems = inventory.GetComponent<InventoryController>().GetInventoryItems();

		if (checkInventory.isCraftable (consumableItems, inventoryItems)) {
			inventory.GetComponent<InventoryController> ().RemoveInventoryItems (consumableItems);

			//need to get item prefab based on name then create that an instance of that then add to inventory
			GameObject item = Instantiate(itemToCraft) as GameObject;

			if (item != null) {
				item.transform.parent = GameObject.Find ("CraftedItems").transform;
				inventory.GetComponent<InventoryController> ().AddNewObject (item);
				isCraftable = true;
			}
		} else {
			isCraftable = false;
		}
	}

	//get the list of requirments or consumables needed then display them
	public void ShowItemRequirements(GameObject itemToCraft){
		requirements.GetComponent<Text> ().text = "Item Requirements:\n";
		Dictionary<string, int> tempComsumables = jsonData.GetRecipeItemsConsumables(itemToCraft.tag);

		string info = "";
		foreach (KeyValuePair<string, int> obj in tempComsumables) {
			
			//for (int j = 0; j < recipeItems [obj.Value].Count; j++) {
			info += (obj.Key + ": " + obj.Value + "/n");
			//}


		}
		requirements.text = itemToCraft.tag + ":" + "\n" + info;
		requirements.transform.parent.GetComponent<CanvasGroup> ().alpha = 1;
	}
}

//continue to flesh out the key code pressing
//current thought how to get sprite images for recipe items when items don't exist in this context
//Resources.Load(itemToCraft)