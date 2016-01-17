using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

	public Text contents;
	private SelectionHandler selectionHandler;
	private SortedDictionary<string, List<GameObject>> inventoryItems = new SortedDictionary<string, List<GameObject>>();
	//private List<string[]> recipes = new List<string[]>();

	// Use this for initialization
	void Start () {
		InsertObjectNames ();
		selectionHandler = new SelectionHandler (inventoryItems);
	}

	void FixedUpdate(){
		if (Input.GetKeyDown ("up")) {
			selectionHandler.Previous ();
			InsertObjectNames ();
		}
		if (Input.GetKeyDown ("down")) {
			selectionHandler.Next ();
			InsertObjectNames ();
		}
	}

	public void InsertObjectNames(){
		contents.GetComponent<Text> ().text = "";

		foreach (KeyValuePair<string, List<GameObject>> obj in inventoryItems) {
			string totalCount = (obj.Value.Count > 1 ? obj.Value.Count.ToString() : "");
            
			if (obj.Key == selectionHandler.GetSelectedIndex ())
				contents.GetComponent<Text> ().text += ("+" + obj.Key + " " + totalCount + "\n");
			else {
				contents.GetComponent<Text> ().text += (obj.Key + " " + totalCount + "\n");
			}
		}
	}

    //call add in a collision funtion of player then just add that colided object
	//fix how add works
	public void AddNewObject(bool count){
		//temp stuff
		int size = inventoryItems.Count;
		if (!count)
			size = 0;
		string objName = "Moon" + size;

		if (!inventoryItems.ContainsKey (objName))
			inventoryItems.Add (objName, new List<GameObject> (){new GameObject (objName)}); //to be replaced with actual gameObjects
		else {
			inventoryItems [objName].Add(new GameObject (objName));
		}
			
		selectionHandler = new SelectionHandler (inventoryItems);
		InsertObjectNames ();
	}

	public void RemoveObject(){
		string key = selectionHandler.GetSelectedIndex ();
		if(inventoryItems.ContainsKey(key)){
			if (inventoryItems [key].Count > 1) {
				inventoryItems [key].RemoveAt (inventoryItems [key].Count - 1);
			} else if (inventoryItems [key].Count == 1) {
				inventoryItems.Remove (key);
			}
		}
		selectionHandler = new SelectionHandler (inventoryItems);
		InsertObjectNames ();
	}
}
