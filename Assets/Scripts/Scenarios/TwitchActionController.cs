using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TwitchActionController : MonoBehaviour
{
	static readonly int max_ap = 5;
	static int curr_ap = 0;
	static float ap_regen = 1000f;

	static Image[] AP = new Image[max_ap];
	static readonly Color inactive_clr = new Color (1f, 1f, 1f, 1f);
	static readonly Color active_clr   = new Color(0.392f, 0.255f, 0.647f, 1f);

	static string[] cmd_separator = { "_" };

	static bool debug_on = true;

	static TwitchActionController self;
	public TwitchActionController instance {
		get { return self; }
	}

	void Awake () {
		self = GameObject.Find ("Controllers").GetComponent<TwitchActionController> ();
	}

	// Use this for initialization
	void Start ()
	{
		
		AP [0] = GameObject.Find ("AP_1").GetComponent<Image> ();
		AP [1] = GameObject.Find ("AP_2").GetComponent<Image> ();
		AP [2] = GameObject.Find ("AP_3").GetComponent<Image> ();
		AP [3] = GameObject.Find ("AP_4").GetComponent<Image> ();
		AP [4] = GameObject.Find ("AP_5").GetComponent<Image> ();

		InvokeRepeating ("IncreaseAP", ap_regen, ap_regen);

		// Test lines - IncreaseAP (AP) for AP level you want; then Do(command) the command you want to test
		//IncreaseAP (5);
		//Do ("tree_nut");
		//Do ("nut_grow");
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnApplicationQuit() {
		CancelInvoke ();
	}

	// command synopsis: target_command_name[_argument...]
	// --target: the target object, e.g. "chicken", or "hex" if hex-based
	// --command: the command we want to do
	// --name: name of the object, e.g. name of the chicken, or hex number if hex-based
	// --argument: any additional argument(s)
	public static void Do(string command) {
		string[] argv = command.Split (cmd_separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (argv [0]) {
		case "bear":
			if (debug_on) Debug.Log ("argv[0] = " + argv[0]);
			DecreaseAP (DoBear (argv));
			break;
		case "chicken":
			if (debug_on) Debug.Log ("argv[0] = " + argv[0]);
			DecreaseAP (DoChicken (argv));
			break;
		case "hex":
			if (debug_on) Debug.Log ("argv[0] = " + argv[0]);
			DecreaseAP (DoHex (argv));
			break;
		case "nut":
			if (debug_on) Debug.Log ("argv[0] = " + argv[0]);
			DecreaseAP (DoNut (argv));
			break;
		case "tree":
			if (debug_on) Debug.Log ("argv[0] = " + argv [0]);
			DecreaseAP (DoTree (argv));
			break;
		default:
			Debug.Log ("Incorrect Command: " + command);
			break;
		}
	}

	static int DoBear(string[] argv) {
		GameObject[] bears = WorldContainer.GetAllInstances ("Bear");
		if (bears != null) {
			switch (argv [1]) {
			case "powerup":
				if (debug_on) Debug.Log ("argv[1] = " + argv[1]);
				if (curr_ap < 3 || bears.Length == 0) break;
				else {
					bears [WorldContainer.RandomChance (bears.Length)].GetComponent<BearNMA> ().Rage ();
					return 3;
				}
			case "spawn":
				if (debug_on) Debug.Log ("argv[1] = " + argv[1]);
				if (curr_ap >= 2) {
					Debug.Log ("Bear spawn");
					Spawn (argv, "Bear");
					return 2;
				} break;
			default:
				if (debug_on) Debug.Log ("DoBear defaulted");
				break;
			}
		}
		return 0;
	}

	static int DoChicken(string[] argv) {
		Chicken chicken = GameObject.Find (argv [2]).GetComponent<Chicken> ();
		switch (argv [1]) {
		case "speed":
			if (debug_on) Debug.Log ("argv[1] = " + argv[1]);
			chicken.DoubleSpeed ();
			break;
		case "jump":
			if (debug_on) Debug.Log ("argv[1] = " + argv[1]);
			chicken.Craze ();
			break;
		case "shrink":
			if (debug_on) Debug.Log ("argv[1] = " + argv[1]);
			chicken.Shrink ();
			break;
		default:
			if (debug_on) Debug.Log ("DoChicken defaulted");
			break;
		}
		return 0;
	}

	static int DoHex (string[] argv) {
		GameObject hex = GameObject.Find (argv [1]).transform.GetChild (0).gameObject;
		switch (argv [1]) {
		default:
			if (debug_on) Debug.Log ("DoHex defaulted");
			break;
		}
		return 0;
	}

	static int DoNut (string[] argv) {
		GameObject[] nuts = WorldContainer.GetAllInstances ("Nut");
		if (debug_on) Debug.Log (nuts.Length);
		if (nuts != null) {
			switch (argv [1]) {
			case "grow":
				if (debug_on) Debug.Log ("argv[1] = " + argv [1]);
				if (curr_ap < 1 || nuts.Length == 0) break;
				else {
					GameObject nut = nuts [WorldContainer.RandomChance (nuts.Length)].gameObject;
					GameObject tree = WorldContainer.Create ("PineTree", nut.transform.position, Quaternion.identity);
					tree.transform.localScale = new Vector3 (0.42f, 0.42f, 0.42f);
					tree.transform.position = new Vector3 (tree.transform.position.x, 0, tree.transform.position.z);
					tree.transform.rotation = Quaternion.Euler (new Vector3 (270, 0, 0));
					WorldContainer.Remove (nut);
					return 1;
				}
			default:
				if (debug_on) Debug.Log ("DoNut defaulted");
				break;
			}
		}
		return 0;
	}

	static int DoTree (string[] argv) {
		GameObject[] trees = WorldContainer.GetAllInstances ("Tree");
		if (trees != null) {
			switch (argv [1]) {
			case "bear":
				if (debug_on) Debug.Log ("argv[1] = " + argv[1]);
				if (curr_ap < 2 || trees.Length == 0) break;
				else {
					trees [WorldContainer.RandomChance (trees.Length)].GetComponent<Tree> ().containsBear = true;
					return 2;
				}
			case "fall":
				if (debug_on) Debug.Log ("argv[1] = " + argv[1]);
				if (curr_ap < 1 || trees.Length == 0) break;
				else {
					trees [WorldContainer.RandomChance (trees.Length)].GetComponent<Tree> ().Fall ();
					return 1;
				}
			case "nut":
				if (debug_on) Debug.Log ("argv[1] = " + argv [1]);
				if (curr_ap < 1 || trees.Length == 0) break;
				else {
					trees [WorldContainer.RandomChance (trees.Length)].GetComponent<Tree> ().DropNut ();
					return 1;
				}	
			case "smite":
				if (debug_on) Debug.Log ("argv[1] = " + argv[1]);
				if (curr_ap < 3 || trees.Length == 0) break;
				else {
					trees [WorldContainer.RandomChance (trees.Length)].GetComponent<Tree> ().GetSmitten ();
					return 3;
				}
			default:
				if (debug_on) Debug.Log ("DoTree defaulted");
				break;
			}
		}
		return 0;
	}

	static void Spawn(string[] argv, string tag) {
		if (debug_on) Debug.Log ("argv[2] = " + argv[2]);
		GameObject hex;
		if (argv [2].Equals ("random")) {
			Transform room = GameObject.Find ("Room").transform;
			hex = room.GetChild (WorldContainer.RandomChance (room.childCount)).gameObject;
		} else hex = GameObject.Find (argv [2]);
		GameObject spawn = WorldContainer.Create(tag, hex.transform.position, Quaternion.identity);
		//spawn.GetComponent<Animal>().SkyDrop ();
	}

	static void IncreaseAP () {
		if (curr_ap + 1 <= max_ap) {
			AP [curr_ap++].color = active_clr;
			//TODO More OJ to satisfy Bill later
		}
	}

	static void IncreaseAP (int v) {
		while (v-- > 0) IncreaseAP ();
	}

	static void DecreaseAP () {
		if (curr_ap != 0) {
			AP [--curr_ap].color = inactive_clr;
			//TODO More OJ to satisfy Bill later
		}
	}

	static void DecreaseAP (int v) {
		while (v-- > 0) DecreaseAP ();
	}
}