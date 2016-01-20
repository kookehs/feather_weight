using UnityEngine;
using System;
using System.Collections;
using System.IO;
using LitJson;

[Serializable]
public class ReadRecipeJSON {

	private string recipeList = "";
	private JsonData recipeData;

	// Use this for initialization
	public ReadRecipeJSON () {
		//rc = new RecipeClass ();
		string pathFile = Application.dataPath + "/Scripts/Recipes/Recipes.JSON";

		if(File.Exists(pathFile)){
			recipeList = File.ReadAllText (pathFile);
			recipeData = JsonMapper.ToObject (recipeList);
		}

	}
	
	public JsonData GetRecipe(string name, string type){
		for(int i = 0; i < recipeData[type].Count; i++){
			if (recipeData [type] [i] ["Name"].ToString () == name)
				return recipeData [type] [i];
		}
		return null;
	}

	public string[] GetRecipeNames(string type){
		
		int size = recipeData [type].Count;
		string[] temp = new string[size];

		for (int i = 0; i < size; i++) {
			temp [i] = recipeData [type] [i] ["Name"].ToString();
		}

		return temp;
	}
}
