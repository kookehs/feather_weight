using UnityEngine;
using System.Collections;

public class Collection : MonoBehaviour {

	public float delay = 1.0f;
	public Color defaultCol;

	private bool playerNearObject = false;
	private bool onMouseOver = false;

	private InventoryController inventoryController;

	void Start(){
		inventoryController = GameObject.Find ("Inventory").GetComponent<InventoryController>();
		defaultCol = GetComponentInChildren<SpriteRenderer> ().color;
	}

	void OnGUI(){
		//display the objects name when time has been reached
		if (onMouseOver) {
			GUI.Box (new Rect (Event.current.mousePosition.x - 55, Event.current.mousePosition.y, 50, 25), name);
		}

		//make sure name does not display when not hovering over object
		if(GetComponentInChildren<SpriteRenderer> ().color != Color.red)
			onMouseOver = false;
	}

	void OnMouseEnter()
	{
		GetComponentInChildren<SpriteRenderer> ().color = Color.red;
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = true;
		StartCoroutine ("DisplayObjectNamt"); //delay before showing the object name
	}

	void OnMouseExit()
	{
		GetComponentInChildren<SpriteRenderer> ().color = defaultCol;
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovementRB>().mouseHovering = false;
		onMouseOver = false;
	}

	void OnMouseDown(){
		if(playerNearObject) inventoryController.AddNewObject (gameObject); //collect the object in inventory
	}

	void OnTriggerEnter(Collider obj){
		if(obj.tag == "Player")
			playerNearObject = true;
	}

	void OnTriggerExit(Collider obj){
		if (obj.tag == "PLayer")
			playerNearObject = false;
	}

	//to delay display of the object name
	IEnumerator DisplayObjectNamt(){
		yield return new WaitForSeconds(delay);
		onMouseOver = true;
	}
}
