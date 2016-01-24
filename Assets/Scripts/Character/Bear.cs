using UnityEngine;
using System.Collections;

public enum BearState
{
	UNAWARE,
	HOSTILE,
	FRIENDLY
}

[RequireComponent (typeof(CharacterController))]
public class Bear : MonoBehaviour
{

	private BearState state = BearState.UNAWARE;
	public GameObject player;
	public GameObject scenarioController;
	public GameObject target;
	public bool isPlayerNear;
	public float friendliness;

	// Use this for initialization
	void Start ()
	{

		player = GameObject.Find ("Player");
		target = null;
		isPlayerNear = false;
		friendliness = 0f;
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		switch (state) {
		case BearState.HOSTILE:
			Debug.Log (">:(");
			if (target == player) {

			}
			if (friendliness > 0)
				state = BearState.FRIENDLY; 
			break;
		case BearState.FRIENDLY:
			Debug.Log (":)");
			if (friendliness <= 0)
				state = BearState.HOSTILE;
			break;
		case BearState.UNAWARE:
			Debug.Log ("-___-");
			if (Vector3.Distance (player.transform.position, transform.position) < 5f) {
				if (friendliness > 0) state = BearState.FRIENDLY;
				if (friendliness <= 0) state = BearState.HOSTILE;
			}
			break;
		}	
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.Equals (player)) {
			isPlayerNear = true;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.Equals (player)) {
			isPlayerNear = false;
		}
	}

	public void increaseFriendliness ()
	{
		friendliness += 1;
	}

	public void decreaseFriendliness ()
	{
		friendliness += 1;
	}
}
