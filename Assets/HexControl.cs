using UnityEngine;
using System.Collections;

public enum HexState {
	BUFF,
	RAISE,
	LOWER,
	IDLE,
	TREE,
	WALL,
	GRASS,
	ROCK,
	MONSTER,
}

public enum HexType {
	GRASS,
	TREE,
	ROCK,
	BURROW,
	MONSTER,
}

public class HexControl : MonoBehaviour {

	private bool raised = false;
	private float walltime = 10f;
	private bool haswall = false;
	public HexState state = HexState.IDLE;
	public HexType type = HexType.GRASS;
	public float steprate = 1f;
	public Vector3 basepos;
	public Vector3 raisepos;
	public Vector3 moveto;
	public float maxheight = 3f;
		
	private ArrayList grasslist = new ArrayList{
		"LogHex",
		"LogHex1",
		"LogHex2",
		"LogHex3",
		"LogHex4",
		"LogHex5",
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

	private ArrayList monsterlist = new ArrayList{
		"RockMonsterHex",
	};

	// Use this for initialization
	void Start () {
		basepos = new Vector3 (
			transform.position.x,
			transform.position.y,
			transform.position.z);
		raisepos = new Vector3 (
			transform.position.x,
			transform.position.y + maxheight,
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
		case HexState.GRASS:
			SwapGrass ();
			state = HexState.IDLE;
			break;
		case HexState.ROCK:
			SwapRocks ();
			state = HexState.IDLE;
			break;
		case HexState.MONSTER:
			SwapMonster();
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
			wall.transform.Rotate (Vector3.right * ((Mathf.Floor (Random.value * 6)) * 60));
			haswall = true;
			StartCoroutine (KillAtTime (wall, walltime));
		}
	}

	public void SwapTree(){
		GameObject newhex = Instantiate (Resources.Load ((string)treelist [(int)Mathf.Floor (Random.value * (treelist.Count))],typeof (GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		newhex.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild("Hex").gameObject);
		type = HexType.TREE;
	}

	public void SwapGrass(){
		GameObject newhex = Instantiate (Resources.Load ((string)grasslist [(int)Mathf.Floor (Random.value * (grasslist.Count))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		newhex.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild("Hex").gameObject);
		type = HexType.GRASS;
	}

	public void SwapRocks(){
		GameObject newhex = Instantiate (Resources.Load ((string)rocklist [(int)Mathf.Floor (Random.value * (rocklist.Count))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		newhex.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild("Hex").gameObject);
		type = HexType.ROCK;
	}

	public void SwapMonster(){
		if (type != HexType.ROCK)
			return;
		GameObject newhex = Instantiate (Resources.Load ((string)monsterlist [(int)Mathf.Floor (Random.value * (monsterlist.Count))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		newhex.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild ("Hex").gameObject);
		type = HexType.MONSTER;

	}

	IEnumerator KillAtTime(GameObject tar, float time){
		yield return new WaitForSeconds (time);
		Destroy (tar);
		haswall = false;
	}
}
