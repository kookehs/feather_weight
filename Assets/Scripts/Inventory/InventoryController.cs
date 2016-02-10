using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

	//text object that displays the items in the inventory
	public Text contents;

	private SelectionHandler<GameObject> selectionHandler; //cycles through the list of items
	private SortedDictionary<string, List<GameObject>> inventoryItems; //contains all the gameobjects collected

	// Use this for initialization
	void Start () {
		inventoryItems = new SortedDictionary<string, List<GameObject>> ();
		PrintOutObjectNames ();
		selectionHandler = new SelectionHandler<GameObject> (inventoryItems);
	}

	void FixedUpdate(){
		//use the Up and Down arrow keys to cycle through the inventory list
		if (Input.GetKeyDown ("up") && selectionHandler.GetListSize() != 0) {
			selectionHandler.Previous ();
			PrintOutObjectNames ();
		}
		if (Input.GetKeyDown ("down") && selectionHandler.GetListSize() != 0) {
			selectionHandler.Next ();
			PrintOutObjectNames ();
		}
	}

	//display to the screen all the items in the inventory
	public void PrintOutObjectNames(){
		contents.GetComponent<Text> ().text = "";

		foreach (KeyValuePair<string, List<GameObject>> obj in inventoryItems) {
			string totalCount = (obj.Value.Count > 1 ? obj.Value.Count.ToString() : ""); //so that if the item has more then one occurance then display total count
            
			//check if the current key is what is select to display to the user that what item is selected
			if (obj.Key == selectionHandler.GetSelectedIndex ())
				contents.GetComponent<Text> ().text += ("+" + obj.Key + " " + totalCount + "\n");
			else {
				contents.GetComponent<Text> ().text += (obj.Key + " " + totalCount + "\n");
			}
		}
	}

	//add collected objects to the inventory and disable/remove those items from the world
	public void AddNewObject(GameObject obj){
		if(obj.GetComponentInChildren<SpriteRenderer> () != null) obj.GetComponentInChildren<SpriteRenderer> ().color = obj.GetComponent<Collection>().defaultCol; //remove object highlight

		//Remove Clone from Objects Name
		if(obj.name.Contains("(Clone)")){
			int index = obj.name.IndexOf ("(Clone)");
			obj.name = obj.name.Substring (0, index);
		}

		//see if object item already exist if so then add to GameObjects list if not create new key
		if (!inventoryItems.ContainsKey (obj.name))
			inventoryItems.Add (obj.name, new List<GameObject> (){obj});
		else {
			inventoryItems [obj.name].Add(obj);
		}

		//delete gameobject from world
		obj.SetActive(false);

		selectionHandler = new SelectionHandler<GameObject> (inventoryItems); //to rebuild the selection handler with the correct items
		PrintOutObjectNames ();
	}

	//remove an object from the inventory based on which on the user has selected
	public void RemoveObject(){
		string key = selectionHandler.GetSelectedIndex ();

		//make sure key does exist
		if(inventoryItems.ContainsKey(key)){
			//check how many of those items the player has if they have more then one item then just remove from gameObject list
			//if there is only one item then remove entire object key
			if (inventoryItems [key].Count > 1) {
				DropItem (key);
				inventoryItems [key].RemoveAt (inventoryItems [key].Count - 1);
			} else if (inventoryItems [key].Count == 1) {
				DropItem (key);
				inventoryItems.Remove (key);
			}

			selectionHandler = new SelectionHandler<GameObject> (inventoryItems);
			PrintOutObjectNames ();
		}
	}

	//remove all the items that are used to craft an item
	public void RemoveInventoryItems(Dictionary<string, int> consumableItems){
		foreach (KeyValuePair<string, int> itemNeeded in consumableItems) {
			//check if the value is one in the inventory
			if (inventoryItems.ContainsKey(itemNeeded.Key)){
				for (int i = 0; i < consumableItems [itemNeeded.Key]; i++) {
					//destory the object from the game world
					Destroy (inventoryItems [itemNeeded.Key][inventoryItems[itemNeeded.Key].Count - 1]);

					//check how many of those items the player has if they have more then one item then just remove from gameObject list
					//if there is only one item then remove entire object key
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

	//to drop a removed item a close distance from the player
	private void DropItem(string key){
		GameObject player = GameObject.Find ("Player");
		Vector3 playerPos = player.transform.position;
		float playerWidth = player.GetComponentInChildren<SpriteRenderer> ().bounds.size.x; //get the width of the player so thrown object won't be inside the player
		int index = inventoryItems [key].Count - 1; //the last item of the key's type will be dropped

		inventoryItems [key] [index].SetActive(true);
		inventoryItems [key] [index].transform.position = new Vector3(playerPos.x + playerWidth, playerPos.y, playerPos.z);
	}

	//get the inventory
	public SortedDictionary<string, List<GameObject>> GetInventoryItems()
	{
		return inventoryItems;
	}
}
