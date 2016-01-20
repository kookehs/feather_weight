using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {

	public Button removeObject;
	public Button addObject;
	public InventoryController intControl;

	private int openClose; //toggle whether the inventory is already open or not

	// Use this for initialization
	void Start () {
		openClose = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Open the inventory
		if (Input.GetKeyUp ("q") && openClose == 0) {
			transform.Find("Inventory").gameObject.SetActive (true);
			openClose = 1;
		}

		//close the inventory
		if (Input.GetKeyUp ("q") && openClose > 2) {
			transform.Find("Inventory").gameObject.SetActive (false);
			openClose=0;
		}

		//change the toggle value
		if (openClose > 0)
			openClose++;
	}
}
