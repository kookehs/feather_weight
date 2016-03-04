using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{

	public GameObject player;
	public GameObject myWeapon;
	private Vector3 spawnPos;

	//public string originalWeaponName = "";

	//	Stun and stun timer
	private bool coolingDown = false;
	private float cooldownTime;
	public float cooldownLength = 1f;

	Vector3 targetDirection = Vector3.zero;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		//originalWeaponName = myWeapon.name;
		spawnPos = GameObject.Find ("SpawnPos").transform.position;

		//	The equipped weapon is instantiated at the spawn point, and then made a child of this object.
		myWeapon = Instantiate (myWeapon, spawnPos, Quaternion.identity) as GameObject;
		myWeapon.transform.parent = GameObject.Find ("SpawnPos").transform;
		myWeapon.name = "EquipedWeapon";
		myWeapon.layer = LayerMask.NameToLayer ("Default");
	}

	// Update is called once per frame
	void Update ()
	{

		if (myWeapon.tag.StartsWith ("Spear") || myWeapon.tag.StartsWith("Pickaxe")) {
			if (Input.GetMouseButtonDown (0) && coolingDown == false && !player.GetComponent<PlayerMovementRB> ().mouseHovering) {
				player.GetComponent<Animator> ().SetTrigger ("spear");
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

			//	
			//	The following code maintains the position of the SpawnPos object,
			//	which floats around the player at a fixed distance and at an angle
			//	that depends on where the mouse cursor is.
			//
			//	Declaration of ray, hit, and whereHit
			if (!coolingDown) {
				RaycastHit hit;
				Vector3 whereHit = Vector3.zero;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				//	Ray debug statement
				Debug.DrawRay (ray.origin, ray.direction * 10, Color.yellow);

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
		} else if (myWeapon.tag.StartsWith ("Sword")) {
			if (Input.GetMouseButtonDown (0) && coolingDown == false && !player.GetComponent<PlayerMovementRB> ().mouseHovering) {
				player.GetComponent<Animator> ().SetTrigger ("sword");
				myWeapon.SetActive (true);
				if (!myWeapon.GetComponentInChildren<SpriteRenderer> ().color.Equals (Color.white))
					myWeapon.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
				coolingDown = true;
				cooldownTime = Time.time;
			}

			//Deal with cooldown
			if (coolingDown == true) {

				transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * 1000);
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
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

				//	Ray debug statement
				Debug.DrawRay (ray.origin, ray.direction * 10, Color.yellow);

				//	A ray is cast from the mouse position. The y of the hit position
				//	is replaced with the y of the player position.
				if (Physics.Raycast (ray, out hit)) {
					whereHit = hit.point;
					whereHit.y = player.transform.position.y;
				}

				//	The rotation of the SpawnPos is determined based on the ray
				targetDirection = whereHit - player.transform.position;

				//	Difference between sword and spear:
				//	This code tells us the sword will spawn in the opposite
				//	direction of the player's mouse
				transform.rotation = Quaternion.LookRotation (-targetDirection);
			}
		}
	}


}
