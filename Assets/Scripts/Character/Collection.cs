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
		if(gameObject.tag != "river") defaultCol = GetComponentInChildren<SpriteRenderer> ().color;
        }

	void OnGUI(){
		//display the objects name when time has been reached
		if (onMouseOver) {
			GUI.Box (new Rect (Event.current.mousePosition.x - 55, Event.current.mousePosition.y, 50, 25), name);
		}

                if(Vector3.Distance(transform.position, player.transform.position) < 5f){
                        playerNearObject = true;
                }
                else{
                        playerNearObject = false;
                }
	}

	void OnMouseEnter()
	{
		if(gameObject.tag != "river"){
			GetComponentInChildren<SpriteRenderer> ().color = Color.red;
			player.mouseHovering = true;
			StartCoroutine ("DisplayObjectName"); //delay before showing the object name
		}
	}

	void OnMouseExit()
	{
		if (gameObject.tag != "river") {
			GetComponentInChildren<SpriteRenderer> ().color = defaultCol;
			player.mouseHovering = false;
			onMouseOver = false;
		}
	}

	void OnMouseDown(){
		if (playerNearObject && gameObject.tag != "river")
			inventoryController.AddNewObject (gameObject); //collect the object in inventory

		//collect some water first see if player has a water skin to add fill
		if(gameObject.tag == "river"){
			GameObject[] waterSkin = GameObject.FindGameObjectsWithTag ("waterskin");
			foreach (GameObject obj in waterSkin) {
				if (!obj.GetComponent<WaterSkin> ().waterFull) {
					obj.GetComponent<WaterSkin> ().Fill ();
					break;
				}
			}
		}
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
	IEnumerator DisplayObjectName(){
		yield return new WaitForSeconds(delay);
		onMouseOver = true;
	}
}
