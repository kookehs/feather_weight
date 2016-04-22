using UnityEngine;
using System.Collections;

public class Tree : Strikeable
{

	public bool containsNut;
	public bool hasFallen;
	public bool isSmitten;
	public bool isPlayerNear;
	public GameObject player;
	public GameObject nut;
	public GameObject wood;
	public GameObject stump;
	public GameObject mountainLion;

	public float fall_rate = 1000.0f;

	public bool checkMeForFall = false;

	private Vector3 drop_pos;
	private int totalTreeLogs = 5;

	//  Fire things
	public GameObject ember_spawn;
	public GameObject myFire;
	public bool hasBurned = false;
	public float burnTime;
	public float burnLength = 5f;

	private void Awake ()
	{
		rb = GetComponent<Rigidbody> ();
		rb.isKinematic = true;

		drop_pos = new Vector3 (transform.position.x, transform.position.y + 10, transform.position.z - 1);
	}

	// Use this for initialization
	void Start ()
	{
		player = GameObject.Find ("Player");
		containsNut = true;
		hasFallen = false;
		isSmitten = false;
		// myFire = transform.Find ("Fire").gameObject;
	}

	// Update is called once per frame
	void Update ()
	{
		if (checkMeForFall == true) {
			Fall ();
		}
		//  When the burn timer has run out...
		if (Time.time - burnTime > burnLength && hasBurned == true) {
			// If the tree hasn't fallen, then it falls and the burn timer resets.
			//  Now the tree gets to burn on the ground for another 'burnLength' seconds
			if (!hasFallen) {
				Fall ();
				burnTime = Time.time;
			}
            //  If it has already fallen, end the burn
            else
				endBurn ();
		}
	}

	protected override bool AfterHit (string hitter)
	{
                DropCollectable (hitter);
		return false;
	}

	protected override void DropCollectable (string hitter)
	{
		if (WorldContainer.RandomChance () < -1) {
			DropLion ();
		} else {
			if (containsNut)
				DropNut ();
			else if (hitter.Contains("Axe")) {
				if (totalTreeLogs > 0)
					DropWood ();
				else if (totalTreeLogs <= 0)
					KillTree ();
			}
		}
	}

	// Drop nuts on the ground
	public void DropNut ()
	{
		WorldContainer.Create (nut.transform, drop_pos);
		WorldContainer.UpdateList ("Nut");
		containsNut = !containsNut;
	}

	public void DropWood ()
	{
		totalTreeLogs--;
		WorldContainer.Create (wood.transform, drop_pos);
	}

	public void DropLion ()
	{
		WorldContainer.Create (mountainLion.transform, drop_pos);
	}

	public void KillTree ()
	{
		GameObject stumpObj = Instantiate (stump, transform.position, transform.rotation) as GameObject;
		stumpObj.transform.parent = transform.parent;
		Destroy (gameObject);
	}

	public void Fall ()
	{
		if (!hasFallen) {
			rb.isKinematic = false;
			Transform player = GameObject.Find ("Player").GetComponent<Transform> ();
			Vector3 direction = Vector3.MoveTowards (transform.position, player.position, 1.0f);
			direction.y = 0.0f;
			rb.AddForce ((player.position - direction) * fall_rate);
			hasFallen = true;
		}
	}

	public void GetSmitten ()
	{
		if (!isSmitten) {
			isSmitten = true;
			containsNut = false;
		}
	}

	public void OnTriggerEnter (Collider other)
	{
		// Non-burning tree hits campfire
		if (other.tag.Equals ("CampFire")) {
			if (hasBurned == false) {
				beginBurn (other);
			}
		}
		//  Non-burning tree hits ember
		if (other.tag.Equals ("Ember")) {
			beginBurn ();
		}
		//  Already burning tree hits ground
		if (other.tag.Equals ("Ground") && hasBurned) {
			createEmbers ();
		}
	}

	//  This should be called when a tree is hit by an ember
	public void beginBurn ()
	{
		hasBurned = true;
		burnTime = Time.time;
		myFire.SetActive (true);
	}

	//  This should be called when a tree hits a campfire
	public void beginBurn (Collider other)
	{
		//  Create embers
		Vector3 emberSpawnPos = new Vector3 (other.transform.position.x, 6, other.transform.position.z);
		Instantiate (ember_spawn, emberSpawnPos, Quaternion.identity);

		hasBurned = true;
		burnTime = Time.time;

		//  Activate visual fire
		myFire.SetActive (true);

	}

	//  This should be called when a burning tree hits the ground
	public void createEmbers ()
	{
		//  Create embers
		Vector3 emberSpawnPos = transform.FindChild ("EmberSpawnSpawnPos").transform.position;
		Instantiate (ember_spawn, emberSpawnPos, Quaternion.identity);
	}

	public void endBurn ()
	{
		myFire.SetActive (false);
	}
}
