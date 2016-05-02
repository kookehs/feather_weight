using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Currency : MonoBehaviour {

	public int currency = 0;

	// Use this for initialization
	void Start () {
		GetComponentInChildren<Text> ().text = currency.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		GetComponentInChildren<Text> ().text = currency.ToString ();

		//this inner code needs to be used in the wave when the chickenRoom is being opened up from the shop
		if (Input.GetKeyUp (KeyCode.Backspace)) {
			GameObject.Find ("PlayerUIElements").GetComponent<GrabPlayerUIElements> ().RestPlayerUI ();
			GameObject.Find ("InventoryContainer").GetComponent<InventoryController> ().moveGameObjectsParent ();
			
			Application.LoadLevel ("HexLayoutChickenRoom");
		}

		//this inner code needs to be used in the wave when the shop is being opened up
		if (Input.GetKeyUp (KeyCode.KeypadEnter)) {
			InventoryController inventory = GameObject.Find ("InventoryContainer").GetComponent<InventoryController> ();

			for (int i = 0; i < inventory.inventoryItems.Count; i ++){
				GameObject itemHave = inventory.inventoryItems[i];

				if (itemHave.name == "EquipedWeapon") {
					inventory.currentlySelected = i;
					inventory.UseEquip ();
				}
			}

			Application.LoadLevel ("ShopCenter");
		}

		if (Input.GetKeyUp (KeyCode.N)) {
			Application.LoadLevel ("Credits");
		}

	}
}
