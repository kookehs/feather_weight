using UnityEngine;
using System.Collections;

public class Collection : MonoBehaviour {

	public InventoryController inventoryController;
	public float delay = 4.0f;

	private bool playerNearObject = false;
	private bool onMouseOver = false;

	void OnGUI(){
		//display the objects name when time has been reached
		if (onMouseOver) {
			GUI.Box (new Rect (Event.current.mousePosition.x - 55, Event.current.mousePosition.y, 50, 25), name);
		}

		//make sure name does not display when not hovering over object
		if (GetComponent<Renderer> ().sharedMaterial.GetFloat ("_Outline") == 0.0f)
			onMouseOver = false;
	}

	void OnMouseEnter()
	{
		GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.005f); //highlight the object
		StartCoroutine ("DisplayObjectNamt"); //delay before showing the object name
	}

	void OnMouseExit()
	{
		GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.0f); //remove highlight of object
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
