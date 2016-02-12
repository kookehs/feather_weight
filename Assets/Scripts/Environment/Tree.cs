using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {

	public bool containsNut;
	public bool hasFallen;
	public bool isSmitten;
	public bool isPlayerNear;
	public GameObject player;
	public GameObject nut;
	public GameObject wood;

        private Rigidbody rb;
        public float fall_rate = 1000.0f;

        private void
        Awake() {
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

	void OnTriggerEnter (Collider other) {
		// Debug.Log ("Trigger");
	}

	void OnTriggerExit (Collider other) {
	}

	void OnCollisionEnter(Collision obj){
		if (obj.collider.tag.Equals ("sword")) {
			DropNut ();
			if(!containsNut) DropWood ();
		}
	}

	// Drop nuts on the ground
	public void DropNut () {
		if (containsNut) {
			Instantiate(nut, new Vector3(player.transform.position.x + 5, player.transform.position.y + 10, player.transform.position.z + 1), player.transform.rotation);
			containsNut = !containsNut;
		}
	}

	public void DropWood(){
		Instantiate(wood, new Vector3(player.transform.position.x + 5, player.transform.position.y + 10, player.transform.position.z + 1), player.transform.rotation);
	}

        public void
        Fall() {
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
