using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SelectionHandler<T>
{
	private string selectedKey;
	private string previousKey;

	[SerializeField]
	private List<T> optionObject;

	[SerializeField]
	private SortedDictionary<string, List<T>> optionLabels = new SortedDictionary<string, List<T>>();

	public SelectionHandler(SortedDictionary<string, List<T>>labels)
	{
		selectedKey = "";
		optionLabels.Clear ();
		optionLabels = labels;
		if (optionLabels.Count > 0) {
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

		foreach (KeyValuePair<string, List<T>> obj in optionLabels) {
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

	public List<T> GetOptionListObject()
	{
		return optionObject;
	}

	public string GetSelectedIndex()
	{
		return selectedKey;
	}
}