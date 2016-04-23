using UnityEngine;
using System.Collections;

public enum HexState {
	BUFF,
	RAISE,
	LOWER,
	IDLE,
}

public class HexControl : MonoBehaviour {

	private bool raised = false;
	private float bufftime = 10f;
	private float endbufftime = 0f;
	private float currenttime = 0f;
	public HexState state = HexState.IDLE;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case HexState.BUFF:
			break;
		
		}
	}


}
