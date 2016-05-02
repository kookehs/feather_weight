using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RecipesController : MonoBehaviour {

	public GameObject[] contents; //text object that will diaply the recipes list
	public GameObject requirements;
	public Sprite defaultSprite;
	public bool isCraftable = true; //to determine whether or not to display the diolog telling user the item cannot be crafted
	public bool mousePressed = false;

	//data variables for recipes and user's inventory
	private GameObject inventory;
	private ReadRecipeJSON jsonData;
	private Currency checkCurrency;
	private List<string> recipeItems;
	public Dictionary<int, string> keyCodes;

	private Vector3 requirementsDefaultLoc;
	public GameObject currentlySelected;

	// Use this for initialization
	public void Start () {
		recipeItems = new List<string>();

		if (GameObject.Find ("PlayerUICurrent") != null) {
			checkCurrency = GameObject.Find ("ChickenInfo").GetComponent<Currency> ();
			inventory = GameObject.FindGameObjectWithTag ("InventoryUI");
		}
		jsonData = new ReadRecipeJSON ();
		InsertRecipeData ();
		DisplayRecipeNames ();

		requirements.transform.GetChild(0).GetComponent<CanvasGroup> ().alpha = 0;
		requirementsDefaultLoc = requirements.transform.position;
	}

	//using categories we lock in onto certain items that then let the number craft and show the item
	//1-9 set as category buttons starts
	//once inside a category then nums work for item selection
	void Update(){
		//undo selection of category
		if (Input.GetKeyDown (KeyCode.Escape)) {
			currentlySelected = null;
			mousePressed = false;
		}

		//now determine and select the item through a new tear of hotkeys
		currentlySelected = GetHotKeyValues(currentlySelected, -1);
	}

	//determines if a key was pressed and determine the assosiated value for that button press based on category and item keycode
	public GameObject GetHotKeyValues(GameObject startName, int numB){
		GameObject itemName = startName;
		for (int i = 0; i < contents.Length; i++) {
			string num = contents [i].transform.GetChild(0).GetComponentInChildren<Text> ().text.ToString(); //get the number key set in the recipe gui
			int numI = int.Parse (num); //set the value to an int to find that key value in the keycodes dict

			if (numI == numB && keyCodes.Count >= numI && keyCodes.ContainsKey (numI)) {

				itemName = Resources.Load (keyCodes [numI]) as GameObject;

				if (recipeItems.Contains (keyCodes [numI])) {
					ShowItemRequirements (itemName);
					requirements.transform.position = requirementsDefaultLoc;
				}
			}
		}
		return itemName;
	}


	//insert into the recipes dictionary the list of recipes from the json file
	public void InsertRecipeData(){
		string[] recipeNames = jsonData.GetRecipeNames ("Recipes");
		for (int i = 0; i < recipeNames.Length; i++) {
			recipeItems.Add (recipeNames [i]);
		}
	}

	//get the recipes from the dictionary and add the gui text object
	//only display those that are in the category
	public void DisplayRecipeNames(){
		//ResetDisplaySprites ();
		keyCodes = new Dictionary<int, string> ();
		GameObject[] tempContentsChecker = new GameObject[contents.Length];

		//foreach (KeyValuePair<string, List<string>> obj in recipeItems) {
		for (int count = 0; count < contents.Length; count++) {
			int displayRecipeItem = Random.Range (0, recipeItems.Count);
			if (recipeItems [displayRecipeItem] != null && !contents.Contains (tempContentsChecker [count])) {
				GameObject recipeItemDisplay = Resources.Load (recipeItems [displayRecipeItem]) as GameObject;

				if (recipeItemDisplay.GetComponentInChildren<SpriteRenderer> () != null)
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponentInChildren<SpriteRenderer> ().sprite;
				else if (recipeItemDisplay.GetComponent<SpriteRenderer> () != null)
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<SpriteRenderer> ().sprite;
				else
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<Sprite3DImages> ().texture3DImages;

				keyCodes.Add (count + 1, recipeItems [displayRecipeItem]);
				tempContentsChecker [count] = recipeItemDisplay;
			} else {
				count--;
			}
		}
	}

	//check if the player has enough chickens to buy the items if yes then add the new item
	public void CraftItem(GameObject itemToCraft){
		Dictionary<string, int> consumableItems = jsonData.GetRecipeItemsConsumables(itemToCraft.tag);

		if (checkCurrency != null && checkCurrency.currency >= consumableItems["Chicken"]) {
			checkCurrency.currency -= consumableItems ["Chicken"];

			//need to get item prefab based on name then create that an instance of that then add to inventory
			GameObject item = Instantiate(itemToCraft) as GameObject;

			if (item != null) {
                GameObject playerItems = GameObject.Find ("PlayerItems");
				item.transform.parent = playerItems.transform;
				inventory.GetComponent<InventoryController> ().AddNewObject (item);
				isCraftable = true;
                playerItems.GetComponent<AudioSource>().Play();
			}
		} else {
			isCraftable = false;
		}
	}

	//get the list of requirments or consumables needed then display them
	public void ShowItemRequirements(GameObject itemToCraft){
		//requirements.GetComponentInChildren<Text> ().text = "Item Requirements:\n";
		Dictionary<string, int> tempComsumables = jsonData.GetRecipeItemsConsumables(itemToCraft.tag);

		string info = (tempComsumables["Chicken"] + " Chickens");

		requirements.GetComponentInChildren<Text> ().text = itemToCraft.tag + " | " + info;
		requirements.transform.GetChild(0).GetComponent<CanvasGroup> ().alpha = 1;
	}
}

//continue to flesh out the key code pressing
