
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{

	//text object that displays the items in the inventory
	public GameObject[]contents; //these are the numberpad ui elements that are being set with the inventory items

	public GameObject inventory;
	public bool mousePressed = false;

	private GameObject weaponHolder;
	public GameObject currentlyEquiped;

	public List<GameObject> inventoryItems;
	private Dictionary<string,bool> _specialEquipped;
	//contains all the gameobjects collected

	private GameObject player;
	private PlayerMovementRB playerScript;

	public int currentlySelected = -1;
	private GameObject lightOn = null;

    public AudioSource[] aSources;

	public Dictionary<string,bool> specialEquipped {
		set { this._specialEquipped = value; }
	}

	// Use this for initialization
	void Start ()
	{
		inventoryItems = new List<GameObject> ();

		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<PlayerMovementRB> ();
		weaponHolder = GameObject.Find ("WeaponHolder");
		EquipWeapon (weaponHolder.GetComponent<WeaponController>().myWeapon);
		AddNewObject (weaponHolder.GetComponent<WeaponController> ().myWeapon);
		currentlyEquiped = GameObject.Find ("WeaponHolder").GetComponent<WeaponController> ().myWeapon;

        aSources = GetComponents<AudioSource>();

		PrintOutObjectNames ();
	}


	void Update ()
	{
		//This is for hotkey press checks
		if (Input.anyKey) {
			currentlySelected = GetHotKeyValues (currentlySelected);
			PrintOutObjectNames ();
		}

		//confirm your selction to use the item
		if (Input.GetKeyUp (KeyCode.Return)) {
			UseEquip ();
		}

		//discard your selction
		if (Input.GetKeyUp (KeyCode.X)) {
			RemoveObject ();
		}
	}

	//determines if a key was pressed and determine the assosiated value for that button press based on category and item keycode
	public int GetHotKeyValues (int currentItem)
	{
		int itemName = currentItem;
		for (int i = 0; i < contents.Length; i++) {
			if (contents [i] == null)
				continue;
				
			string num = contents [i].transform.GetChild (0).GetComponentInChildren<Text> ().text.ToString (); //get the number key set in the inventory gui

			int numI = int.Parse (num); //set the value to an int to find that key value in the keycodes dict
			numI--;

			if ((Input.GetKeyDown (num)) && inventoryItems.Count > numI && numI != -1) {
				inventory.GetComponent<InventoryDisplay>().ShowItemInfo (numI);
				itemName = numI;
			}
		}
			
		return itemName;
	}

	//display to the screen all the items in the inventory
	public void PrintOutObjectNames ()
	{
		inventory.GetComponent<InventoryDisplay>().ResetDisplaySprites ();

		int count = 1;
		foreach (GameObject objs in inventoryItems) {				
			//check if the current key is what is select to display to the user that what item is selected
			if (objs.GetComponentInChildren<SpriteRenderer> () != null)
				contents [count - 1].GetComponent<Image> ().sprite = objs.GetComponentInChildren<SpriteRenderer> ().sprite;
			else
				contents [count - 1].GetComponent<Image> ().sprite = objs.GetComponent<Sprite3DImages> ().texture3DImages;

			count++;

			//make sure we do not exceed the number of slots available
			if (count - 1 >= contents.Length)
				break;
		}
	}

	//add collected objects to the inventory and disable/remove those items from the world
	public void AddNewObject (GameObject obj)
	{
		if (inventoryItems.Count == 5 || obj == null)
			return;
		
        GetComponents<AudioSource>()[4].Play();

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
		inventoryItems.Add (obj);

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

		PrintOutObjectNames ();

	}

	//remove an object from the inventory based on which on the user has selected
	public void RemoveObject ()
	{
		//make sure key does exist
		if (inventoryItems.Count > currentlySelected && currentlySelected != -1) {
			DropItem (currentlySelected);
			inventoryItems [currentlySelected].GetComponent<Collection> ().onMouseOver = false;

			inventoryItems.Remove (inventoryItems[currentlySelected]);

			inventory.GetComponent<InventoryDisplay>().itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 0;
			currentlySelected = -1;
			PrintOutObjectNames ();
		}
	}

	public void RemoveSetBridgeObject (Transform riverPoint)
	{
		//make sure key does exist then place bridge in the correct place
		if (inventoryItems.Count > currentlySelected && currentlySelected != -1) {
			GameObject bridge = inventoryItems [currentlySelected];
			RemoveObject ();
			bridge.transform.position = riverPoint.position;
			bridge.transform.rotation = riverPoint.rotation;
			bridge.transform.localScale = riverPoint.localScale;
		}
	}

	public void RemoveSetLadderObject (Transform cliffPoint)
	{
		//make sure key does exist then place ladder in the correct place
		if (inventoryItems.Count > currentlySelected && currentlySelected != -1) {
			GameObject ladder = inventoryItems [currentlySelected];
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
			if (inventoryItems[itemNeeded.Value] != null) {
				for (int i = 0; i < itemNeeded.Value; i++) {
					//destory the object from the game world
					Destroy (inventoryItems [itemNeeded.Value]);
					inventoryItems.Remove (inventoryItems [itemNeeded.Value]);
				}
			}
		}

		PrintOutObjectNames ();
	}

	//to drop a removed item a close distance from the player
	private void DropItem (int key)
	{
		Vector3 playerPos = player.transform.position;
		float playerWidth = player.GetComponentInChildren<SpriteRenderer> ().bounds.size.x; //get the width of the player so thrown object won't be inside the player

		GameObject obj = inventoryItems [key];
		if (obj.name.Equals ("EquipedWeapon"))
			UnEquipItem (obj);

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
		else if (obj.GetComponent<SpriteRenderer> () != null)
			obj.GetComponent<SpriteRenderer> ().enabled = true;
		else
			obj.GetComponent<MeshRenderer> ().enabled = true;
		if (obj.transform.FindChild ("Fire") != null)
			obj.transform.FindChild ("Fire").gameObject.SetActive (false);
		if (obj.transform.FindChild ("SpotLight") != null)
			obj.transform.FindChild ("SpotLight").gameObject.SetActive (false);
		
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
		if (inventoryItems.Count > currentlySelected && currentlySelected == -1)
			return;

		GameObject item = inventoryItems [currentlySelected];

		switch (item.gameObject.tag) {
		case "WaterSkin":
			item.GetComponent<WaterSkin> ().DrinkWater ();
			break;
		case "Bridge":
			Destroy (item.GetComponent ("Collection"));
			item.layer = LayerMask.NameToLayer ("Ground");
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
			if (lightOn != null) {
				lightOn.GetComponentInChildren<SpriteRenderer> ().enabled = false;

				Transform lightForm = lightOn.transform.FindChild ("Fire");
				if (lightForm == null)
					lightForm = lightOn.transform.FindChild ("Spotlight");
				
				lightForm.gameObject.SetActive (false);

			}

			if (lightOn == null || !lightOn.Equals (item)) {
				lightOn = item;
				item.GetComponentInChildren<SpriteRenderer> ().enabled = true;
				item.transform.FindChild ("Fire").gameObject.SetActive (true);
			} else {
				lightOn = null;
				item.GetComponentInChildren<SpriteRenderer> ().enabled = false;
				item.transform.FindChild ("Fire").gameObject.SetActive (false);
			}
			break;
		case "Flashlight":
			if (lightOn != null && !lightOn.Equals (item)) {
				lightOn.GetComponentInChildren<SpriteRenderer> ().enabled = false;

				Transform lightForm = lightOn.transform.FindChild ("Fire");
				if (lightForm == null)
					lightForm = lightOn.transform.FindChild ("Spotlight");
				
				lightForm.gameObject.SetActive (false);
			}

			if (lightOn == null || !lightOn.Equals (item)) {
				lightOn = item;
				item.GetComponentInChildren<SpriteRenderer> ().enabled = true;
				item.transform.FindChild ("Spotlight").gameObject.SetActive (true);
			} else {
				lightOn = null;
				item.GetComponentInChildren<SpriteRenderer> ().enabled = false;
				item.transform.FindChild ("Spotlight").gameObject.SetActive (false);
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
			EquipSpecial (item);
			break;
		default:
			//will equip weapons if the item is a weapon
			if (item.gameObject.tag.Contains ("Sword") || item.gameObject.tag.Contains ("Spear") || item.gameObject.tag.Contains ("Axe")) {
				if (!item.name.Equals ("EquipedWeapon"))
					EquipWeapon (item);
			}
			break;
		}

		inventory.GetComponent<InventoryDisplay>().ShowItemInfo (currentlySelected);
		PrintOutObjectNames ();
	}

	private void EquipWeapon (GameObject newWeapon)
	{
		//Unequip the current weapon if one is equiped
		currentlyEquiped = GameObject.Find ("WeaponHolder").GetComponent<WeaponController> ().myWeapon;

		if (currentlyEquiped != null) {
			UnEquipItem (currentlyEquiped);
		}

		//equip the new desired weapon
		EquipItem (newWeapon);
	}

	private void UnEquipItem (GameObject currentlyEquiped)
	{
		foreach (Collider comp in currentlyEquiped.GetComponentsInChildren<Collider>()) {
			comp.enabled = false;
		}
		if (currentlyEquiped.GetComponent<Rigidbody> () != null)
			currentlyEquiped.GetComponent<Rigidbody> ().isKinematic = true;
		currentlyEquiped.GetComponentInChildren<SpriteRenderer> ().enabled = false;
		weaponHolder.GetComponent<WeaponController> ().unequipWeapon (currentlyEquiped);
	}

	private void EquipItem (GameObject newWeapon)
	{
		foreach (Collider comp in newWeapon.GetComponentsInChildren<Collider>()) {
			comp.enabled = true;
		}
		if (newWeapon.GetComponent<Rigidbody> () != null)
			newWeapon.GetComponent<Rigidbody> ().isKinematic = false;
		newWeapon.GetComponentInChildren<SpriteRenderer> ().enabled = true;
		weaponHolder.GetComponent<WeaponController> ().equipWeapon (newWeapon);
	}

	private void EquipSpecial (GameObject special) {
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
				UnEquipItem (special);
				_specialEquipped [thing] = false;
			} else {
				EquipWeapon (special);
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
	public List<GameObject> GetInventoryItems ()
	{
		return inventoryItems;
	}
}

//current issues
//at some point I will need to create an inventory cap that you can only have 9 items per category an you can only have a max of 5 items per item type
