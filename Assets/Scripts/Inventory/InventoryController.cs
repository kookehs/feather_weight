using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

	//text object that displays the items in the inventory
	public Text contents;

	private SelectionHandler<GameObject> selectionHandler; //cycles through the list of items
	private GameObject weaponHolder;

	public SortedDictionary<string, List<GameObject>> inventoryItems; //contains all the gameobjects collected

	// Use this for initialization
	void Start () {
		inventoryItems = new SortedDictionary<string, List<GameObject>> ();
		weaponHolder = GameObject.Find ("WeaponHolder");
		AddNewObject (weaponHolder.GetComponent<WeaponController> ().myWeapon);
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

		foreach (KeyValuePair<string, List<GameObject>> objs in inventoryItems) {
			string totalCount = (objs.Value.Count > 1 ? objs.Value.Count.ToString() : ""); //so that if the item has more then one occurance then display total count

			//check if the current key is what is select to display to the user that what item is selected
			if (objs.Key == selectionHandler.GetSelectedIndex ())
				contents.GetComponent<Text> ().text += ("+" + objs.Key + " " + totalCount + "\n");
			else {
				contents.GetComponent<Text> ().text += (objs.Key + " " + totalCount + "\n");
			}
		}
	}

	//add collected objects to the inventory and disable/remove those items from the world
	public void AddNewObject(GameObject obj){
		if(obj.layer.Equals("Collectable")) obj.GetComponentInChildren<SpriteRenderer> ().color = obj.GetComponent<Collection>().defaultCol; //remove object highlight

		//Remove Clone from Objects Name
		if(obj.name.Contains("(Clone)")){
			int index = obj.name.IndexOf ("(Clone)");
			obj.name = obj.name.Substring (0, index);
		}

		//get the tag value capitolize first letter to use tag for name in inventory
		string capitotizeLetter = obj.tag [0].ToString ().ToUpper ();
		string inventoryName = obj.tag;
		inventoryName = inventoryName.Remove (0, 1);
		inventoryName = inventoryName.Insert (0, capitotizeLetter);

		//see if object item already exist if so then add to GameObjects list if not create new key
		if (!inventoryItems.ContainsKey (inventoryName)) {
			inventoryItems.Add (inventoryName, new List<GameObject> (){ obj });
		}
		else {
			inventoryItems [inventoryName].Add(obj);
		}

		//delete gameobject from world
		foreach (Collider comp in obj.GetComponentsInChildren<Collider>()) {
			comp.enabled = false;
		}
		if (obj.GetComponent<Collection> () != null)
			obj.GetComponent<Collection> ().enabled = false;
		if (obj.GetComponent<Rigidbody> () != null)
			obj.GetComponent<Rigidbody> ().isKinematic = true;
		if (obj.GetComponentInChildren<SpriteRenderer> () != null)
			obj.GetComponentInChildren<SpriteRenderer> ().enabled = false;
		else
			obj.GetComponent<MeshRenderer> ().enabled = false;

		selectionHandler = new SelectionHandler<GameObject> (inventoryItems); //to rebuild the selection handler with the correct items
		PrintOutObjectNames ();

		StartCoroutine ("TurnOffHover");
	}

	IEnumerator TurnOffHover(){
		yield return new WaitForSeconds(0.2f);
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = false;
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

	public void RemoveSetBridgeObject(Transform riverPoint){
		string key = selectionHandler.GetSelectedIndex ();

		//make sure key does exist then place bridge in the correct place
		if (inventoryItems.ContainsKey (key)) {
			GameObject bridge = inventoryItems [key][inventoryItems[key].Count-1];
			RemoveObject ();
			bridge.transform.position = riverPoint.position;
			bridge.transform.rotation = riverPoint.rotation;
			bridge.transform.localScale = riverPoint.localScale;
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

		GameObject obj = inventoryItems [key] [index];
		//delete gameobject from world
		foreach (Collider comp in obj.GetComponentsInChildren<Collider>()) {
			comp.enabled = true;
		}
		if (obj.GetComponent<Collection> () != null)
			obj.GetComponent<Collection> ().enabled = true;
		if (obj.GetComponent<Rigidbody> () != null)
			obj.GetComponent<Rigidbody> ().isKinematic = false;
		if (obj.GetComponentInChildren<SpriteRenderer> () != null)
			obj.GetComponentInChildren<SpriteRenderer> ().enabled = true;
		else
			obj.GetComponent<MeshRenderer> ().enabled = true;

		obj.name += (index + 1);
		obj.transform.position = new Vector3(playerPos.x + playerWidth, playerPos.y, playerPos.z);
	}

	//allow player to use or equip the items in their inventory
	public void UseEquip(){
		GameObject item = inventoryItems [selectionHandler.GetSelectedIndex ()][inventoryItems[selectionHandler.GetSelectedIndex()].Count - 1];

		switch (item.gameObject.tag) {
			case "sword":
				if(!item.name.Equals("EquipedWeapon")) EquipWeapon (item);
				break;
			case "spear":
				if(!item.name.Equals("EquipedWeapon")) EquipWeapon (item);
					break;
			case "waterskin":
				item.GetComponent<WaterSkin> ().DrinkWater ();
				break;
			case "bridge":
				item.GetComponent<Bridge> ().SetBridge ();
				break;
		}
	}

	private void EquipWeapon(GameObject newWeapon){
		GameObject currentlyEquiped = GameObject.Find ("EquipedWeapon");
		currentlyEquiped.layer = LayerMask.NameToLayer ("Collectable");
		currentlyEquiped.GetComponent<Animator> ().enabled = false;
		ChangeInventoryItem (ref currentlyEquiped, "CraftedItems");
		foreach (Behaviour comp in currentlyEquiped.GetComponents<Behaviour>()) {
			comp.enabled = false;
		}

		newWeapon.transform.parent = weaponHolder.transform;
		newWeapon.layer = LayerMask.NameToLayer ("Default");
		newWeapon.GetComponent<Animator> ().enabled = true;
		weaponHolder.GetComponent<WeaponController> ().originalWeaponName = newWeapon.name;
		weaponHolder.GetComponent<WeaponController> ().myWeapon = newWeapon;
		ChangeInventoryItem (ref newWeapon, "weaponholder");
	}

	private void ChangeInventoryItem(ref GameObject value, string parent){
		foreach(KeyValuePair<string, List<GameObject>> obj in inventoryItems){
			for (int i = 0; i < obj.Value.Count; i++) {
				if (obj.Value [i].tag == value.tag) {
					if (parent.Equals ("weaponholder")) {
						value.transform.parent = weaponHolder.transform;
						value.name = "EquipedWeapon";
					} else {
						value.name = weaponHolder.GetComponent<WeaponController> ().originalWeaponName;
						value.transform.parent = GameObject.Find (parent).transform;
					}
					obj.Value [i] = value;
				}
			}
		}
	}

	//get the inventory
	public SortedDictionary<string, List<GameObject>> GetInventoryItems()
	{
		return inventoryItems;
	}
}

//things needed for inventory
//a way to eauipe a new weapon
//current issues
//sword is vary far away from player
//stop player from dropping bridge