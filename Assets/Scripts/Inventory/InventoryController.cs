
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{
	public GameObject[] chickens = new GameObject[4];
	//text object that displays the items in the inventory
	public GameObject[] contents;
	//these are the numberpad ui elements that are being set with the inventory items

	public GameObject inventory;
	public bool mousePressed = false;

	private GameObject weaponHolder;
	public GameObject currentlyEquiped;

	public List<GameObject> inventoryItems;
	//contains all the gameobjects collected

	private GameObject player;
	private PlayerMovementRB playerScript;

	public int currentlySelected = -1;
	private GameObject lightOn = null;
	private GameObject chickenCurrency;
	private GameObject playerItems;

	private Vector3 originalInventoryPos;

	public AudioSource[] aSources;
	public GameObject happySparks;

	// Use this for initialization
	void Start ()
	{
		inventoryItems = new List<GameObject> ();
		chickenCurrency = GameObject.Find ("ChickenInfo");
		playerItems = GameObject.Find ("PlayerItems").gameObject;

		player = GameObject.FindGameObjectWithTag ("Player");
		playerScript = player.GetComponent<PlayerMovementRB> ();
		weaponHolder = GameObject.Find ("WeaponHolder");
		EquipWeapon (weaponHolder.GetComponent<WeaponController> ().myWeapon);
		AddNewObject (weaponHolder.GetComponent<WeaponController> ().myWeapon);
		currentlyEquiped = GameObject.Find ("WeaponHolder").GetComponent<WeaponController> ().myWeapon;
		contents [0].transform.GetChild (0).gameObject.SetActive (true);

		originalInventoryPos = transform.GetComponent<RectTransform> ().localPosition;

		aSources = GetComponents<AudioSource> ();

		PrintOutObjectNames ();
	}


	void Update ()
	{
		//remove need to use enter when using numkeys?

		//This is for hotkey press checks
		if (Input.GetKeyDown ("1") || Input.GetKeyDown ("2") || Input.GetKeyDown ("3") || Input.GetKeyDown ("4") || Input.GetKeyDown ("5")) {
			currentlySelected = GetHotKeyValues (currentlySelected);
			if (currentlySelected != -1) {
				UseEquip ();
			}
			PrintOutObjectNames ();
		}
	}

	//determines if a key was pressed and determine the assosiated value for that button press based on category and item keycode
	public int GetHotKeyValues (int currentItem)
	{
		int itemName = currentItem;

		for (int i = 0; i < contents.Length; i++) {
			if (contents [i] == null)
				continue;
			
			string num = contents [i].transform.GetChild (1).GetComponentInChildren<Text> ().text.ToString (); //get the number key set in the inventory gui

			int numI = int.Parse (num); //set the value to an int to find that key value in the keycodes dict
			numI--;

			if ((Input.GetKeyDown (num)) && inventoryItems.Count > numI && numI != -1) {
				//check to see if player wants to remove or not the item
				if (Input.GetKey (KeyCode.LeftShift) && !inventoryItems [numI].tag.Contains ("Campfire") && !inventoryItems [numI].tag.Contains ("Electric Antenna")) {
					currentlySelected = numI;
					RemoveObject ();
					itemName = -1;
				} else {
					inventory.GetComponent<InventoryDisplay> ().ResetItemDetailsLoc ();
					inventory.GetComponent<InventoryDisplay> ().ShowItemInfo (numI);
					itemName = numI;
				}

				break;
			} else if (!mousePressed) {
				itemName = -1;
			}
		}

		return itemName;
	}

	//display to the screen all the items in the inventory
	public void PrintOutObjectNames ()
	{
		inventory.GetComponent<InventoryDisplay> ().ResetDisplaySprites ();

		int count = 1;
		foreach (GameObject objs in inventoryItems) {
			//check if the current key is what is select to display to the user that what item is selected
			if (objs.GetComponentInChildren<SpriteRenderer> () != null)
				contents [count - 1].GetComponent<Image> ().sprite = objs.GetComponentInChildren<SpriteRenderer> ().sprite;
			else
				contents [count - 1].GetComponent<Image> ().sprite = objs.GetComponent<Sprite3DImages> ().texture3DImages;

			//highlight if equiped
			if (objs.name.Contains ("Equiped"))
				contents [count - 1].transform.GetChild (0).gameObject.SetActive (true);

			count++;

			//make sure we do not exceed the number of slots available
			if (count - 1 >= contents.Length)
				break;
		}
	}

	//add collected objects to the inventory and disable/remove those items from the world
	public void AddNewObject (GameObject obj)
	{
		//	If inventory full, return.
		if (inventoryItems.Count == 5 || obj == null) {
			GetComponents<AudioSource> () [5].Play ();
			return;
		}

		//	Juice
		GetComponents<AudioSource> () [4].Play ();
		if (happySparks != null)
			Instantiate (happySparks, obj.transform.position, Quaternion.identity); //create wonderful particles

		//	Remove object highlight
		if (obj.layer.Equals ("Collectable"))
			obj.GetComponentInChildren<SpriteRenderer> ().color = obj.GetComponent<Collection> ().defaultCol;

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

		//delete gameobject from world
		if (!obj.name.Equals ("EquipedWeapon")) {
			obj.transform.parent = playerItems.transform;

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
			if (obj.tag.Equals ("Chicken")) {
				obj.GetComponent<NavMeshAgent> ().enabled = false;
				obj.transform.FindChild ("Name").GetComponent<MeshRenderer> ().enabled = false;
			}
		}

		inventoryItems.Add (obj);
		PrintOutObjectNames ();
	}

	//remove an object from the inventory based on which on the user has selected
	public void RemoveObject ()
	{
		//make sure key does exist
		if (inventoryItems.Count > currentlySelected && currentlySelected != -1) {
			if (Application.loadedLevelName.Equals ("ShopCenter")) {
				chickenCurrency.GetComponent<Currency> ().currency += (ReadRecipeJSON.items_List [inventoryItems [currentlySelected].tag].cost / 2);
			}

			if (currentlySelected != -1)
				contents [currentlySelected].transform.GetChild (0).gameObject.SetActive (false);

			GameObject inventoryItem = inventoryItems [currentlySelected];
			DropItem (currentlySelected);

			inventoryItem.GetComponent<Collection> ().onMouseOver = false;

			inventoryItem.transform.parent = null;
			inventoryItems.Remove (inventoryItem);

			inventory.GetComponent<InventoryDisplay> ().itemDetails.transform.GetComponent<CanvasGroup> ().alpha = 0;
			inventory.GetComponent<InventoryDisplay> ().itemDetails.transform.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			inventory.GetComponent<InventoryDisplay> ().itemDetails.transform.GetComponent<CanvasGroup> ().interactable = false;

			currentlySelected = -1;

			if ((player == null && inventoryItem != null) || inventoryItem.tag == "Torchick" || Application.loadedLevelName == "ShopCenter")
				Destroy (inventoryItem);

			PrintOutObjectNames ();
		}
	}

	//remove all the items that are used to craft an item
	public void RemoveInventoryItems (Dictionary<string, int> consumableItems)
	{
		foreach (KeyValuePair<string, int> itemNeeded in consumableItems) {
			//check if the value is one in the inventory
			if (inventoryItems [itemNeeded.Value] != null) {
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
		GameObject obj = inventoryItems [key];
		if (obj.name.Equals ("EquipedWeapon"))
			UnEquipItem (obj);

		obj.SetActive (true);

		//remove gameobject from inventory
		foreach (Collider comp in obj.GetComponentsInChildren<Collider>()) {
			comp.enabled = true;
		}
		//if (obj.GetComponent<Collection> () != null && !obj.tag.Equals ("Chicken"))
		//	obj.GetComponent<Collection> ().enabled = true;
		if (obj.GetComponent<Rigidbody> () != null)
			obj.GetComponent<Rigidbody> ().isKinematic = false;
		if (obj.GetComponentInChildren<SpriteRenderer> () != null)
			obj.GetComponentInChildren<SpriteRenderer> ().enabled = true;
		else if (obj.GetComponent<SpriteRenderer> () != null)
			obj.GetComponent<SpriteRenderer> ().enabled = true;
		else
			obj.GetComponent<MeshRenderer> ().enabled = true;
		if (!obj.tag.Equals ("CampFire") && obj.transform.FindChild ("Fire") != null)
			obj.transform.FindChild ("Fire").gameObject.SetActive (false);
		if (obj.transform.FindChild ("SpotLight") != null)
			obj.transform.FindChild ("SpotLight").gameObject.SetActive (false);

		if (obj.tag.Equals ("Chicken") || obj.tag.Equals ("TorChick")) {
			obj.GetComponent<NavMeshAgent> ().enabled = true;
			obj.transform.FindChild ("Name").GetComponent<MeshRenderer> ().enabled = true;

			if (obj.GetComponent<Chicken> ().isCaged  && obj.GetComponent<Collection> () != null) {
				Destroy (obj.GetComponent<Collection> ());
			}
		}

		if (player != null) {
			Vector3 playerPos = player.transform.position;
			float playerWidth = player.GetComponentInChildren<SpriteRenderer> ().bounds.size.x; //get the width of the player so thrown object won't be inside the player
			obj.transform.position = new Vector3 (playerPos.x + playerWidth + 1, playerPos.y, playerPos.z);
			if (obj.tag != ("Chicken") && Application.loadedLevelName != "ShopCenter")
				StartCoroutine (MakeCollectable(1f,obj));
		}

		if (!obj.tag.Equals ("Chicken"))
			obj.layer = LayerMask.NameToLayer ("Collectable");
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
		if (inventoryItems.Count > currentlySelected && currentlySelected == -1 && Application.loadedLevelName.Equals ("ShopCenter"))
			return;

		GameObject item = inventoryItems [currentlySelected];

		switch (item.gameObject.tag) {
		case "Cooked_Meat":
			if (player.GetComponent<Health> ().health < 100f) {
				item.GetComponent<EatFood> ().EatMeat ();
				RemoveObject ();
				Destroy (item);
			}
			break;
		case "Nut":
			if (player.GetComponent<Health> ().health < 100f) {
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
				item.transform.parent = player.transform;
			} else {
				lightOn = null;
				item.GetComponentInChildren<SpriteRenderer> ().enabled = false;
				item.transform.FindChild ("Fire").gameObject.SetActive (false);
				item.transform.parent = playerItems.transform;
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
				item.transform.parent = player.transform;
			} else {
				lightOn = null;
				item.GetComponentInChildren<SpriteRenderer> ().enabled = false;
				item.transform.FindChild ("Spotlight").gameObject.SetActive (false);
				item.transform.parent = playerItems.transform;
			}
			break;
		case "CampFire":
			if (player.GetComponent<PlayerMovementRB> ().hexImIn != null) {
				RemoveObject ();
				item.layer = LayerMask.NameToLayer ("Default");
				Destroy (item.GetComponent ("Collection"));
				item.GetComponent<Campfire> ().isActive = true;
				item.transform.GetChild (0).gameObject.SetActive (true);

				item.transform.position = player.GetComponent<PlayerMovementRB> ().hexImIn.transform.position;
				item.transform.parent = player.GetComponent<PlayerMovementRB> ().hexImIn.transform;
				player.GetComponent<PlayerMovementRB> ().hexImIn.GetComponent<HexControl> ().protectedHex = true;
				player.GetComponent<PlayerMovementRB> ().hexImIn.transform.FindChild("HexOutline").gameObject.layer = LayerMask.NameToLayer ("Ground");
			}
			break;
		case "Boots_of_Leporine_Swiftness":
			if (player.GetComponent<PlayerMovementRB> () != null) {
				if (!item.name.Equals ("EquipedEquipment")) {
					player.GetComponent<PlayerMovementRB> ().addSpeed = 400.0f;
					player.GetComponent<PlayerMovementRB> ().maxSpeed = 20f;
					item.name = "EquipedEquipment";
				} else {
					player.GetComponent<PlayerMovementRB> ().addSpeed = 200.0f;
					player.GetComponent<PlayerMovementRB> ().maxSpeed = 10f;
					item.name = item.tag;
				}
			}
			break;
		case "Electric_Antenna":
			if (player.GetComponent<PlayerMovementRB> ().hexImIn != null) {
				RemoveObject ();
				item.layer = LayerMask.NameToLayer ("Default");
				Destroy (item.GetComponent ("Collection"));
				item.transform.GetChild (0).gameObject.SetActive (true); //put some glowing pulsating juice around the antenna

				item.transform.position = new Vector3 (player.GetComponent<PlayerMovementRB> ().hexImIn.transform.position.x,
					player.GetComponent<PlayerMovementRB> ().hexImIn.transform.position.y + player.GetComponent<PlayerMovementRB> ().hexImIn.transform.localScale.y,
					player.GetComponent<PlayerMovementRB> ().hexImIn.transform.position.z);
				item.transform.parent = player.GetComponent<PlayerMovementRB> ().hexImIn.transform;
				player.GetComponent<PlayerMovementRB> ().hexImIn.transform.FindChild("HexOutline").gameObject.layer = LayerMask.NameToLayer ("Default");

				item.GetComponent<Electric_Antenna> ().EnableElectric_Antenna (player.GetComponent<PlayerMovementRB> ().hexImIn);
			}
			break;
		default:
				//will equip weapons if the item is a weapon
			if (item.gameObject.tag.Contains ("Sword") || item.gameObject.tag.Contains ("Spear") || item.gameObject.tag.Contains ("Axe") || item.gameObject.tag.Contains ("Hammer") ||
			    item.gameObject.tag.Contains ("Net") || item.gameObject.tag.Contains ("Scratch")) {
				if (!item.name.Equals ("EquipedWeapon"))
					EquipWeapon (item);
				else {
					if (currentlyEquiped != null) {
						currentlyEquiped = GameObject.Find ("WeaponHolder").GetComponent<WeaponController> ().myWeapon;
						UnEquipItem (currentlyEquiped);
					}
				}
			}
			break;
		}

		if (currentlySelected != -1)
			inventory.GetComponent<InventoryDisplay> ().ShowItemInfo (currentlySelected);
		PrintOutObjectNames ();
	}

	private void EquipWeapon (GameObject newWeapon)
	{
		weaponHolder = GameObject.Find ("WeaponHolder");
		if (weaponHolder != null)
			currentlyEquiped = weaponHolder.GetComponent<WeaponController> ().myWeapon;

		//Unequip the current weapon if one is equiped
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
		
		currentlyEquiped = null;
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
		currentlyEquiped = newWeapon;
	}

	//get the inventory
	public List<GameObject> GetInventoryItems ()
	{
		return inventoryItems;
	}

	public void moveGameObjectsParent ()
	{
		for (int i = 0; i < inventoryItems.Count; i++) {
			if (inventoryItems [i].transform.parent != playerItems.transform && inventoryItems[i] != currentlyEquiped)
				inventoryItems [i].transform.parent = playerItems.transform;
		}
	}

	void OnLevelWasLoaded (int level)
	{
		chickenCurrency = GameObject.Find ("ChickenInfo");
		playerItems = GameObject.Find ("PlayerItems").gameObject;

		/*if (!Application.loadedLevelName.Equals ("ShopCenter") && !Application.loadedLevelName.Equals ("Credits")) {
			player = GameObject.FindGameObjectWithTag ("Player");
			playerScript = player.GetComponent<PlayerMovementRB> ();
			weaponHolder = GameObject.Find ("WeaponHolder");
		}*/
	}

	IEnumerator MakeCollectable(float time, GameObject item){
		yield return new WaitForSeconds(time);
		if (item.GetComponent<Collection>() != null)
			item.GetComponent<Collection> ().enabled = true;
	}
}
