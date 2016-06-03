using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

	public float health = 100;
	public float maxHealth = 100;

	public bool hungry = false;
	public bool thirsty = false;
	public float malnutritionLossInterval = 5;
	public float malnutritionTimer;

	private Animator anim;
	private Transform playerHeart;
	private bool flashNow = true;

	// Use this for initialization
	void Awake ()
	{
		anim = gameObject.GetComponentInChildren<Animator> ();
		if (anim == null)
			anim = GetComponent<Animator> ();
		malnutritionTimer = malnutritionLossInterval;
		if (gameObject.tag.Equals("Player"))
			playerHeart = GameObject.Find ("PlayerUICurrent").transform.FindChild ("SurvivalHUD").FindChild ("SurvivalGUI").GetChild(0);
	}

	// Update is called once per frame
	void Update ()
	{
		/*if (hungry || thirsty) {
			malnutritionTimer -= Time.deltaTime;
			if (malnutritionTimer <= 0) {
				Decrease (10f);
				malnutritionTimer = malnutritionLossInterval;
			}
		}*/

		if (gameObject.tag.Equals("Player") && health < 30 && health > 10 && flashNow) {
			StartCoroutine ("Flash", 1f);
		}

		if (gameObject.tag.Equals("Player") && health <= 10 && flashNow) {
			StartCoroutine ("Flash", 0.5f);
		}

		if (health <= 0) {
			if (gameObject.tag.Equals ("Bear")) {
				//Debug.Log ("BEAR CODE");
				GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
				StartCoroutine (WaitAndDestroy ());
			} else if (anim != null) {
				anim.SetBool ("isDead", true);
				AnimatorStateInfo current_state = anim.GetCurrentAnimatorStateInfo (0);

				if (current_state.nameHash == Animator.StringToHash ("Base Layer.death")) {
					if (gameObject.tag.Equals ("Player"))
						StartCoroutine ("GameOver", current_state.length);
					//Destroy (gameObject, current_state.length);
				}
			} else if (gameObject.tag.Equals("Player")){
				/*if (gameObject.tag.Equals ("Boss")) {
					DeathCode ();
				}*/
				DeathCode ();
				//Destroy (gameObject);
				//gameObject.SetActive(false);
			}
		}

	}

	public IEnumerator WaitAndDestroy() {
		yield return new WaitForSeconds (2f);
		DeathCode ();
		//Destroy (gameObject);
	}

	private IEnumerator Flash(int time){
		flashNow = false;
		yield return new WaitForSeconds(time);
		flashNow = true;
		playerHeart.parent.gameObject.SetActive (!playerHeart.transform.parent.gameObject.activeSelf);
	}

	public void Increase ()
	{
		health = Mathf.Min (maxHealth, health + 10f);
	}

	public void Increase (float d)
	{
		health = Mathf.Min (maxHealth, health + d);
	}

	public void Decrease ()
	{
		health = Mathf.Max (0f, health - 10f);
	}

	public void Decrease (float d)
	{
		health = Mathf.Max (0f, health - d);
	}

	public bool IsDead ()
	{
		return health <= 0;
	}

	IEnumerator GameOver (int duration)
	{
		yield return new WaitForSeconds (duration);
		DeathCode ();
	}

	private void DeathCode(){
		try{
			GameObject[] allobs = FindObjectsOfType<GameObject>() as GameObject[];
			foreach (GameObject o in allobs) {
				if(o != gameObject && !o.name.Equals("TwitchData"))
					Destroy(o);
			}
		}catch(Exception e){
			Debug.Log ("No EventSystem" + e.Message);
		}

		Application.LoadLevel("HexLayoutChickenroom");
	}
}
