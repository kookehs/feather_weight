using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HoverOverInventory : MonoBehaviour {

	public float delay = 0.1f;
	public Color defaultCol;

	private bool playerNearObject = false;
	private bool onMouseOver = false;

	private InventoryController inventoryController;

	void Start(){
		if (GameObject.FindGameObjectWithTag("InventoryUI") != null)
			inventoryController = GameObject.FindGameObjectWithTag ("InventoryUI").GetComponent<InventoryController>();
	}

	void OnMouseEnter()
	{
		enabled = true;
		StartCoroutine ("DisplayObjectName"); //delay before showing the object name
	}

	void OnMouseExit()
	{
		onMouseOver = false;
		enabled = false;
		inventoryController.ShowItemInfo (-1);
	}

	//to delay display of the object name
	IEnumerator DisplayObjectName(){
		yield return new WaitForSeconds(delay);
		onMouseOver = true;
		int s = 0;
		if(int.TryParse(transform.GetComponentInChildren<Text>().text.ToString(), out s))
			inventoryController.ShowItemInfo (s);
	}
}
