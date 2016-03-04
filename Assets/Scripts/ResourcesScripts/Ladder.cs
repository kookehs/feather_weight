using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour
{
	private InventoryController inventoryController;

	private void
        Awake ()
	{
		inventoryController = GameObject.Find ("Inventory").GetComponent<InventoryController> ();
	}

	public void
        SetLadder ()
	{
		GameObject[] cliff_points = GameObject.FindGameObjectsWithTag ("CliffPoint");

		GameObject closestObj = null;
		float min = float.MaxValue;

		foreach (GameObject obj in cliff_points) {
			if (obj.GetComponent<DistancePoints> ().isNearest < min) {
				min = obj.GetComponent<DistancePoints> ().isNearest;
				closestObj = obj;
			}
		}

		if (!closestObj.GetComponent<DistancePoints> ().pointUsed) {
			inventoryController.RemoveSetLadderObject (closestObj.transform);
			closestObj.GetComponent<DistancePoints> ().pointUsed = true;
		}
	}
}
