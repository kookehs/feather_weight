using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {

	public bool containsNut;
	public bool isPlayerNear;
	public GameObject player;
	public GameObject nut;
	public GameObject wood;

	public PlayerNearTree playerNearTreeScript;

        private Rigidbody rb;

        private void
        Awake() {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
        }

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		ScenarioController sc = GameObject.Find ("ScenarioController").GetComponent<ScenarioController> ();
		object container;
		sc.GetScenario ("PlayerNearTree", out container);
		playerNearTreeScript = (PlayerNearTree) container;
		containsNut = true;
	}

	// Update is called once per frame
	void Update () {
            if (Input.GetKeyDown("b")) {
                Timber();
            }
	}

	void OnTriggerEnter (Collider other) {
		// Debug.Log ("Trigger");
		if (other.gameObject.Equals (player)) {
			isPlayerNear = true;
			playerNearTreeScript.addTree (gameObject);
		}
	}

	void OnTriggerExit (Collider other) {
		if (other.gameObject.Equals (player)) {
			isPlayerNear = false;
			playerNearTreeScript.removeTree (gameObject);
		}
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
			Instantiate(nut, new Vector3(transform.position.x + 5, transform.position.y + 1, transform.position.z + 1), transform.rotation);
			containsNut = !containsNut;
		}
	}

	public void DropWood(){
		Instantiate(wood, new Vector3(transform.position.x + 5, transform.position.y + 1, transform.position.z + 1), transform.rotation);
	}

        private void
        Timber() {
           // TODO(bill): Randomize fall direction
           rb.isKinematic = false;
           rb.AddForce(transform.right * 1000.0f);
        }
}
