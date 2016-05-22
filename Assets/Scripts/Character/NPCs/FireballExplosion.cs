using UnityEngine;
using System.Collections;

public class FireballExplosion : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag ("Tree")) {
			other.GetComponent<Tree> ().GetSmitten ();
		} else {
			if (other.name.Equals("Torchick") || other.CompareTag("Chicken")) return;
			Strikeable s = other.GetComponent<Strikeable> ();
			if (s == null) return;
			s.maxforce = 30f;
			s.receiveHit (GetComponent<Collider> (), 15, 2000, "Fireball");
			s.maxforce = 21f;
		}
	}
}

