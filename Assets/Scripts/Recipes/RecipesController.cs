using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RecipesController : MonoBehaviour {

	public Text contents;
	private ReadRecipeJSON jsonData;
	private SelectionHandler<string> selectionHandler;
	private SortedDictionary<string, List<string>> inventoryItems = new SortedDictionary<string, List<string>>();

	// Use this for initialization
	void Start () {
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
			inventoryItems.Add (recipeNames [i], new List<string> ());
		}

		selectionHandler = new SelectionHandler<string> (inventoryItems);
		DisplayRecipeNames ();
	}

	public void DisplayRecipeNames(){
		contents.GetComponent<Text> ().text = "";

		foreach (KeyValuePair<string, List<string>> obj in inventoryItems) {
			string totalCount = (obj.Value.Count > 1 ? obj.Value.Count.ToString() : "");

			if (obj.Key == selectionHandler.GetSelectedIndex ())
				contents.GetComponent<Text> ().text += ("+" + obj.Key + " " + totalCount + "\n");
			else {
				contents.GetComponent<Text> ().text += (obj.Key + " " + totalCount + "\n");
			}
		}
	}

	public void CraftItem(){
		print ("crafting");
	}
}
