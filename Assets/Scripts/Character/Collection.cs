using UnityEngine;
using System.Collections;

public class Collection : MonoBehaviour {

	public float delay = 1.0f;
	public Color defaultCol;
	public bool playerNearRiver = false;

	private bool playerNearObject = false;
	private bool onMouseOver = false;

	private PlayerMovementRB player;
	private InventoryController inventoryController;

	void Start(){
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovementRB> ();
		inventoryController = GameObject.Find ("Inventory").GetComponent<InventoryController>();
		if(gameObject.tag != "River") defaultCol = GetComponentInChildren<SpriteRenderer> ().color;
	}

	void OnGUI(){
		//display the objects name when time has been reached
		if (onMouseOver) {
			GUI.Box (new Rect (Event.current.mousePosition.x - 55, Event.current.mousePosition.y, 50, 25), name);
		}
	}

	void OnMouseEnter()
	{
		if(gameObject.tag != "River"){
			GetComponentInChildren<SpriteRenderer> ().color = Color.red;
			player.mouseHovering = true;
			StartCoroutine ("DisplayObjectName"); //delay before showing the object name
		}
	}

	void OnMouseExit()
	{
		if (gameObject.tag != "River") {
			GetComponentInChildren<SpriteRenderer> ().color = defaultCol;
			player.mouseHovering = false;
			onMouseOver = false;
		}
	}

	void OnMouseDown(){
		if (playerNearObject && gameObject.tag != "River") 
			inventoryController.AddNewObject (gameObject); //collect the object in inventory

		//collect some water first see if player has a water skin to add fill
		if(gameObject.tag == "River"){
			GameObject[] waterSkin = GameObject.FindGameObjectsWithTag ("waterskin");
			foreach (GameObject obj in waterSkin) {
				if (!obj.GetComponent<WaterSkin> ().waterFull) {
					obj.GetComponent<WaterSkin> ().Fill ();
					break;
				}
			}
		}
	}

	//to delay display of the object name
	IEnumerator DisplayObjectName(){
		yield return new WaitForSeconds(delay);
		onMouseOver = true;
	}
}
