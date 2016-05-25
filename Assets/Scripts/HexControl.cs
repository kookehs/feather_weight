﻿using UnityEngine;
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
	ICE,
	LAVA,
}

public enum HexType {
	GRASS,
	TREE,
	ROCK,
	BURROW,
	MONSTER,
	ICE,
	LAVA,
}

public class HexControl : MonoBehaviour {

	private bool raised = false;
	private float walltime = 10f;
	private float wallcooldown = 10f;
	private bool haswall = false;
	private bool canwall = true;
	public HexState state = HexState.IDLE;
	public HexType type = HexType.GRASS;
	public bool protectedHex = false;
	public float steprate = 1f;
	public Vector3 basepos;
	public Vector3 raisepos;
	public Vector3 moveto;
	public float maxheight = 3f;
		
	private ArrayList grasslist = new ArrayList{
		"LogHex",
		"LogHex1",
		"LogHex2",
		"LogHex4",
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

	private ArrayList lavalist = new ArrayList{
		"LavaHex",
	};

	private ArrayList icelist = new ArrayList{
		"IceHex 1",
	};

	private ArrayList particlelist = new ArrayList{
		"GrassOnly",
		"GrassFlowers"
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
				GetComponent<NavMeshObstacle> ().enabled = false;
			}
			break;
		case HexState.TREE:
			SwapTree ();
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
		case HexState.GRASS:
			SwapGrass ();
			state = HexState.IDLE;
			break;
		case HexState.ICE:
			SwapIce ();
			state = HexState.IDLE;
			break;
		case HexState.LAVA:
			SwapLava ();
			state = HexState.IDLE;
			break;
		}
	}

	public void Move(float rate){
		float step = rate * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, moveto, step);
	}

	public void Raise(){
		if (protectedHex)
			return;
		if (raised == false) {
			GetComponent<NavMeshObstacle> ().enabled = true;
			moveto = raisepos;
			state = HexState.RAISE;
		}
	}

	public void Lower(){
		if (protectedHex)
			return;
		if (raised == true) {
			moveto = basepos;
			state = HexState.LOWER;
		}
	}

	public void Wall(){
		if (protectedHex)
			return;
		if (haswall == false) {
			GameObject wall = Instantiate (Resources.Load ("Wall", typeof(GameObject))) as GameObject;
			wall.transform.position = transform.position;
			wall.transform.parent = transform;
			wall.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
			haswall = true;
			canwall = false;
			StartCoroutine (KillAtTime (wall, walltime));
		}
	}

	public void OnTriggerStay(Collider player){
		if (player.attachedRigidbody.transform.name == "Player") {
			player.attachedRigidbody.gameObject.GetComponent<PlayerMovementRB> ().hexImIn = transform.gameObject;
		}
	}

	public void SwapTree(){
		if (protectedHex)
			return;
		GameObject newhex = Instantiate (Resources.Load ((string)treelist [(int)Mathf.Floor (Random.value * ((float)treelist.Count-.001f))],typeof (GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		newhex.transform.Rotate (Vector3.forward* ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild("Hex").gameObject);
		type = HexType.TREE;
	}

	public void SwapGrass(){
		if (Random.value < .03f)
			SwapRocks ();
		else if (Random.value < .06f)
			SwapTree ();
		else{
			GameObject newhex = Instantiate (Resources.Load ((string)grasslist [(int)Mathf.Floor (Random.value * ((float)grasslist.Count-.001f))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		newhex.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild("Hex").gameObject);
		type = HexType.GRASS;
		//Populate ();
		}
	}

	public void SwapRocks(){
		if (protectedHex)
			return;
		GameObject newhex = Instantiate (Resources.Load ((string)rocklist [(int)Mathf.Floor (Random.value * ((float)rocklist.Count-.001f))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		newhex.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild("Hex").gameObject);
		type = HexType.ROCK;
	}

	public void SwapIce(){
		if (protectedHex)
			return;
		GameObject newhex = Instantiate (Resources.Load ((string)icelist [(int)Mathf.Floor (Random.value * ((float)icelist.Count-.001f))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		//newhex.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild("Hex").gameObject);
		type = HexType.ICE;
	}

	public void SwapLava(){
		if (protectedHex)
			return;
		GameObject newhex = Instantiate (Resources.Load ((string)lavalist [(int)Mathf.Floor (Random.value * ((float)lavalist.Count-.001f))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		//newhex.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild("Hex").gameObject);
		type = HexType.LAVA;
	}

	public void SwapMonster(){
		if (protectedHex)
			return;
		if (type != HexType.ROCK)
			return;
		GameObject newhex = Instantiate (Resources.Load ((string)monsterlist [(int)Mathf.Floor (Random.value * ((float)monsterlist.Count-.001f))], typeof(GameObject))) as GameObject;
		newhex.transform.name = "Hex";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
		newhex.transform.Rotate (Vector3.up * ((Mathf.Floor (Random.value * 6)) * 60));
		Destroy (transform.FindChild ("Hex").gameObject);
		type = HexType.MONSTER;

	}

	/*private void Populate() {
		GameObject newhex = Instantiate (Resources.Load ((string)particlelist [(int)Mathf.Floor (Random.value * (particlelist.Count))], typeof(GameObject))) as GameObject;
		Destroy (transform.FindChild ("Grass").gameObject);
		newhex.transform.name = "Grass";
		newhex.transform.position = transform.position;
		newhex.transform.parent = transform;
	}*/

	IEnumerator KillAtTime(GameObject tar, float time){
		yield return new WaitForSeconds (time);
		if (tar != null)
			Destroy (tar);
		haswall = false;
		StartCoroutine (RemoveWallCooldwon (wallcooldown));
	}

	IEnumerator RemoveWallCooldwon(float time){
		yield return new WaitForSeconds (time);
		canwall = true;
	}
}
