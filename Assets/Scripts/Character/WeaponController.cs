using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{

	public GameObject player;
	public GameObject myWeapon;
	private Vector3 spawnPosFront;
	private Vector3 spawnPosBack;

	public AudioSource buzz;

	//	Stun and stun timer
	private bool coolingDown = false;
	private float cooldownTime;
	public float cooldownLength = 1f;

	Vector3 targetDirection = Vector3.zero;

	public Animator anim;

	//This is controlled by Collection.cs
	public bool hovering;

	public Camera mainCam;

	private GameObject spawnPosFrontG;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		//originalWeaponName = myWeapon.name;
		spawnPosFront = GameObject.Find ("SpawnPosFront").transform.position;
		spawnPosBack = GameObject.Find ("SpawnPosBack").transform.position;

		if (myWeapon != null) {
			myWeapon = Instantiate (myWeapon, spawnPosFront, Quaternion.identity) as GameObject;
			myWeapon.transform.parent = GameObject.Find ("SpawnPosFront").transform;
			myWeapon.name = "EquipedWeapon";
			myWeapon.layer = LayerMask.NameToLayer ("Default");
		}

		//	An AudioSource is declared here in code
		buzz = GetComponent<AudioSource> ();

		anim = GameObject.Find("PlayerSprite").GetComponent<Animator> ();
		mainCam = Camera.main;

		spawnPosFrontG = GameObject.Find ("SpawnPosFront");
	}

	// Update is called once per frame
	void Update ()
	{
		if (player == null || myWeapon == null)
			return;

		//***********************//
		// 	SPEARS AND PICKAXES  //
		//***********************//
		if (myWeapon.tag.Contains ("Spear") || myWeapon.tag.Contains ("Pick_Axe")) {
			if (Input.GetMouseButtonDown (0) && coolingDown == false && hovering == false) {
				anim.SetBool ("spear", true);
				myWeapon.SetActive (true);
				if (!myWeapon.GetComponentInChildren<SpriteRenderer> ().color.Equals (Color.white))
					myWeapon.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
				coolingDown = true;
				cooldownTime = Time.time;
			}

			//	End cooldown at the appropriate time
			if (coolingDown == true) {
				if (Time.time - cooldownTime >= .5f)
					coolingDown = false;
			}
			//
			//	The following code maintains the position of the SpawnPos object,
			//	which floats around the player at a fixed distance and at an angle
			//	that depends on where the mouse cursor is.
			//
			//	Declaration of ray, hit, and whereHit
			if (!coolingDown) {
				RaycastHit hit;
				Vector3 whereHit = Vector3.zero;
				Ray ray = mainCam.ScreenPointToRay (Input.mousePosition);

				//	Ray debug statement
				//Debug.DrawRay (ray.origin, ray.direction * 10, Color.yellow);

				//	A ray is cast from the mouse position. The y of the hit position
				//	is replaced with the y of the player position.
				if (Physics.Raycast (ray, out hit)) {
					whereHit = hit.point;
					whereHit.y = player.transform.position.y;
				}

				//	The rotation of the SpawnPos is determined based on the ray
				targetDirection = whereHit - player.transform.position;
				transform.rotation = Quaternion.LookRotation (targetDirection);
			}
			//****************************//
			// 	SWORDS, WOODAXES, HAMMER  //
			//****************************//
		} else if (myWeapon.tag.Contains ("Sword") || myWeapon.tag.Contains ("Wood_Axe") || myWeapon.tag.Contains ("Heaven")) {
			if (Input.GetMouseButtonDown (0) && coolingDown == false && hovering == false) {
				anim.SetBool ("sword", true);
				myWeapon.SetActive (true);
				if (!myWeapon.GetComponentInChildren<SpriteRenderer> ().color.Equals (Color.white))
					myWeapon.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
				coolingDown = true;
				cooldownTime = Time.time;
			}

			//Deal with cooldown
			if (coolingDown == true) {

				transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation (targetDirection), Time.deltaTime * 1000);
				if (Time.time - cooldownTime >= .5f)
					coolingDown = false;
			}
			//
			//	The following code maintains the position of the SpawnPos object,
			//	which floats around the player at a fixed distance and at an angle
			//	that depends on where the mouse cursor is.
			//
			//	Declaration of ray, hit, and whereHit
			if (!coolingDown) {
				RaycastHit hit;
				Vector3 whereHit = Vector3.zero;
				Ray ray = mainCam.ScreenPointToRay (Input.mousePosition);

				//	Ray debug statement
				//Debug.DrawRay (ray.origin, ray.direction * 10, Color.yellow);

				//	A ray is cast from the mouse position. The y of the hit position
				//	is replaced with the y of the player position.
				if (Physics.Raycast (ray, out hit)) {
					whereHit = hit.point;
					whereHit.y = player.transform.position.y;
				}

				//	The rotation of the SpawnPos is determined based on the ray.

				//  First, we get a vector from the player to the enemy
				Vector3 playerToEnemy = whereHit - player.transform.position;
				//  playerToEnemyRight will be perpendicular to that vector and Vector3.up. Aka "Right"
				Vector3 playerToEnemyRight = Vector3.Cross(playerToEnemy.normalized, Vector3.up);
				//  targetDirection lies between those previous two vectors. Aka "Front-right"
				targetDirection = (playerToEnemyRight + playerToEnemy).normalized;
				//  Now we set targetDirection so that it is between its previous value and playerToEnemyRight. Aka "Front-right-right"
				targetDirection = (targetDirection + playerToEnemyRight).normalized;
				// Debug.Log(targetDirection);
				//  Finally, we spawn the weapon at the "Left" side and it should swing around to the "Front-right-right" position
                                if (-playerToEnemyRight != Vector3.zero)
				    transform.rotation = Quaternion.LookRotation (-playerToEnemyRight);
			}
		}

	}

	public void equipWeapon (GameObject newWeapon)
	{
		newWeapon.transform.position = spawnPosFrontG.transform.position;
		newWeapon.gameObject.SetActive (true);
		newWeapon.transform.FindChild ("Trail").gameObject.SetActive (true);
		newWeapon.transform.parent = spawnPosFrontG.transform;
		newWeapon.name = "EquipedWeapon";
		newWeapon.layer = LayerMask.NameToLayer ("Default");
		newWeapon.GetComponent<Animator> ().enabled = true;

		myWeapon = newWeapon;
	}

	public void unequipWeapon (GameObject newWeapon)
	{
		newWeapon.transform.position = player.transform.position;
		newWeapon.gameObject.SetActive (false);
		newWeapon.transform.FindChild ("Trail").gameObject.SetActive (false);
		newWeapon.transform.parent = GameObject.Find ("PlayerItems").transform;
		newWeapon.name = myWeapon.tag;
		newWeapon.layer = LayerMask.NameToLayer ("Collectable");
		newWeapon.GetComponent<Animator> ().enabled = false;

		myWeapon = null;
	}

	public void playBuzzer ()
	{
		buzz.Play ();
	}

}
