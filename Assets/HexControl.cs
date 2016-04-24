using UnityEngine;
using System.Collections;

/*public enum HexState {
	BUFF,
	RAISE,
	LOWER,
	IDLE,
}

public class HexControl : MonoBehaviour {

	private bool raised = false;
	private float walltime = 10f;
	private float endwalltime = 0f;
	private float currenttime = 0f;
	public HexState state = HexState.IDLE;
	public float steprate = 2f;
	public Vector3 basepos;
	public Vector3 raisepos;
	public Vector3 moveto;

	private IList grasslist = {
		"GrassHex",
		"GrassHex1",
		"GrassHex2",
		"GrassHex3",
		"GrassHex4",
		"GrassHex5",
		"GrassHex6",
		"GrassHex7",
		"GrassHex8",
		"GrassHex9",
		"GrassHex10",
		"GrassHex11",
		"GrassHex12",
		"GrassHex13",
		"GrassHex14"
	};

	private IList rocklist = {
		"BoulderHex",
		"BoulderHex1"
	};

	private IList treelist = {
		"TreeClusterTile1",
		"TreeClusterTile2"
	};
		
	// Use this for initialization
	void Start () {
		basepos = new Vector3 (
			transform.position.x,
			transform.position.y,
			transform.position.z);
		raisepos = new Vector3 (
			transform.position.x,
			transform.position.y + .5f,
			transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case HexState.IDLE:
			break;
		case HexState.RAISE:
			Move (steprate);
			if (transform.position == moveto)
				state = HexState.IDLE;
			break;
		case HexState.LOWER:
			Move (steprate);
			if (transform.position == moveto)
				state = HexState.IDLE;
			break;
		}
	}

	void Move(float rate){
		float step = rate * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, moveto, step);
	}

	void Raise(){
		moveto = raisepos;
		raised = true;
		state = HexState.RAISE;
	}

	void Lower(){
		moveto = basepos;
		raised = false;
		state = HexState.LOWER;
	}

	void Wall(){
		GameObject wall = Instantiate (Resources.Load ("Wall", typeof(GameObject))) as GameObject;
		wall.transform.parent = transform;
	}

	void SwapTree(){
		GameObject newhex = Instantiate (Resources.Load (treelist [Mathf.Floor (Random.value * (treelist.Count))], typeof(GameObject))) as GameObject;
		Destroy (transform.GetChild (0).gameObject);
		newhex.transform.parent = transform;
	}

	void SwapGrass(){
		GameObject newhex = Instantiate (Resources.Load (treelist [Mathf.Floor (Random.value * (grasslist.Count))], typeof(GameObject))) as GameObject;
		Destroy (transform.GetChild (0).gameObject);
		newhex.transform.parent = transform;
	}

	void SwapRocks(){
		GameObject newhex = Instantiate (Resources.Load (treelist [Mathf.Floor (Random.value * (rocklist.Count))], typeof(GameObject))) as GameObject;
		Destroy (transform.GetChild (0).gameObject);
		newhex.transform.parent = transform;
	}
}

*/