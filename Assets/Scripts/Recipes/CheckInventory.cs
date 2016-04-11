using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CheckInventory {

	//make sure that the user has the right amount of items for the desired craft item
	public bool isCraftable(Dictionary<string, int> consumableItems, List<GameObject> inventoryItems, string category)
	{
		if (consumableItems.Count == 0 || inventoryItems.Count == 0)
			return false;

		// if the item is a Special item and the player is not allowed to craft the special item, return false
		if (category.Equals("Special") && !GameObject.Find("Monument").GetComponent<QuestController>().special_craftable)
			return false;

		bool craft = false;
		int craftCount = 0; //determine how many times the correct amount of objects were found in the inventory for the recipe

		foreach (GameObject itemHave in inventoryItems) {
			foreach (KeyValuePair<string, int> itemNeeded in consumableItems) {
				//if the key and the total number in the inventory is greater or equal to recipe value then user satifies 1 requirement
				if (itemHave.name == itemNeeded.Key)
					craftCount++;
			}
		}

		//if the total number of distinct items needed for recipe have been met 
		if (craftCount == consumableItems.Count)
			craft = true;

		return craft;
	}
}
