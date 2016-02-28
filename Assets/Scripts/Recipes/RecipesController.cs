using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RecipesController : MonoBehaviour {

	public Text contents; //text object that will diaply the recipes list
	public Text requirements;
	public GameObject inventory;
	public bool isCraftable = true; //to determine whether or not to display the diolog telling user the item cannot be crafted

	//data variables for recipes and user's inventory
	private ReadRecipeJSON jsonData;
	private CheckInventory checkInventory;
	private SortedDictionary<string, List<string>> recipeItems;
	private Dictionary<string, int> keyCodes;
	private Dictionary<string, string> categories;

	private string category = "";
	private string currentlySelected = "Spear_Stone";

	// Use this for initialization
	void Start () {
		recipeItems = new SortedDictionary<string, List<string>>();
		checkInventory = new CheckInventory ();
		jsonData = new ReadRecipeJSON ();
		InsertRecipeData ();
		DisplayCategory ();
	}

	//using categories we lock in onto certain items that then let the number craft and show the item
	//1-9 set as category buttons starts
	//once inside a category then nums work for item selection
	void FixedUpdate(){
		//use the W and S keys to move through the recipes list
		if (Input.GetKeyDown ("1") && category == "") {
			DisplayRecipeNames ("Weapon");
			category = "Weapon";
		} else if(category != ""){
			if(Input.GetKeyDown ("1")){
				ShowItemRequirements ("Spear_Stone");
				currentlySelected = "Spear_Stone";
			}
			if (Input.GetKeyDown ("2")){
				ShowItemRequirements ("Spear_Metal");
				currentlySelected = "Spear_Metal";
			}
		}
		if (Input.GetKeyDown ("2") && category == "") {
			DisplayRecipeNames ("Survival");
			category = "Survival";
		}
		if (Input.GetKeyDown ("3") && category == "") {
			DisplayRecipeNames ("Travel");
			category = "Travel";
		}
		if (Input.GetKeyDown (KeyCode.Return)) {
			Debug.Log (currentlySelected);
			CraftItem (currentlySelected);
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			category = "";
			DisplayCategory ();
		}

		if (Input.GetKeyDown ("4") && category == "Travel") {
			ShowItemRequirements ("Torch");
			currentlySelected = "Torch";
		}
	}

	//insert into the recipes dictionary the list of recipes from the json file
	public void InsertRecipeData(){
		string[] recipeNames = jsonData.GetRecipeNames ("Recipes");
		for (int i = 0; i < recipeNames.Length; i++) {
			recipeItems.Add (recipeNames [i], new List<string> ());
		}

		keyCodes = jsonData.GetRecipeItemsKeyCode ();
		categories = jsonData.GetRecipeItemsCategories();
	}

	//get the recipes from the dictionary and add the gui text object
	//only display those that are in the category
	public void DisplayRecipeNames(string category){
		contents.GetComponent<Text> ().text = "";

		foreach (KeyValuePair<string, List<string>> obj in recipeItems) {
			if(categories[obj.Key] == category)
				contents.GetComponent<Text> ().text += (obj.Key + " " + "(" + keyCodes[obj.Key] + ")\n");
		}

		ShowItemRequirements ("Spear_Stone");
	}

	public void DisplayCategory(){
		contents.GetComponent<Text> ().text = "";

		int size = categories.Count;
		string[] temp = new string[size];
		int count = 0;

		foreach (KeyValuePair<string, string> obj in categories) {
			if (!temp.Contains (obj.Value)) {
				contents.GetComponent<Text> ().text += (obj.Value + "(" + (count + 1) + ")\n");
				temp [count] = obj.Value;
				count++;
			}
		}
	}

	//check if the player has enough items to craft with and if so then remove the items from the inventory and world then add the new item
	public void CraftItem(string itemToCraft){
		Dictionary<string, int> consumableItems = jsonData.GetRecipeItemsConsumables(itemToCraft);
		SortedDictionary<string, List<GameObject>> inventoryItems = inventory.GetComponent<InventoryController>().GetInventoryItems();

		if (checkInventory.isCraftable (consumableItems, inventoryItems)) {
			inventory.GetComponent<InventoryController> ().RemoveInventoryItems (consumableItems);

			//need to get item prefab based on name then create that an instance of that then add to inventory
			GameObject item = Instantiate(Resources.Load(itemToCraft)) as GameObject;

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
	public void ShowItemRequirements(string itemToCraft){
		requirements.GetComponent<Text> ().text = "Item Requirements:\n";
		Dictionary<string, int> tempComsumables = jsonData.GetRecipeItemsConsumables(itemToCraft);

		foreach (KeyValuePair<string, int> obj in tempComsumables) {
			requirements.GetComponent<Text> ().text += (obj.Key + ": " + obj.Value + " " + "\n");
		}
	}
}

//continue to flesh out the key code pressing
//turn sorted dic into a none sorted one