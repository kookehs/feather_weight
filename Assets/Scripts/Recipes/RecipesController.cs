using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RecipesController : MonoBehaviour {

	public GameObject[] contents; //text object that will diaply the recipes list
	public GameObject requirements;
	public GameObject description;
	public Sprite defaultSprite;
	public bool isCraftable = true; //to determine whether or not to display the diolog telling user the item cannot be crafted
	public bool mousePressed = false;
	public Dictionary<int, string> keyCodes;

	//data variables for recipes and user's inventory
	private GameObject inventory;
	private ReadRecipeJSON jsonData;
	private Currency checkCurrency;
	private Dictionary<string, GameItems> recipeItems;
	private int[] teirLevels = new int[3]{0, 3, 9}; //the teirs timeline waves 0(1) teir1 items added, 3(4) teir2 items added, 9(10) teir3 items added
	private int currentTeirLevel = -1;

	private Vector3 requirementsDefaultLoc;
	public GameObject currentlySelected;

	// Use this for initialization
	public void Start () {
		recipeItems = new Dictionary<string, GameItems>();

		if (GameObject.Find ("PlayerUICurrent") != null) {
			checkCurrency = GameObject.Find ("ChickenInfo").GetComponent<Currency> ();
			inventory = GameObject.FindGameObjectWithTag ("InventoryUI");
		}

		jsonData = new ReadRecipeJSON ();
		recipeItems = jsonData.GetRecipeItemsList ();

		if (WaveController.current_wave > teirLevels [currentTeirLevel])
			currentTeirLevel += 1;
		
		DisplayRecipeNames ();

		requirements.transform.GetChild(0).GetComponent<CanvasGroup> ().alpha = 0;
		requirementsDefaultLoc = requirements.transform.position;
		description.transform.GetComponent<CanvasGroup> ().alpha = 0;
	}
		
	void Update(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			currentlySelected = null;
			mousePressed = false;
		}

		//now determine and select the item through a new tear of hotkeys
		currentlySelected = GetHotKeyValues(currentlySelected, -1);
	}

	//determines if a key was pressed and determine the assosiated value for that button press
	public GameObject GetHotKeyValues(GameObject startName, int numB){
		GameObject itemName = startName;
		for (int i = 0; i < contents.Length; i++) {
			string num = contents [i].transform.GetChild(0).GetComponentInChildren<Text> ().text.ToString(); //get the number key set in the recipe gui
			int numI = int.Parse (num); //set the value to an int to find that key value in the keycodes dict

			if (numI == numB && keyCodes.Count >= numI && keyCodes.ContainsKey (numI)) {

				itemName = Resources.Load (keyCodes [numI]) as GameObject;

				if (recipeItems.ContainsKey (keyCodes [numI])) {
					ShowItemRequirements (itemName);
					requirements.transform.position = requirementsDefaultLoc;
				}
			}
		}
		return itemName;
	}

	//get the recipes from the dictionary and add the gui text object
	public void DisplayRecipeNames(){
		//ResetDisplaySprites ();
		keyCodes = new Dictionary<int, string> ();
		List<string> recNames = FisherYatesShuffle (recipeItems.Keys.ToList());
		int count = 0;

		for (int i = 0; i < recNames.Count; i++) {
			if (count > contents.Length - 1)
				break;
			
			if (recipeItems[recNames[i]].teir <= currentTeirLevel) {
				GameObject recipeItemDisplay = Resources.Load (recNames[i]) as GameObject;

				if (recipeItemDisplay.GetComponentInChildren<SpriteRenderer> () != null)
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponentInChildren<SpriteRenderer> ().sprite;
				else if (recipeItemDisplay.GetComponent<SpriteRenderer> () != null)
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<SpriteRenderer> ().sprite;
				else
					contents [count].GetComponent<Image> ().sprite = recipeItemDisplay.GetComponent<Sprite3DImages> ().texture3DImages;

				//visually show item cannot be purchased
				if (recipeItems [recNames [i]].cost > checkCurrency.currency)
					gameObject.transform.GetChild(0).GetChild(count).GetComponent<Image> ().color = new Color(255, 0, 0, 0.5f);
				else
					gameObject.transform.GetChild(0).GetChild(count).GetComponent<Image> ().color = new Color(0, 0, 0, 0.5f);

				keyCodes.Add (count + 1, recNames[i]);
				count++;
			}
		}
	}

	//get the recipes from the dictionary and add the gui text object
	public void UpdateItemAvailablity(){
		//ResetDisplaySprites ();
		Transform backdrop = gameObject.transform.GetChild (0);

		for (int i = 1; i < keyCodes.Count; i++) {
			//visually show item cannot be purchased
			if (recipeItems [keyCodes [i]].cost > checkCurrency.currency)
				backdrop.GetChild(i - 1).GetComponent<Image> ().color = new Color(255, 0, 0, 0.5f);
			else
				backdrop.GetChild(i - 1).GetComponent<Image> ().color = new Color(0, 0, 0, 0.5f);
		}
	}

	//shuffle up the items so that they can be random each time regardless of tier
	private List<string> FisherYatesShuffle(List<string> recNames){
		for(int i = 0; i < recNames.Count; i++){
			int displayRecipeItem = Random.Range (0, recNames.Count - 1);

			string tempString = recNames[i];
			recNames[i] = recNames[displayRecipeItem];
			recNames[displayRecipeItem] = tempString;
		}

		return recNames;
	}

	//check if the player has enough chickens to buy the items if yes then add the new item
	public void CraftItem(GameObject itemToCraft){
		if (itemToCraft != null && recipeItems.ContainsKey(itemToCraft.name) && checkCurrency != null && checkCurrency.currency >= recipeItems[itemToCraft.name].cost) {
			checkCurrency.currency -= recipeItems [itemToCraft.tag].cost;

			//need to get item prefab based on name then create that an instance of that then add to inventory
			GameObject item = Instantiate(itemToCraft) as GameObject;

			if (item != null) {
                GameObject playerItems = GameObject.Find ("PlayerItems");
				item.transform.parent = playerItems.transform;
				inventory.GetComponent<InventoryController> ().AddNewObject (item);
				isCraftable = true;
                playerItems.GetComponent<AudioSource>().Play();
				UpdateItemAvailablity ();
			}
		} else {
			isCraftable = false;
		}
	}

	//get the list of requirments or consumables needed then display them
	public void ShowItemRequirements(GameObject itemToCraft){
		//requirements.GetComponentInChildren<Text> ().text = "Item Requirements:\n";

		string info = (recipeItems[itemToCraft.name].cost + " Chickens");

		requirements.GetComponentInChildren<Text> ().text = itemToCraft.tag + " | " + info;
		description	.GetComponentInChildren<Text> ().text = recipeItems[itemToCraft.name].description;

		requirements.transform.GetChild(0).GetComponent<CanvasGroup> ().alpha = 1;
		description.transform.GetComponent<CanvasGroup> ().alpha = 1;
	}
}

//continue to flesh out the key code pressing
