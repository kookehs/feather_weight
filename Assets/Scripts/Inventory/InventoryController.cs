
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{

	//text object that displays the items in the inventory
	public Sprite defaultSprite;
	public GameObject[] contents;
	public RecipesController recController;
	public GameObject inventory;
	public GameObject itemDetails;
	public bool mousePressed = false;

	private GameObject weaponHolder;
	private GameObject currentlyEquiped;

	public Dictionary<string, List<GameObject>> inventoryItems;
	private Dictionary<string,bool> _specialEquipped;
	//contains all the gameobjects collected

	private ReadRecipeJSON jsonData;
	private Dictionary<string, string> categories;
	//used so the inventory can be sorted and searched through
	public Dictionary<int, string> keyCodes;

	private GameObject player;
	private PlayerMovementRB playerScript;

	private Vector3 itemDefaultLoc;
	public string category = "";
	public string currentlySelected = "";

	public Dictionary<string,bool> specialEquipped {
		set { this._specialEquipped = value; }
	}

	// Use this for initialization
	void Start ()
	{
		inventoryItems = new Dictionary<string, List<GameObject>> ();
		jsonData = new ReadRecipeJSON ();
		keyCodes = new Dictionary<int, string> ();
		categories = jsonData.GetRecipeItemsCategories ();
		categories.Add ("Collectable", "Collectable");
		itemDefaultLoc = itemDetails.transform.position;

		player = GameObject.Find ("Player");
		playerScript = player.GetComponent<PlayerMovementRB> ();
		weaponHolder = GameObject.Find ("WeaponHolder");
		weaponHolder.GetComponent<WeaponController> ().myWeapon.name = "EquipedWeapon";
		AddNewObject (weaponHolder.GetComponent<WeaponController> ().myWeapon);

		currentlyEquiped = GameObject.Find ("WeaponHolder").GetComponent<WeaponController> ().myWeapon;
	}


	void Update ()
	{
		//make sure we are in the inventory first before doing anything
		if (inventory.GetComponent<InventoryDisplay>().openClose && inventory.GetComponent<InventoryDisplay>().focus) {
			if (category == "") {
				//first use a hotkey to select a category to work with
				category = GetHotKeyValues ("", -1);
			}

			//confirm your selction to use the item
			if (Input.GetKeyUp (KeyCode.Return)) {
				UseEquip ();
			}

			//discard your selction
			if (Input.GetKeyUp (KeyCode.X)) {
				RemoveObject ();
			}

			//undo selection of category
			if (Input.GetKeyUp (KeyCode.Escape)) {
				category = "";
				currentlySelected = "";
				DisplayCategory ();
				mousePressed = false;
			}

			//now determine and select the item through a new tear of hotkeys
			if (category != "") {
				currentlySelected = GetHotKeyValues (currentlySelected, -1);
				PrintOutObjectNames ();
			}
		} else if(!mousePressed){
			category = "";
			currentlySelected = "";
			DisplayCategory ();
		}
	}

	public void DisplayCategory ()
	{
		ResetDisplaySprites ();
		itemDetails.GetComponentInChildren<Text> ().text = "";
		itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 0;
		keyCodes = new Dictionary<int, string> ();

		int size = categories.Count;
		string[] temp = new string[size];
		int count = 1;

		//find a collectable item in the list
		foreach (KeyValuePair<string, List<GameObject>> item in inventoryItems) {
			if (!categories.ContainsKey (item.Key) && !item.Key.Equals ("Cooked_Meat")) {
				GameObject col = Resources.Load ("Collectable") as GameObject;
				//insert the first collectable item image texture to represent the categories displayed in the gui num spots
				if(col != null)
					contents [count - 1].GetComponent<Image> ().sprite = col.GetComponent<SpriteRenderer>().sprite;
				keyCodes.Add (count, "Collectable");
				temp [count] = "Collectable";
				count++;
				break;
			}
		}

		//determine items in the inventory that need to be labeled in a category for inventory
		foreach (KeyValuePair<string, string> obj in categories) {
			if (inventoryItems.ContainsKey (obj.Key) && !temp.Contains (obj.Value)) {
				GameObject intCat = Resources.Load (obj.Value) as GameObject;
				//insert the first items image texture to represent the categories displayed in the gui num spots
				if(intCat != null)
					contents [count - 1].GetComponent<Image> ().sprite = intCat.GetComponent<SpriteRenderer>().sprite;
				keyCodes.Add (count, obj.Value);
				temp [count] = obj.Value;
				count++;
			}

			//case just for cooked meat is it is neither a collectable nor a crafted item
			else if (inventoryItems.ContainsKey ("Cooked_Meat") && !temp.Contains ("Survival")) {
				GameObject survCat = Resources.Load ("Survival") as GameObject;
				if(survCat != null)
					contents [count - 1].GetComponent<Image> ().sprite = survCat.GetComponent<SpriteRenderer>().sprite;
				keyCodes.Add (count, "Survival");
				temp [count] = "Survival";
				count++;
			}

			if (count - 1 >= contents.Length)
				break;
		}
	}

	//determines if a key was pressed and determine the assosiated value for that button press based on category and item keycode
	public string GetHotKeyValues (string startName, int numB)
	{
		string itemName = startName;
		for (int i = 0; i < contents.Length; i++) {
			string num = contents [i].transform.GetChild (0).GetComponentInChildren<Text> ().text.ToString (); //get the number key set in the inventory gui
			int numI = int.Parse (num); //set the value to an int to find that key value in the keycodes dict

			if ((Input.GetKeyUp (num) || numB == numI) && keyCodes.Count >= numI && keyCodes.ContainsKey (numI)) {
				if (category != "" && inventoryItems.ContainsKey (keyCodes [numI])) {
					ShowItemInfo (numI);
					itemDetails.transform.position = itemDefaultLoc;
				}
				itemName = keyCodes [numI];
			}
		}
		return itemName;
	}

	private void ResetDisplaySprites ()
	{
		for (int i = 0; i < contents.Length; i++) {
			contents [i].GetComponent<Image> ().sprite = defaultSprite;
		}
	}

	public void ShowItemInfo(int numI){
		string totalCount = (inventoryItems [keyCodes [numI]].Count > 1 ? inventoryItems [keyCodes [numI]].Count.ToString () : "1"); //so that if the item has more then one occurance then display total count

		string info = "X - Discard";
		if (inventoryItems [keyCodes [numI]] [0].name == "EquipedWeapon")
			info = "Currently Equiped";

		itemDetails.GetComponentInChildren<Text> ().text = keyCodes [numI] + " | " + totalCount + "\n" + info;
		itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 1;
	}

	//display to the screen all the items in the inventory
	public void PrintOutObjectNames ()
	{
		ResetDisplaySprites ();
		keyCodes = new Dictionary<int, string> ();

		int count = 1;
		foreach (KeyValuePair<string, List<GameObject>> objs in inventoryItems) {
			if ((categories.ContainsKey (objs.Key) && categories [objs.Key].Equals (category)) || (category.Equals ("Collectable") && !categories.ContainsKey (objs.Key) && !objs.Key.Equals ("Cooked_Meat"))) {
				//check if the current key is what is select to display to the user that what item is selected
				if (inventoryItems [objs.Key] [0].GetComponentInChildren<SpriteRenderer> () != null)
					contents [count - 1].GetComponent<Image> ().sprite = inventoryItems [objs.Key] [0].GetComponentInChildren<SpriteRenderer> ().sprite;
				else
					contents [count - 1].GetComponent<Image> ().sprite = inventoryItems [objs.Key] [0].GetComponent<Sprite3DImages> ().texture3DImages;
				keyCodes.Add (count, objs.Key);
				count++;
			}
			//case just for cooked meat is it is neither a collectable nor a crafted item
			else if (category.Equals ("Survival") && objs.Key.Equals ("Cooked_Meat")) {
				//check if the current key is what is select to display to the user that what item is selected
				contents [count - 1].GetComponent<Image> ().sprite = inventoryItems [objs.Key] [0].GetComponentInChildren<SpriteRenderer> ().sprite;
				keyCodes.Add (count, objs.Key);
				count++;
			}

			if (count - 1 >= contents.Length)
				break;
		}

		//have a check for if the category is now empty then return to the categories list
		if (count == 0)
			DisplayCategory ();
	}

	//add collected objects to the inventory and disable/remove those items from the world
	public void AddNewObject (GameObject obj)
	{
		if (obj.layer.Equals ("Collectable"))
			obj.GetComponentInChildren<SpriteRenderer> ().color = obj.GetComponent<Collection> ().defaultCol; //remove object highlight

		//Remove Clone from Objects Name
		if (obj.name.Contains ("(Clone)")) {
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
		} else {
			inventoryItems [obj.tag].Add (obj);
		}

		//delete gameobject from world
		if (!obj.name.Equals ("EquipedWeapon")) {
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
			if (obj.transform.FindChild ("Trail") != null)
				obj.transform.FindChild ("Trail").gameObject.SetActive (false);
			if (obj.transform.FindChild ("Fire") != null)
				obj.transform.FindChild ("Fire").gameObject.SetActive (false);
		}

		DisplayCategory ();

		StartCoroutine ("TurnOffHover");
	}

	IEnumerator TurnOffHover ()
	{
		yield return new WaitForSeconds (0.2f);
		player.GetComponent<PlayerMovementRB> ().mouseHovering = false;
	}

	//remove an object from the inventory based on which on the user has selected
	public void RemoveObject ()
	{
		//make sure key does exist
		if (inventoryItems.ContainsKey (currentlySelected)) {
			//check how many of those items the player has if they have more then one item then just remove from gameObject list
			//if there is only one item then remove entire object key
			if (inventoryItems [currentlySelected].Count > 1) {
				DropItem (currentlySelected);
				inventoryItems [currentlySelected].RemoveAt (inventoryItems [currentlySelected].Count - 1);
			} else if (inventoryItems [currentlySelected].Count == 1) {
				DropItem (currentlySelected);
				inventoryItems.Remove (currentlySelected);
			}

			PrintOutObjectNames ();
		}
	}

	public void RemoveSetBridgeObject (Transform riverPoint)
	{
		//make sure key does exist then place bridge in the correct place
		if (inventoryItems.ContainsKey (currentlySelected)) {
			GameObject bridge = inventoryItems [currentlySelected] [inventoryItems [currentlySelected].Count - 1];
			RemoveObject ();
			bridge.transform.position = riverPoint.position;
			bridge.transform.rotation = riverPoint.rotation;
			bridge.transform.localScale = riverPoint.localScale;
		}
	}

	public void RemoveSetLadderObject (Transform cliffPoint)
	{
		//make sure key does exist then place ladder in the correct place
		if (inventoryItems.ContainsKey (currentlySelected)) {
			GameObject ladder = inventoryItems [currentlySelected] [inventoryItems [currentlySelected].Count - 1];
			RemoveObject ();
			ladder.transform.position = cliffPoint.position;
			ladder.transform.rotation = cliffPoint.rotation;
			ladder.transform.localScale = cliffPoint.localScale;
		}
	}

	//remove all the items that are used to craft an item
	public void RemoveInventoryItems (Dictionary<string, int> consumableItems)
	{
		foreach (KeyValuePair<string, int> itemNeeded in consumableItems) {
			//check if the value is one in the inventory
			if (inventoryItems.ContainsKey (itemNeeded.Key)) {
				for (int i = 0; i < consumableItems [itemNeeded.Key]; i++) {
					//destory the object from the game world
					Destroy (inventoryItems [itemNeeded.Key] [inventoryItems [itemNeeded.Key].Count - 1]);

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

		PrintOutObjectNames ();
	}

	//to drop a removed item a close distance from the player
	private void DropItem (string key)
	{
		Vector3 playerPos = player.transform.position;
		float playerWidth = player.GetComponentInChildren<SpriteRenderer> ().bounds.size.x; //get the width of the player so thrown object won't be inside the player
		int index = inventoryItems [key].Count - 1; //the last item of the key's type will be dropped

		GameObject obj = inventoryItems [key] [index];
		if (obj.name.Equals ("EquipedWeapon"))
			UnEquipItem (ref obj);

		obj.SetActive (true);

		//remove gameobject from inventory
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
		if (obj.transform.FindChild ("Fire") != null)
			obj.transform.FindChild ("Fire").gameObject.SetActive (false);
		
		obj.name += (index + 1);
		obj.transform.position = new Vector3 (playerPos.x + playerWidth, playerPos.y, playerPos.z);
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
	public void UseEquip ()
	{
		Debug.Log (currentlySelected);
		if (!inventoryItems.ContainsKey (currentlySelected))
			return;

		GameObject item = inventoryItems [currentlySelected] [inventoryItems [currentlySelected].Count - 1];

		switch (item.gameObject.tag) {
		case "WaterSkin":
			item.GetComponent<WaterSkin> ().DrinkWater ();
			break;
		case "Bridge":
			Destroy (item.GetComponent ("Collection"));
			item.GetComponent<Bridge> ().SetBridge ();
			break;
		case "Ladder":
			Destroy (item.GetComponent ("Collection"));
			item.GetComponent<Ladder> ().SetLadder ();
			break;
		case "Raw_Meat":
			bool consume = (player.GetComponent<FoodLevel> ().foodLevel < 100f || player.GetComponent<Health> ().health < 100f);
			item.GetComponent<RawMeat> ().CampDistance ();

			if (item.GetComponent<RawMeat> ().distance >= 5f && consume) {
				RemoveObject ();
				item.GetComponent<RawMeat> ().EatMeat ();
				Destroy (item);
			} else if (item.GetComponent<RawMeat> ().distance < 5f) {
				RemoveObject ();
				GameObject cooked = Instantiate (Resources.Load ("Cooked_Meat")) as GameObject;
				AddNewObject (cooked);
				Destroy (item);
			}

			break;
		case "Cooked_Meat":
			if (player.GetComponent<FoodLevel> ().foodLevel < 100f || player.GetComponent<Health> ().health < 100f) {
				item.GetComponent<EatFood> ().EatMeat ();
				RemoveObject ();
				Destroy (item);
			}
			break;
		case "Nut":
			if (player.GetComponent<FoodLevel> ().foodLevel < 100f || player.GetComponent<Health> ().health < 100f) {
				item.GetComponent<EatFood> ().EatMeat ();
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
		case "Boots of Leporine Swiftness":
		case "Heaven Shattering Hammer":
		case "Nikola's Armor":
			EquipSpecial (ref item);
			break;
		default:
			//will equip weapons if the item is a weapon
			if (item.gameObject.tag.Contains ("Sword") || item.gameObject.tag.Contains ("Spear") || item.gameObject.tag.Contains ("Axe")) {
				if (!item.name.Equals ("EquipedWeapon"))
					EquipWeapon (ref item);
			}
			break;
		}

		PrintOutObjectNames ();
	}

	private void EquipWeapon (ref GameObject newWeapon)
	{
		//Unequip the current weapon if one is equiped
		currentlyEquiped = GameObject.Find ("WeaponHolder").GetComponent<WeaponController> ().myWeapon;
		Debug.Log (currentlyEquiped);
		if (currentlyEquiped != null) {
			UnEquipItem (ref currentlyEquiped);
		}

		//equip the new desired weapon
		EquipItem (ref newWeapon);
	}

	private void UnEquipItem (ref GameObject currentlyEquiped)
	{
		foreach (Collider comp in currentlyEquiped.GetComponentsInChildren<Collider>()) {
			comp.enabled = false;
		}
		if (currentlyEquiped.GetComponent<Rigidbody> () != null)
			currentlyEquiped.GetComponent<Rigidbody> ().isKinematic = true;
		currentlyEquiped.transform.FindChild ("Trail").gameObject.SetActive (false);
		currentlyEquiped.GetComponentInChildren<SpriteRenderer> ().enabled = false;
		currentlyEquiped.layer = LayerMask.NameToLayer ("Collectable");
		currentlyEquiped.name = currentlyEquiped.tag;
		currentlyEquiped.transform.parent = GameObject.Find ("CraftedItems").transform;
	}

	private void EquipItem (ref GameObject newWeapon)
	{
		newWeapon.layer = LayerMask.NameToLayer ("Default");
		foreach (Collider comp in newWeapon.GetComponentsInChildren<Collider>()) {
			comp.enabled = true;
		}
		if (newWeapon.GetComponent<Rigidbody> () != null)
			newWeapon.GetComponent<Rigidbody> ().isKinematic = false;
		newWeapon.GetComponentInChildren<SpriteRenderer> ().enabled = true;
		newWeapon.transform.FindChild ("Trail").gameObject.SetActive (true);
		weaponHolder.GetComponent<WeaponController> ().equipWeapon (ref newWeapon);
	}

	private void EquipSpecial (ref GameObject special) {
		string thing = special.gameObject.tag;
		switch (thing) {
		case "Boots of Leporine Swiftness":
			float speed_bonus = 2f;
			if (_specialEquipped [thing]) {
				playerScript.maxSpeed -= speed_bonus;
				_specialEquipped [thing] = false;
			} else {
				playerScript.maxSpeed += speed_bonus;
				_specialEquipped [thing] = true;
			}
			break;
		case "Heaven Shattering Hammer":
			if (_specialEquipped [thing]) {
				UnEquipItem (ref special);
				_specialEquipped [thing] = false;
			} else {
				EquipWeapon (ref special);
				_specialEquipped [thing] = true;
			}
			break;
		case "Nikola's Armor":
			playerScript.lightning_armor_on = !playerScript.lightning_armor_on;
			break;
		default:
			break;
		}
	}

	//get the inventory
	public Dictionary<string, List<GameObject>> GetInventoryItems ()
	{
		return inventoryItems;
	}
}

//current issues
//at some point I will need to create an inventory cap that you can only have 9 items per category an you can only have a max of 5 items per item type
