using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

	public GameObject mainChar;
	public GameObject myWeapon;
	private Vector3 spawnPos;

	public string originalWeaponName = "";

	//	Stun and stun timer
	private bool coolingDown = false;
	private float cooldownTime;
	public float cooldownLength = 1f;

	// Use this for initialization
	void Start () {
		mainChar = GameObject.FindGameObjectWithTag ("Player");
		originalWeaponName = myWeapon.name;
		spawnPos = GameObject.Find ("SpawnPos").transform.position;
		myWeapon = Instantiate (myWeapon, spawnPos, Quaternion.identity) as GameObject;
		myWeapon.transform.parent = gameObject.transform;
		myWeapon.name = "EquipedWeapon";
		myWeapon.layer = LayerMask.NameToLayer ("Default");
	}

	// Update is called once per frame
	void Update () {

		// Debug.Log (mainChar.GetComponent<PlayerMovementRB> ());

		//	When I click, spawn the sword at the position of the swordSpawn
		// Debug.Log(mainChar.GetComponent<PlayerMovementRB>());
		if (Input.GetMouseButtonDown (0) && coolingDown == false && !mainChar.GetComponent<PlayerMovementRB>().mouseHovering) {
			myWeapon.SetActive (true);
			if (!myWeapon.GetComponentInChildren<SpriteRenderer> ().color.Equals (Color.white))
				myWeapon.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
			coolingDown = true;
			cooldownTime = Time.time;
		}

		//Deal with cooldown
		if (coolingDown == true) {
			if (Time.time - cooldownTime >= .5f)
				coolingDown = false;
		}

		//	Maintain the position of the swordSpawn
		RaycastHit hit;
		Vector3 whereHit = Vector3.zero;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
		if (Physics.Raycast (ray, out hit)) {
			whereHit = hit.point;
			whereHit.y = mainChar.transform.position.y;
		}
		Vector3 targetDirection = whereHit - mainChar.transform.position;
		transform.rotation = Quaternion.LookRotation (targetDirection);
	}
}
