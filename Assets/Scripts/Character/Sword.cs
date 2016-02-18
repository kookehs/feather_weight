using UnityEngine;
using System.Collections;

public class Sword : MonoBehaviour {

	WorldContainer the_world;

	// Use this for initialization
	void Start () {
		the_world = GameObject.Find ("WorldContainer").GetComponent<WorldContainer> ();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter (Collider other) {
		//Debug.Log ("Weapon Colliding");
		bool killed = false;

        switch (other.tag) {
		case "Bear":
			if (other.gameObject.GetComponent<Animal> () != null) {
				killed = other.gameObject.GetComponent<Animal> ().receiveHit (GetComponent<Collider> (), 10, 1000);
			} else if (other.gameObject.GetComponent<BearRB> () != null) {
				killed = other.gameObject.GetComponent<BearRB> ().receiveHit (GetComponent<Collider> (), 10, 1000);
			}
                        break;
                case "Tree":
                        other.gameObject.GetComponent<Tree>().receiveHit();
                        break;
                case "Rock3D":
                        other.gameObject.GetComponent<Rock>().receiveHit();
                        break;
                default:
                        break;
        }

		if (killed) {
			the_world.UpdateKillCount (other.tag);
		}
	}

	void OnEnable(){
		GetComponent<Animator> ().Play ("sword_swing");
	}


	void disableMe(){
		if(gameObject.layer.Equals(0))
			gameObject.SetActive (false);
	}
}
