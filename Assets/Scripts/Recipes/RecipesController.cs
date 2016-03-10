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
	void Update(){
		//make sure we are in the inventory first before doing anything
		if (gameObject.GetComponent<RecipesDisplay>().openClose && gameObject.GetComponent<RecipesDisplay>().focus) {
			//first use a hotkey to select a category to work with
			if (category == "") {
				category = GetHotKeyCategories (category);
			}

			//confirm your selction to craft the item
			if (Input.GetKeyDown (KeyCode.Return)) {
				CraftItem (currentlySelected);
			}

			//undo selection of category
			if (Input.GetKeyDown (KeyCode.Escape)) {
				category = "";
				currentlySelected = null;
				DisplayCategory ();
			}

			//now determine and select the item through a new tear of hotkeys
			if (category != "") {
				DisplayRecipeNames (category);
				currentlySelected = GetHotKeyValues(currentlySelected);
			}
		} else {
			category = "";
			currentlySelected = null;
			DisplayCategory ();
		}
	}

	//determines if a key was pressed and determine the assosiated value for that button press based on category and item keycode
	private GameObject GetHotKeyValues(GameObject startName){
		GameObject itemName = startName;
		for (int i = 0; i < contents.Length; i++) {
			string num = contents [i].transform.GetChild(0).GetComponentInChildren<Text> ().text.ToString(); //get the number key set in the inventory gui
			int numI = int.Parse (num); //set the value to an int to find that key value in the keycodes dict

			if (Input.GetKeyUp (num) && keyCodes.Count >= numI && keyCodes.ContainsKey (numI)) {

				itemName = Resources.Load (keyCodes [numI]) as GameObject;

				if (recipeItems.ContainsKey (keyCodes [numI])) {
					ShowItemRequirements (itemName);
				}

			}
		}
		return itemName;
	}

	//determines if a key was pressed and determine the assosiated value for that button press based on category and item keycode
	private string GetHotKeyCategories(string category){
		string itemName = category;
		foreach (KeyValuePair<int, string> hotkey in keyCodes) {
			foreach(KeyValuePair<string, string> type in categories){
				if (hotkey.Value.Equals(type.Value) && Input.GetKeyUp (hotkey.Key.ToString())) {
					itemName = type.Value;
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
	}

	//get the recipes from the dictionary and add the gui text object
	//only display those that are in the category
	public void DisplayRecipeNames(string category){
		ResetDisplaySprites ();
		int size = categories.Count;
		string[] temp = new string[size];
		keyCodes = new Dictionary<int, string> ();
		int count = 0;

		foreach (KeyValuePair<string, List<string>> obj in recipeItems) {
			if (categories [obj.Key] == category && !temp.Contains(obj.Key)) {
				GameObject recipeItemDisplay = Resources.Load(obj.Key) as GameObject;
				if (recipeItemDisplay.GetComponentInChildren<SpriteRenderer> () != null)
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponentInChildren<SpriteRenderer> ().sprite;
				else
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<Sprite3DImages> ().texture3DImages;
				temp [count] = obj.Key;
				keyCodes.Add (count + 1, obj.Key);
				count++;
			}

			if (count + 1 >= contents.Length)
				break;
		}
	}

	public void DisplayCategory(){
		ResetDisplaySprites ();
		requirements.transform.parent.GetComponent<CanvasGroup> ().alpha = 0;
		keyCodes = new Dictionary<int, string> ();

		int size = categories.Count;
		string[] temp = new string[size];
		int count = 0;

		Dictionary<string,bool> specials = new Dictionary<string,bool> ();
		foreach (KeyValuePair<string, string> obj in categories) {
			if (!temp.Contains (obj.Value)) {
				GameObject recipeItemDisplay = Resources.Load(obj.Key) as GameObject;
				if (recipeItemDisplay.GetComponentInChildren<SpriteRenderer> () != null)
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponentInChildren<SpriteRenderer> ().sprite;
				else
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<Sprite3DImages> ().texture3DImages;
				temp [count] = obj.Value;
				keyCodes.Add (count + 1, obj.Value);
				if(count < contents.Length && count < temp.Length) count++;

				if (obj.Value.Equals ("Special")) specials.Add (obj.Key, false);
			}
		}
		inventory.GetComponent<InventoryController> ().specialEquipped = specials;
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

		if (checkInventory.isCraftable (consumableItems, inventoryItems, category)) {
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
			info += (obj.Key + ": " + obj.Value + "\n");
		}
		requirements.text = itemToCraft.tag + ":" + "\n" + info;
		requirements.transform.parent.GetComponent<CanvasGroup> ().alpha = 1;
	}
}

//continue to flesh out the key code pressing