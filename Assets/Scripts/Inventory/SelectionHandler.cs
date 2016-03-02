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
	private Dictionary<string, List<T>> optionLabels = new Dictionary<string, List<T>>();

	public SelectionHandler(Dictionary<string, List<T>>labels)
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

	//move to the next item in the list this cycles downward
	public void Next()
	{
		DetermineNextPrevKey ("Next");
		optionObject = optionLabels [selectedKey];
	}

	//move to the previous item in the list cycles upward
	public void Previous(){
		selectedKey = previousKey;
		DetermineNextPrevKey ("Previous");
		optionObject = optionLabels [selectedKey];
	}

	//check to make sure that cycling is happening correctly
	private void DetermineNextPrevKey(string keyToFind){
		bool flagHit = false;
		bool firstObj = true;
		string temp = "";
		int totalCount = 0;

		foreach (KeyValuePair<string, List<T>> obj in optionLabels) {
			//so that the loop will stop once the next item in the list has been reachd
			if (flagHit) {
				selectedKey = obj.Key;
				break;
			}

			//once the currently selected item has been selected set flag true so break happens next loop
			if (obj.Key == selectedKey) {
				flagHit = true;

				//does not loop back for previous check
				if (keyToFind == "Previous")
					break;
			}

			//if we are at the top of the list set value to that
			if (firstObj) {
				temp = obj.Key;
				firstObj = false;
			}

			//check if we have reached the end so that we do not have an empty selected item (null)
			if (totalCount >= optionLabels.Count - 1)
				flagHit = false;

			//keep track of previous item
			previousKey = obj.Key;
			totalCount++;
		}

		//to set the next item to be selected
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

	//a bit misleading by title returns the key in the dictionary that is selected
	public string GetSelectedIndex()
	{
		return selectedKey;
	}
}