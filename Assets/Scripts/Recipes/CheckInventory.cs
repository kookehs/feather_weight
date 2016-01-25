using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class CheckInventory {

	public bool isCraftable(Dictionary<string, int> consumableItems, SortedDictionary<string, List<GameObject>> inventoryItems)
	{
		if (consumableItems.Count == 0 || inventoryItems.Count == 0)
			return false;
		
		bool craft = false;
		int craftCount = 0;
		foreach (KeyValuePair<string, List<GameObject>> itemHave in inventoryItems) {
			foreach (KeyValuePair<string, int> itemNeeded in consumableItems) {
				if (itemHave.Key == itemNeeded.Key && inventoryItems [itemHave.Key].Count >= consumableItems [itemNeeded.Key])
					craftCount++;
			}
		}

		if (craftCount == consumableItems.Count)
			craft = true;

		return craft;
	}
}
