using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecipesController : MonoBehaviour {

	public Text contents; //text object that will diaply the recipes list
	public GameObject inventory;
	public bool isCraftable = true; //to determine whether or not to display the diolog telling user the item cannot be crafted

	//data variables for recipes and user's inventory
	private ReadRecipeJSON jsonData;
	private CheckInventory checkInventory;
	private SelectionHandler<string> selectionHandler; //used for cycling through the items on the list
	private SortedDictionary<string, List<string>> recipeItems;

	// Use this for initialization
	void Start () {
		recipeItems = new SortedDictionary<string, List<string>>();
		checkInventory = new CheckInventory ();
		jsonData = new ReadRecipeJSON ();
		InsertRecipeData ();
	}

	void FixedUpdate(){
		//use the W and S keys to move through the recipes list
		if (Input.GetKeyDown ("w")) {
			selectionHandler.Previous ();
			DisplayRecipeNames ();
		}
		if (Input.GetKeyDown ("s")) {
			selectionHandler.Next ();
			DisplayRecipeNames ();
		}
	}

	//insert into the recipes dictionary the list of recipes from the json file
	public void InsertRecipeData(){
		string[] recipeNames = jsonData.GetRecipeNames ("Recipes");
		for (int i = 0; i < recipeNames.Length; i++) {
			recipeItems.Add (recipeNames [i], new List<string> ());
		}

		selectionHandler = new SelectionHandler<string> (recipeItems);
		DisplayRecipeNames ();
	}

	//get the recipes from the dictionary and add the gui text object
	public void DisplayRecipeNames(){
		contents.GetComponent<Text> ().text = "";

		foreach (KeyValuePair<string, List<string>> obj in recipeItems) {
			//check if the current key is what is select to display to the user that what item is selected
			if (obj.Key == selectionHandler.GetSelectedIndex ())
				contents.GetComponent<Text> ().text += ("+" + obj.Key + " " + "\n");
			else {
				contents.GetComponent<Text> ().text += (obj.Key + " " + "\n");
			}
		}
	}

	//check if the player has enough items to craft with and if so then remove the items from the inventory and world then add the new item
	public void CraftItem(){
		Dictionary<string, int> consumableItems = jsonData.GetRecipeItemsConsumables(selectionHandler.GetSelectedIndex());
		SortedDictionary<string, List<GameObject>> inventoryItems = inventory.GetComponent<InventoryController>().GetInventoryItems();

		if (checkInventory.isCraftable (consumableItems, inventoryItems)) {
			inventory.GetComponent<InventoryController> ().RemoveInventoryItems (consumableItems);

			//need to get item prefab based on name then create that an instance of that then add to inventory
			GameObject item = Instantiate(Resources.Load(selectionHandler.GetSelectedIndex ())) as GameObject;

			if (item != null) {
				inventory.GetComponent<InventoryController> ().AddNewObject (item);
				isCraftable = true;
			}
		} else {
			isCraftable = false;
		}
	}
}
