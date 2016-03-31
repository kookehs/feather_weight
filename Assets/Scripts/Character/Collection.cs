using UnityEngine;
using System.Collections;

public class Collection : MonoBehaviour {

	public float delay = 0.1f;
	public Color defaultCol;

	private bool playerNearObject = false;
	private bool onMouseOver = false;

	private PlayerMovementRB player;
	private InventoryController inventoryController;

	void Start(){
        if (GameObject.FindGameObjectWithTag("Player"))
        player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovementRB> ();

        if (GameObject.FindGameObjectWithTag("InventoryUI") != null)
                inventoryController = GameObject.FindGameObjectWithTag ("InventoryUI").GetComponent<InventoryController>();

		if (gameObject.tag != "River") {
			if (GetComponentInChildren<SpriteRenderer> () != null)
				defaultCol = GetComponentInChildren<SpriteRenderer> ().color;
			else
				defaultCol = GetComponent<Renderer> ().material.color;
		}
	}

	void OnGUI(){
		if (player == null) return;
		//display the objects name when time has been reached
		string regex_name = name.Split (new string[] {"("," "}, System.StringSplitOptions.RemoveEmptyEntries)[0];
		if (onMouseOver) {
			GUI.Box (new Rect (Event.current.mousePosition.x - 55, Event.current.mousePosition.y, 50, 25), regex_name);
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
		enabled = true;
		if(gameObject.tag != "River"){
			if (GetComponentInChildren<SpriteRenderer> () != null)
				GetComponentInChildren<SpriteRenderer> ().color = Color.white;
			else
				GetComponent<Renderer> ().material.color = Color.red;//GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.005f);
			player.mouseHovering = true;
			StartCoroutine ("DisplayObjectName"); //delay before showing the object name
		}
	}

	void OnMouseExit()
	{
		if (gameObject.tag != "River") {
			if (GetComponentInChildren<SpriteRenderer> () != null)
				GetComponentInChildren<SpriteRenderer> ().color = defaultCol;
			else
				GetComponent<Renderer> ().material.color = defaultCol;//GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.0f);
			player.mouseHovering = false;
			onMouseOver = false;
			enabled = false;
		}
	}

	void OnMouseDown(){
		if (playerNearObject && gameObject.tag != "River")
			inventoryController.AddNewObject (gameObject); //collect the object in inventory

		//collect some water first see if player has a water skin to add fill
		if(gameObject.tag == "River"){
			GameObject[] waterSkin = GameObject.FindGameObjectsWithTag ("WaterSkin");
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