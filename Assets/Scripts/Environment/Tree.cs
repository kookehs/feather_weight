using UnityEngine;
using System.Collections;

public class Tree : MonoBehaviour {

	public bool containsNut;
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
			Instantiate(nut, new Vector3(transform.position.x + 5, transform.position.y + 1, transform.position.z + 1), transform.rotation);
			containsNut = !containsNut;
		}
	}

	public void DropWood(){
		Instantiate(wood, new Vector3(transform.position.x + 5, transform.position.y + 1, transform.position.z + 1), transform.rotation);
	}

        public void
        Fall() {
            rb.isKinematic = false;
            Random.seed = System.Environment.TickCount;
            Vector3 direction = new Vector3(Random.Range(-180.0f, 180.0f), 0, Random.Range(-180.0f, 180.0f));
            rb.AddForce((transform.forward + direction).normalized * fall_rate);
        }
}
