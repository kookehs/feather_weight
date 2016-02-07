using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

[Serializable]
public class ReadRecipeJSON {

	//to save the json file in data forms that c# can work with easier
	private string recipeList = "";
	private JsonData recipeData;
	private Dictionary<string[], int> consumableList;

	// Use this for initialization
	public ReadRecipeJSON () {
		string pathFile = Application.dataPath + "/Scripts/Recipes/Recipes.JSON";

		//if json file exist get the data from that
		if(File.Exists(pathFile)){
			recipeList = File.ReadAllText (pathFile);
			recipeData = JsonMapper.ToObject (recipeList);
		}

		CreateDictForConsumables ();
	}

	//return the json object associated with the name of the craft object
	public JsonData GetRecipe(string name, string type){
		for(int i = 0; i < recipeData[type].Count; i++){
			if (recipeData [type] [i] ["Name"].ToString () == name)
				return recipeData [type] [i];
		}
		return null;
	}

	//retrieve all the diffent objects from the recipe list and put them in a string array
	public string[] GetRecipeNames(string type){
		
		int size = recipeData [type].Count;
		string[] temp = new string[size];

		for (int i = 0; i < size; i++) {
			temp [i] = recipeData [type] [i] ["Name"].ToString();
		}

		return temp;
	}

	//puts all the objects and the items needed for consumption into a dictionary for easier access
	private void CreateDictForConsumables(){
		consumableList = new Dictionary<string[], int> (){};

		string type = "Recipes";
		int size = recipeData [type].Count;

		//get dictionary values (craft item name, names of items need), how many of that item are needed
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < recipeData [type] [i]["Consumes"].Count; j++) {
				consumableList.Add (new string[2]{recipeData [type] [i] ["Name"].ToString(), recipeData [type] [i]["Consumes"][j][0].ToString()}, (int)recipeData [type] [i]["Consumes"][j][1]);
			}
		}
	}

	//return a dictionary containing all the required items for a particular recipe item
	public Dictionary<string, int> GetRecipeItemsConsumables(string key){
		Dictionary<string, int> itemsNeeded = new Dictionary<string, int> ();

		foreach (KeyValuePair<string[], int> items in consumableList) {
			if (items.Key [0] == key)
				itemsNeeded.Add (items.Key [1], consumableList [items.Key]);
		}

		return itemsNeeded;
	}

}
