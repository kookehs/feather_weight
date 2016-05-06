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
		if (category.Equals("Special") && !QuestController.special_craftable)
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

	//	Find all chickens in inventory
	//	Move them into the cage
	//	Remove them from the inventory
	//	Return the number of chickens
	public int dealWithChickens(GameObject cage, InventoryController inventory) {
		int result = 0;
		for (int i = 0; i < inventory.inventoryItems.Count; i ++){
			GameObject itemHave = inventory.inventoryItems[i];
			if (itemHave.tag == "Chicken") {
				inventory.currentlySelected = i;
				inventory.RemoveObject ();
				itemHave.transform.position = cage.transform.position;
				itemHave.transform.parent = cage.transform;
				result += 1;
			}
		}
		return result;
	}

	//	Find all chickens in the inventory
	//	Remove them from the inventory
	//	Note: This function is called at the end of each wave in WaveController.cs
	public void findAndRemoveChickens(InventoryController inventory) {
		for (int i = inventory.inventoryItems.Count - 1; i >= 0; i--) {
			GameObject itemHave = inventory.inventoryItems[i];
			if (itemHave.tag == "Chicken") {
				inventory.currentlySelected = i;
				inventory.RemoveObject ();
				WorldContainer.Remove (itemHave);
			}
		}
		return;
	}
}
