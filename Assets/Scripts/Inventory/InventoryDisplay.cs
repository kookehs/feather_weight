using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryDisplay : MonoBehaviour {

	public Button removeObject;
	public Button addObject;
	public InventoryController intControl;
	public RecipesController recControl;

	private int openClose; //toggle whether the inventory is already open or not
	private float pauseTime = 5;

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

	//dialog popup that tells player they cannot craft when option is unavailable
	void OnGUI(){
		if (!recControl.isCraftable) {
			GUI.Box (new Rect (20, 10, 400, 20), "Not enough items in inventory to craft this item");
			StartCoroutine ("EndDisplayButton");
		}
	}

	//to keep the display dialog stay for a few seconds before closing
	IEnumerator EndDisplayButton(){
		yield return new WaitForSeconds(pauseTime);
		recControl.isCraftable = true;
	}
}
