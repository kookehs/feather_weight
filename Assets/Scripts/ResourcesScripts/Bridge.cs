using UnityEngine;
using System.Collections;

public class Bridge : MonoBehaviour {

	private InventoryController inventoryController;

	// Use this for initialization
	void Awake () {
		inventoryController = GameObject.Find ("Inventory").GetComponent<InventoryController>();
	}
	
	public void SetBridge(){
		GameObject[] riverPoint = GameObject.FindGameObjectsWithTag ("riverpoint");

		//find the river point closest
		GameObject closestObj = null;
		float min = float.MaxValue;
		foreach (GameObject obj in riverPoint) {
			if (obj.GetComponent<DistancePoints> ().isNearest < min) {
				min = obj.GetComponent<DistancePoints> ().isNearest;
				closestObj = obj;
			}
		}

		if (!closestObj.GetComponent<DistancePoints> ().pointUsed) {
			inventoryController.RemoveSetBridgeObject (closestObj.transform);
			closestObj.GetComponent<DistancePoints> ().pointUsed = true;
		}
	}
}
