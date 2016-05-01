using UnityEngine;
using System.Collections;

public class Tree : Strikeable
{

	public bool containsNut;
	public bool containsBear;
	public bool hasFallen;
	public bool hasSprayedEmbers;
	public bool isSmitten;
	public bool isPlayerNear;
	public GameObject player;
	public GameObject nut;
	public GameObject wood;
	public GameObject stump;
	public GameObject mountainLion;

	public float fall_rate = 1000.0f;

	public bool checkMeForFall = false;
	public bool checkMeForBurn = false;

	private Vector3 drop_pos;
	private int totalTreeLogs = 3;

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
		containsBear = false;
		hasFallen = false;
		isSmitten = false;
	}

	// Update is called once per frame
	void Update ()
	{
		if (checkMeForFall == true && !hasFallen) {
			Fall ();
		}
		if (checkMeForBurn == true && !hasFallen)
			beginBurn ();
	}

	protected override bool AfterHit (string hitter)
	{
		DropCollectable (hitter);
		return false;
	}

	protected override void DropCollectable (string hitter)
	{
		if (containsBear) {
			DropHostile ("Bear");
			containsBear = false;
		} else {
			if (containsNut)
				DropNut ();
			else if (hitter.Contains ("Axe")) {
				if (totalTreeLogs-- == 0)
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

	public void DropHostile (string tag)
	{
		WorldContainer.Create (tag, new Vector3 (transform.position.x, transform.position.y, transform.position.z - 2), Quaternion.identity);
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
			//Debug.Log ("Tree ground trigger");
			//The following line leads to 'firecracker trees':
			//createEmbers ();
		}
	}


	//  This should be called when a tree is hit by an ember
	public void beginBurn ()
	{
		hasBurned = true;
		myFire.SetActive (true);
		StartCoroutine (WaitAndFall ());
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

	public IEnumerator WaitAndFall ()
	{
		yield return new WaitForSeconds (burnLength);
		if (!hasFallen) {
			createEmbers ();
			Fall ();
		}
		StartCoroutine (WaitAndEndBurn ());
	}

	public IEnumerator WaitAndEndBurn ()
	{
		yield return new WaitForSeconds (burnLength);
		myFire.SetActive (false);
	}

	//  This should be called when a burning tree hits the ground
	public void createEmbers ()
	{
		//  Create embers
		Vector3 emberSpawnPos = transform.FindChild ("EmberSpawnSpawnPos").transform.position;
		Instantiate (ember_spawn, emberSpawnPos, Quaternion.identity);
	}
}
