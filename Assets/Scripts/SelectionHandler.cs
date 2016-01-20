using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionHandler
{
	private string selectedKey;
	private string previousKey;
	private List<GameObject> optionObject;

	private SortedDictionary<string, List<GameObject>> optionLabels = new SortedDictionary<string, List<GameObject>>();

	public SelectionHandler(SortedDictionary<string, List<GameObject>>labels)
	{
		selectedKey = "";
		optionLabels.Clear ();
		optionLabels = labels;
		if (labels.Count > 0) {
			DetermineNextPrevKey ("Next");
			optionObject = optionLabels [selectedKey];
		}
	}

	public void Reset()
	{
		selectedKey = "";
	}

	public void Next()
	{
		DetermineNextPrevKey ("Next");
		optionObject = optionLabels [selectedKey];
	}

	public void Previous(){
		selectedKey = previousKey;
		DetermineNextPrevKey ("Previous");
		optionObject = optionLabels [selectedKey];
	}

	private void DetermineNextPrevKey(string keyToFind){
		bool flagHit = false;
		bool firstObj = true;
		string temp = "";
		int totalCount = 0;

		foreach (KeyValuePair<string, List<GameObject>> obj in optionLabels) {
			if (flagHit) {
				selectedKey = obj.Key;
				break;
			}
			
			if (obj.Key == selectedKey) {
				flagHit = true;
				if (keyToFind == "Previous")
					break;
			}

			if (firstObj) {
				temp = obj.Key;
				firstObj = false;
			}

			if (totalCount >= optionLabels.Count - 1)
				flagHit = false;
			previousKey = obj.Key;
			totalCount++;
		}

		if (!flagHit) {
			selectedKey = temp;
		}
	}

	public int GetListSize(){
		return optionLabels.Count;
	}

	public List<GameObject> GetOptionListObject()
	{
		return optionObject;
	}

	public string GetSelectedIndex()
	{
		return selectedKey;
	}
}