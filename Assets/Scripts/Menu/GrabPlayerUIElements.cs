using UnityEngine;
using System.Collections;

public class GrabPlayerUIElements : MonoBehaviour {

	private GameObject inventoryUI;
	private GameObject chickenCurrency;
	private Transform playerUICurrent;

	private Vector3 originalInventoryPos;

	// Use this for initialization
	void Start () {
		inventoryUI = GameObject.Find ("InventoryContainer");
		chickenCurrency = GameObject.Find ("ChickenCurrency");

		playerUICurrent = inventoryUI.transform.parent;
		playerUICurrent.FindChild ("SurvivalHUD").gameObject.SetActive(false);

		originalInventoryPos = inventoryUI.GetComponent<RectTransform> ().localPosition;
		inventoryUI.transform.SetParent(transform);
		inventoryUI.GetComponent<RectTransform>().localPosition = new Vector3 (0, 0, 0);

		chickenCurrency.transform.SetParent(transform);
	}
	
	// Update is called once per frame
	public void OnDestroy () {
		playerUICurrent.FindChild ("SurvivalHUD").gameObject.SetActive(true);
	}
}
