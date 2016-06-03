using UnityEngine;
using System.Collections;

public class GrabPlayerUIElements : MonoBehaviour {

	private GameObject inventoryUI;
	private GameObject chickenCurrency;
	private GameObject timer;
	private GameObject banner;
	private GameObject playerUICurrent;
	private GameObject sellPopup;
	private GameObject twitchChat;
	private GameObject survivalHUD;
	private GameObject pauseMenu;
	private GameObject player;

	private Vector3 originalInventoryPos;
	private Vector3 originalCurrencyPos;
	private Vector3 originalHealthPos;

	// Use this for initialization
	void Start () {
		playerUICurrent = GameObject.Find ("PlayerUICurrent");
		player = GameObject.FindGameObjectWithTag ("Player");

		if (playerUICurrent == null)
			return;
		
		inventoryUI = playerUICurrent.transform.FindChild ("InventoryContainer").gameObject;
		chickenCurrency = playerUICurrent.transform.FindChild ("ChickenCurrency").gameObject;
		timer = playerUICurrent.transform.FindChild ("TimeLimitHUD").gameObject;
		banner = playerUICurrent.transform.FindChild ("TwitchActionPopUp").gameObject;
		twitchChat = playerUICurrent.transform.FindChild ("ChatHUD").gameObject;
		survivalHUD = playerUICurrent.transform.FindChild ("SurvivalHUD").gameObject;
		pauseMenu = playerUICurrent.transform.FindChild ("PauseMenu").gameObject;
		sellPopup = inventoryUI.transform.FindChild ("ConfirmSell").gameObject;
		sellPopup.transform.SetParent(timer.transform);
		//also grab the banner

		originalInventoryPos = inventoryUI.GetComponent<RectTransform> ().localPosition;
		inventoryUI.transform.SetParent(transform.parent);
		inventoryUI.GetComponent<RectTransform> ().localPosition = transform.GetComponent<RectTransform> ().localPosition;
		inventoryUI.transform.SetParent(transform);

		//move over chicken currency
		originalHealthPos = survivalHUD.transform.localPosition;
		Transform healthShop = transform.FindChild ("HealthShop");

		//survivalHUD.transform.SetParent(transform.parent);

		survivalHUD.transform.SetParent(healthShop);
		survivalHUD.transform.localPosition = new Vector3 (healthShop.GetComponent<RectTransform> ().rect.width/2 - 5, healthShop.GetComponent<RectTransform> ().rect.height/2 + 10, 0);

		//move over health
		originalCurrencyPos = chickenCurrency.transform.localPosition;
		chickenCurrency.transform.SetParent(transform);
		chickenCurrency.transform.localPosition = new Vector3 (transform.localScale.x + chickenCurrency.transform.localScale.x, chickenCurrency.transform.localPosition.y, 0);

		timer.transform.SetParent(transform);
		banner.transform.SetParent(transform);
		pauseMenu.transform.SetParent(transform);

		//bring forth the twitch chatHUD
		Transform twitchShop = transform.parent.parent.FindChild("TwitchShop");
		twitchChat.transform.localPosition = new Vector3 (0, twitchChat.transform.localPosition.y, 0);
		twitchChat.transform.SetParent(twitchShop);
	}
	
	// Update is called once per frame
	public void RestPlayerUI () {
		chickenCurrency.transform.SetParent(playerUICurrent.transform);
		chickenCurrency.transform.localPosition = originalCurrencyPos;

		survivalHUD.transform.SetParent(playerUICurrent.transform);
		survivalHUD.transform.localPosition = originalHealthPos;

		timer.transform.SetParent(playerUICurrent.transform);
		banner.transform.SetParent(playerUICurrent.transform);
		pauseMenu.transform.SetParent(playerUICurrent.transform);

		twitchChat.transform.SetParent(playerUICurrent.transform);
		twitchChat.transform.position = new Vector3 (0, twitchChat.transform.position.y, 0);

		inventoryUI.transform.SetParent(playerUICurrent.transform);
		inventoryUI.GetComponent<RectTransform>().localPosition = originalInventoryPos;

		sellPopup.transform.SetParent(inventoryUI.transform);
		sellPopup.GetComponent<CanvasGroup> ().alpha = 0;
		sellPopup.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		sellPopup.GetComponent<CanvasGroup> ().interactable = false;
	}

	public void PurchaseHealth(){
		if (chickenCurrency.GetComponentInChildren<Currency> ().currency > 3 && player.GetComponent<Health> ().health < player.GetComponent<Health> ().maxHealth) {
			player.GetComponent<Health> ().Increase ();
			chickenCurrency.GetComponentInChildren<Currency> ().currency -= 3;
		}
	}
}
