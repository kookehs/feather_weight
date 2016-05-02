using UnityEngine;
using System.Collections;

public class HoverAttack : MonoBehaviour {

	private Camera camera;

	public void Start(){
		camera = Camera.main;
	}

	// Use this for initialization
	void OnMouseEnter () {
		camera.GetComponent<CollectionCursor> ().SetWeapon ();
	}

	void OnMouseExit () {
		camera.GetComponent<CollectionCursor> ().SetDefault ();
	}
}
