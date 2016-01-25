using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

	public Text contents;
	private SelectionHandler<GameObject> selectionHandler;
	private SortedDictionary<string, List<GameObject>> inventoryItems;

	// Use this for initialization
	void Start () {
		inventoryItems = new SortedDictionary<string, List<GameObject>> ();
		PrintOutObjectNames ();
		selectionHandler = new SelectionHandler<GameObject> (inventoryItems);
	}

	void FixedUpdate(){
		if (Input.GetKeyDown ("up")) {
			selectionHandler.Previous ();
			PrintOutObjectNames ();
		}
		if (Input.GetKeyDown ("down")) {
			selectionHandler.Next ();
			PrintOutObjectNames ();
		}
	}

	public void PrintOutObjectNames(){
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
	//parameter will be a gameobject not a bool
	//fix how add works
	string[] tempObjectNames = new string[]{"wood", "chicken", "stone", "iron"};
	public void AddNewObject(string name){
		//temp stuff
		int size = inventoryItems.Count%4;
		if (name == "")
			name = tempObjectNames [size];
		
		GameObject obj = new GameObject (name);
		obj.AddComponent<SpriteRenderer> ();

		if (!inventoryItems.ContainsKey (obj.name))
			inventoryItems.Add (obj.name, new List<GameObject> (){obj}); //to be replaced with actual gameObjects
		else {
			inventoryItems [obj.name].Add(obj);
		}

		//delete gameobject from world
		obj.SetActive(false);

		selectionHandler = new SelectionHandler<GameObject> (inventoryItems);
		PrintOutObjectNames ();
	}

	//make the drop happen near player position
	public void RemoveObject(){
		string key = selectionHandler.GetSelectedIndex ();
		if(inventoryItems.ContainsKey(key)){
			if (inventoryItems [key].Count > 1) {
				DropItem (key);
				inventoryItems [key].RemoveAt (inventoryItems [key].Count - 1);
			} else if (inventoryItems [key].Count == 1) {
				DropItem (key);
				inventoryItems.Remove (key);
			}
		}
		selectionHandler = new SelectionHandler<GameObject> (inventoryItems);
		PrintOutObjectNames ();
	}

	public void RemoveInventoryItems(Dictionary<string, int> consumableItems){
		foreach (KeyValuePair<string, int> itemNeeded in consumableItems) {
			if (inventoryItems.ContainsKey(itemNeeded.Key)){
				for (int i = 0; i < consumableItems [itemNeeded.Key]; i++) {
					Destroy (inventoryItems [itemNeeded.Key][inventoryItems[itemNeeded.Key].Count - 1]);
					if (inventoryItems [itemNeeded.Key].Count > 1) {
						inventoryItems [itemNeeded.Key].RemoveAt (inventoryItems [itemNeeded.Key].Count - 1);
					} else if (inventoryItems [itemNeeded.Key].Count == 1) {
						inventoryItems.Remove (itemNeeded.Key);
					}
				}
			}
		}

		selectionHandler = new SelectionHandler<GameObject> (inventoryItems);
		PrintOutObjectNames ();
	}

	private void DropItem(string key){
		GameObject player = GameObject.Find ("Player");
		Vector3 playerPos = player.transform.position;
		float playerWidth = player.GetComponent<SpriteRenderer> ().bounds.size.x;
		//float objWidth = inventoryItems [key] [0].GetComponent<SpriteRenderer> ().bounds.size.x;
		int index = inventoryItems [key].Count - 1;
		inventoryItems [key] [index].SetActive(true);
		inventoryItems [key] [index].transform.position = new Vector3(playerPos.x + playerWidth, playerPos.y, playerPos.z);
	}

	public SortedDictionary<string, List<GameObject>> GetInventoryItems()
	{
		return inventoryItems;
	}
}
