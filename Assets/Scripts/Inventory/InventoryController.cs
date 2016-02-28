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

	private GameObject player;

	// Use this for initialization
	void Start () {
		inventoryItems = new SortedDictionary<string, List<GameObject>> ();
		player = GameObject.Find ("Player");
		weaponHolder = GameObject.Find ("WeaponHolder");
		weaponHolder.GetComponent<WeaponController> ().myWeapon.name = "EquipedWeapon";
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
		/*string capitotizeLetter = obj.tag [0].ToString ().ToUpper ();
		string inventoryName = obj.tag;
		inventoryName = inventoryName.Remove (0, 1);
		inventoryName = inventoryName.Insert (0, capitotizeLetter);*/

		//see if object item already exist if so then add to GameObjects list if not create new key
		if (!inventoryItems.ContainsKey (obj.tag)) {
			inventoryItems.Add (obj.tag, new List<GameObject> (){ obj });
		}
		else {
			inventoryItems [obj.tag].Add(obj);
		}

		//delete gameobject from world
		if(!obj.name.Equals("EquipedWeapon")){
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
			if (obj.transform.FindChild("Trail") != null)
				obj.transform.FindChild("Trail").gameObject.SetActive(false);
		}

		selectionHandler = new SelectionHandler<GameObject> (inventoryItems); //to rebuild the selection handler with the correct items
		PrintOutObjectNames ();

		StartCoroutine ("TurnOffHover");
	}

	IEnumerator TurnOffHover(){
		yield return new WaitForSeconds(0.2f);
		player.GetComponent<PlayerMovementRB>().mouseHovering = false;
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

	public void RemoveSetLadderObject(Transform cliffPoint){
		string key = selectionHandler.GetSelectedIndex ();

		//make sure key does exist then place ladder in the correct place
		if (inventoryItems.ContainsKey (key)) {
			GameObject ladder = inventoryItems [key][inventoryItems[key].Count-1];
			RemoveObject ();
			ladder.transform.position = cliffPoint.position;
			ladder.transform.rotation = cliffPoint.rotation;
			ladder.transform.localScale = cliffPoint.localScale;
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

	//make this work so I can reduce some code
	/*private void EnableDisableObjectComponent(GameObject obj){
		//delete gameobject from world
		foreach (Collider comp in obj.GetComponentsInChildren<Collider>()) {
			comp.enabled = !comp.enabled;
		}
		if (obj.GetComponent<Collection> () != null)
			obj.GetComponent<Collection> ().enabled = !obj.GetComponent<Collection> ().enabled;
		if (obj.GetComponent<Rigidbody> () != null)
			obj.GetComponent<Rigidbody> ().isKinematic = !obj.GetComponent<Rigidbody> ().isKinematic;
		if (obj.GetComponentInChildren<SpriteRenderer> () != null)
			obj.GetComponentInChildren<SpriteRenderer> ().enabled = !obj.GetComponentInChildren<SpriteRenderer> ().enabled;
		else
			obj.GetComponent<MeshRenderer> ().enabled = true;
	}*/

	//allow player to use or equip the items in their inventory
	public void UseEquip(){
		GameObject item = inventoryItems [selectionHandler.GetSelectedIndex ()][inventoryItems[selectionHandler.GetSelectedIndex()].Count - 1];

		switch (item.gameObject.tag) {
			case "Sword_Metal":
				if(!item.name.Equals("EquipedWeapon")) EquipWeapon (ref item);
				break;
			case "Sword_Stone":
				if(!item.name.Equals("EquipedWeapon")) EquipWeapon (ref item);
				break;
			case "Sword_Wood":
				if(!item.name.Equals("EquipedWeapon")) EquipWeapon (ref item);
				break;
			case "Spear_Stone":
				if(!item.name.Equals("EquipedWeapon")) EquipWeapon (ref item);
					break;
			case "Spear_Metal":
				if(!item.name.Equals("EquipedWeapon")) EquipWeapon (ref item);
				break;
			case "WaterSkin":
				item.GetComponent<WaterSkin> ().DrinkWater ();
				break;
			case "Bridge":
				Destroy(item.GetComponent("Collection"));
				item.GetComponent<Bridge> ().SetBridge ();
				break;
			case "Ladder":
				Destroy(item.GetComponent("Collection"));
				//item.GetComponent<Ladder> ().SetLadder ();
				break;
			case "Raw_Meat":
				bool consume = (player.GetComponent<FoodLevel> ().foodLevel < 100f || player.GetComponent<Health> ().health < 100f);
				item.GetComponent<RawMeat> ().CampDistance ();

				if (item.GetComponent<RawMeat> ().distance >= 5f && consume) {
					RemoveObject ();
					item.GetComponent<RawMeat> ().EatMeat ();
					Destroy (item);
				}
				else if(item.GetComponent<RawMeat> ().distance < 5f){
					RemoveObject ();
					GameObject cooked = Instantiate (Resources.Load ("Cooked_Meat")) as GameObject;
					AddNewObject (cooked);
					Destroy (item);
				}

				break;
			case "Cooked_Meat":
				if (player.GetComponent<FoodLevel> ().foodLevel < 100f || player.GetComponent<Health> ().health < 100f) {
					item.GetComponent<CookedMeat> ().EatMeat ();
					RemoveObject ();
					Destroy (item);
				}
				break;
			case "Torch":
				item.GetComponentInChildren<SpriteRenderer> ().enabled = true;
				if (item.transform.FindChild ("Fire") != null) {
					item.transform.FindChild ("Fire").gameObject.SetActive (true);
				}
			break;
			case "CampFire":
				Destroy (item.GetComponent ("Collection"));
				item.GetComponent<Campfire> ().isActive = true;
				item.transform.FindChild ("Fire").gameObject.SetActive (true);
				RemoveObject ();
				break;
		}

		selectionHandler = new SelectionHandler<GameObject> (inventoryItems);
		PrintOutObjectNames ();
	}

	private void EquipWeapon(ref GameObject newWeapon){
		//Unequip the current weapon if one is equiped
		GameObject currentlyEquiped = GameObject.Find ("EquipedWeapon");
		if (currentlyEquiped != null) {
			UnEquipItem (ref currentlyEquiped);
		}

		//equip the new desired weapon
		EquipItem(ref newWeapon);
	}

	private void UnEquipItem(ref GameObject currentlyEquiped){
		foreach (Collider comp in currentlyEquiped.GetComponentsInChildren<Collider>()) {
			comp.enabled = false;
		}
		if (currentlyEquiped.GetComponent<Rigidbody> () != null)
			currentlyEquiped.GetComponent<Rigidbody> ().isKinematic = true;
		currentlyEquiped.transform.FindChild("Trail").gameObject.SetActive(false);
		currentlyEquiped.GetComponentInChildren<SpriteRenderer> ().enabled = false;
		currentlyEquiped.layer = LayerMask.NameToLayer ("Collectable");
		currentlyEquiped.name = currentlyEquiped.tag;
		currentlyEquiped.transform.parent = GameObject.Find ("CraftedItems").transform;
	}

	private void EquipItem(ref GameObject newWeapon){
		newWeapon.transform.parent = weaponHolder.transform;
		newWeapon.layer = LayerMask.NameToLayer ("Default");
		foreach (Collider comp in newWeapon.GetComponentsInChildren<Collider>()) {
			comp.enabled = true;
		}
		if (newWeapon.GetComponent<Rigidbody> () != null)
			newWeapon.GetComponent<Rigidbody> ().isKinematic = false;
		newWeapon.GetComponentInChildren<SpriteRenderer> ().enabled = true;
		newWeapon.transform.FindChild("Trail").gameObject.SetActive(true);
		weaponHolder.GetComponent<WeaponController> ().myWeapon = newWeapon;
		newWeapon.transform.parent = weaponHolder.transform;
		newWeapon.name = "EquipedWeapon";
	}

	//get the inventory
	public SortedDictionary<string, List<GameObject>> GetInventoryItems()
	{
		return inventoryItems;
	}
}

//current issues
