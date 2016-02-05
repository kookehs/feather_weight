using UnityEngine;
using System.Collections;

public class Collection : MonoBehaviour {

	public InventoryController inventoryController;
	public float delay = 20.0f;

	private bool playerNearObject = false;
	private bool onMouseOver = false;

	void OnGUI(){
		if (onMouseOver) {
			GUI.Box (new Rect (Event.current.mousePosition.x - 55, Event.current.mousePosition.y, 50, 25), name);
		}
	}

	void OnMouseEnter()
	{
		GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.005f);
		delay = 20.0f;
		StartCoroutine ("DisplayObjectNamt");
	}

	void OnMouseExit()
	{
		GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.0f);
		delay = 0.0f;
		onMouseOver = false;
	}

	void OnMouseDown(){
		if(playerNearObject) inventoryController.AddNewObject (gameObject);
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
