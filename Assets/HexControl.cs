using UnityEngine;
using System.Collections;

public enum HexState {
	BUFF,
	RAISE,
	LOWER,
	IDLE,
	TREE,
	WALL,

}

public class HexControl : MonoBehaviour {

	private bool raised = false;
	private float walltime = 10f;
	private bool haswall = false;
	public HexState state = HexState.IDLE;
	public float steprate = 2f;
	public Vector3 basepos;
	public Vector3 raisepos;
	public Vector3 moveto;

	private ArrayList grasslist = new ArrayList{
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

	private ArrayList rocklist = new ArrayList{
		"BoulderHex",
		"BoulderHex1"
	};

	private ArrayList treelist = new ArrayList{
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
			transform.position.y + 3f,
			transform.position.z);
	}

	// Update is called once per frame
	void Update () {
		switch (state) {
		case HexState.IDLE:
			break;
		case HexState.RAISE:
			moveto = raisepos;
			Move (steprate);
			if (transform.position == moveto) {
				state = HexState.IDLE;
				raised = true;
			}
			break;
		case HexState.LOWER:
			moveto = basepos;
			Move (steprate);
			if (transform.position == moveto) {
				state = HexState.IDLE;
				raised = false;
			}
			break;
		case HexState.TREE:
			SwapTree ();
			state = HexState.IDLE;
			break;
		case HexState.WALL:
			Wall ();
			state = HexState.IDLE;
			break;
		}
	}

	public void Move(float rate){
		float step = rate * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, moveto, step);
	}

	public void Raise(){
		if (raised == false) {
			moveto = raisepos;
			state = HexState.RAISE;
		}
	}

	public void Lower(){
		if (raised == true) {
			moveto = basepos;
			state = HexState.LOWER;
		}
	}

	public void Wall(){
		if (haswall == false) {
			GameObject wall = Instantiate (Resources.Load ("Wall", typeof(GameObject))) as GameObject;
			wall.transform.position = transform.position;
			wall.transform.parent = transform;
			haswall = true;
			StartCoroutine (KillAtTime (wall, walltime));
		}
	}

	public void SwapTree(){
		GameObject newhex = Instantiate (Resources.Load ((string)treelist [(int)Mathf.Floor (Random.value * (treelist.Count))],typeof (GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		Destroy (transform.FindChild("Hex").gameObject);
	}

	public void SwapGrass(){
		GameObject newhex = Instantiate (Resources.Load ((string)grasslist [(int)Mathf.Floor (Random.value * (grasslist.Count))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		Destroy (transform.FindChild("Hex").gameObject);
	}

	public void SwapRocks(){
		GameObject newhex = Instantiate (Resources.Load ((string)rocklist [(int)Mathf.Floor (Random.value * (rocklist.Count))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		Destroy (transform.FindChild("Hex").gameObject);
	}

	IEnumerator KillAtTime(GameObject tar, float time){
		yield return new WaitForSeconds (time);
		Destroy (tar);
		haswall = false;
	}
}
