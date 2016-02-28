using UnityEngine;
using System.Collections;

public class Tree : Strikeable {
	
	public bool containsNut;
	public bool hasFallen;
	public bool isSmitten;
	public bool isPlayerNear;
	public GameObject player;
	public GameObject nut;
	public GameObject wood;
	public GameObject stump;

	public float fall_rate = 1000.0f;

	private int totalTreeLogs = 5;

	private void Awake() {
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;
	}

	// Use this for initialization
	void Start () {
		containsNut = true;
		hasFallen = false;
		isSmitten = false;
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("b")) {
			Fall();
		}
	}

	protected void DropCollectable() {
		DropNut ();
		if (!containsNut && totalTreeLogs > 0)
			DropWood ();
		else if (totalTreeLogs <= 0)
			KillTree ();
	}

	// Drop nuts on the ground
	public void DropNut () {
		if (containsNut) {
			Instantiate(nut, new Vector3(player.transform.position.x + 5, player.transform.position.y + 10, player.transform.position.z + 1), player.transform.rotation);
			containsNut = !containsNut;
		}
	}

	public void DropWood(){
		totalTreeLogs--;
		Instantiate(wood, new Vector3(player.transform.position.x + 5, player.transform.position.y + 10, player.transform.position.z + 1), player.transform.rotation);
	}

	public void KillTree(){
		GameObject stumpObj = Instantiate (stump, transform.position, transform.rotation) as GameObject;
		stumpObj.transform.parent = transform.parent;
		Destroy (gameObject);
	}

	public void Fall() {
		if (!hasFallen) {
			rb.isKinematic = false;
			Transform player = GameObject.Find ("Player").GetComponent<Transform> ();
			Vector3 direction = Vector3.MoveTowards (transform.position, player.position, 1.0f);
			direction.y = 0.0f;
			rb.AddForce ((player.position - direction) * fall_rate);
			hasFallen = true;
			Debug.Log("TIMBER!");
		}
	}

	public void GetSmitten() {
		if (!isSmitten) {
			isSmitten = true;
			containsNut = false;
		}
	}
}
