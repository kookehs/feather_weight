using UnityEngine;
using System.Collections;

public class GrabPlayerUIElements : MonoBehaviour {

	private GameObject inventoryUI;
	private GameObject chickenCurrency;
	private GameObject playerUICurrent;

	private Vector3 originalInventoryPos;

	// Use this for initialization
	void Start () {
		playerUICurrent = GameObject.Find ("PlayerUICurrent");
		inventoryUI = playerUICurrent.transform.FindChild ("InventoryContainer").gameObject;
		chickenCurrency = playerUICurrent.transform.FindChild ("ChickenCurrency").gameObject;

		playerUICurrent.transform.FindChild ("SurvivalHUD").gameObject.SetActive(false);

		originalInventoryPos = inventoryUI.GetComponent<RectTransform> ().localPosition;
		inventoryUI.transform.SetParent(transform.parent);
		inventoryUI.GetComponent<RectTransform> ().localPosition = transform.GetComponent<RectTransform> ().localPosition;
		inventoryUI.transform.SetParent(transform);

		//inventoryUI.GetComponent<RectTransform>().localPosition = new Vector3 (inventoryUI.GetComponent<RectTransform>().localPosition.x - inventoryUI.GetComponent<RectTransform>().localScale.x/2, 0, 0);

		chickenCurrency.transform.SetParent(transform);
	}
	
	// Update is called once per frame
	public void OnDestroy () {
		playerUICurrent.transform.FindChild ("SurvivalHUD").gameObject.SetActive(true);
		Debug.Log (chickenCurrency);
		chickenCurrency.transform.SetParent(playerUICurrent.transform);

		inventoryUI.transform.SetParent(playerUICurrent.transform);
		inventoryUI.GetComponent<RectTransform>().localPosition = originalInventoryPos;
	}
}
