using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;


public class ReadRecipeJSON {

	//to save the json file in data forms that c# can work with easier
	private string recipeList = "";
	private JsonData recipeData;
	private static Dictionary<string, GameItems> itemsList;
	//consider making a class RecipeItem so that I can store that class instead of having a bunch of sparate Dictionaries

	public static Dictionary<string, GameItems> items_List{
		get {return itemsList;}
		set {itemsList = value;}
	}

	// Use this for initialization
	public ReadRecipeJSON () {
		string pathFile = Application.dataPath + "/Scripts/Recipes/Recipes.JSON";

		//if json file exist get the data from that
		if(File.Exists(pathFile)){
			recipeList = File.ReadAllText (pathFile);
			recipeData = JsonMapper.ToObject (recipeList);
		}

		CreateDictForItemsList ();
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
	private void CreateDictForItemsList(){
		itemsList = new Dictionary<string, GameItems> (){};

		string type = "Recipes";
		int size = recipeData [type].Count;

		//get dictionary values (craft item name, names of items need), how many of that item are needed
		for (int i = 0; i < size; i++) {
			GameItems newItem = new GameItems (recipeData [type] [i] ["Name"].ToString(), (int)recipeData [type] [i] ["Teir"],
				recipeData [type] [i] ["Description"].ToString(), (int)recipeData [type] [i] ["Cost"]);
			
			itemsList.Add (recipeData [type] [i] ["Name"].ToString(), newItem);
		}
	}

	//return a dictionary containing all the required items for a particular recipe item
	public Dictionary<string, GameItems> GetRecipeItemsList(){
		return itemsList;
	}

	public int GetItemCost(GameObject item){
		//ReadRecipeJSON ();
		return itemsList [item.tag].cost;
	}

}
