using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecipesController : MonoBehaviour {

	public Text contents;
	public GameObject inventory;
	public bool isCraftable = true;

	private ReadRecipeJSON jsonData;
	public CheckInventory checkInventory;
	private SelectionHandler<string> selectionHandler;
	private SortedDictionary<string, List<string>> recipeItems;

	// Use this for initialization
	void Start () {
		recipeItems = new SortedDictionary<string, List<string>>();
		checkInventory = new CheckInventory ();
		jsonData = new ReadRecipeJSON ();
		InsertRecipeData ();
	}

	void FixedUpdate(){
		if (Input.GetKeyDown ("w")) {
			selectionHandler.Previous ();
			DisplayRecipeNames ();
		}
		if (Input.GetKeyDown ("s")) {
			selectionHandler.Next ();
			DisplayRecipeNames ();
		}
	}

	public void InsertRecipeData(){
		string[] recipeNames = jsonData.GetRecipeNames ("Recipes");
		for (int i = 0; i < recipeNames.Length; i++) {
			recipeItems.Add (recipeNames [i], new List<string> ());
		}

		selectionHandler = new SelectionHandler<string> (recipeItems);
		DisplayRecipeNames ();
	}

	public void DisplayRecipeNames(){
		contents.GetComponent<Text> ().text = "";

		foreach (KeyValuePair<string, List<string>> obj in recipeItems) {
			string totalCount = (obj.Value.Count > 1 ? obj.Value.Count.ToString() : "");

			if (obj.Key == selectionHandler.GetSelectedIndex ())
				contents.GetComponent<Text> ().text += ("+" + obj.Key + " " + totalCount + "\n");
			else {
				contents.GetComponent<Text> ().text += (obj.Key + " " + totalCount + "\n");
			}
		}
	}

	public void CraftItem(){
		Dictionary<string, int> consumableItems = jsonData.GetRecipeItemsConsumables(selectionHandler.GetSelectedIndex());
		SortedDictionary<string, List<GameObject>> inventoryItems = inventory.GetComponent<InventoryController>().GetInventoryItems();
		if (checkInventory.isCraftable (consumableItems, inventoryItems)) {
			inventory.GetComponent<InventoryController> ().RemoveInventoryItems (consumableItems);
			inventory.GetComponent<InventoryController> ().AddNewObject (selectionHandler.GetSelectedIndex ());
			isCraftable = true;
		} else {
			isCraftable = false;
		}
	}
}
