using UnityEngine;
using System.Collections;

public enum BearState {
	UNAWARE,
	HOSTILE,
	FRIENDLY
}

[RequireComponent (typeof (CharacterController))]
public class Bear : MonoBehaviour {

	private BearState state = BearState.UNAWARE;

	public GameObject player;
	public GameObject scenarioController;
	public bool isPlayerNear;

	// Use this for initialization
	void Start () {

		player = GameObject.Find ("Player");
		isPlayerNear = false;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.Equals (player)) {
			isPlayerNear = true;
		}
	}
}
