using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TwitchActionController : MonoBehaviour
{
	static readonly int max_ap = 5;
	static int curr_ap = 0;
	static float ap_regen = 2f;

	static Image[] AP = new Image[max_ap];
	static readonly Color inactive_clr = new Color (1f, 1f, 1f, 1f);
	static readonly Color active_clr   = new Color(0.392f, 0.255f, 0.647f, 1f);

	static string[] cmd_separator = { "_" };

	static bool debug_on = true;

	delegate int Verb (string command, string effect, string hex);
	static Dictionary<string,Verb> available_verbs;
	static List<string>            purchased_verbs;

	static TwitchActionController self;
	public TwitchActionController instance {
		get { return self; }
	}

	void Awake () {
		self = GameObject.Find ("Controllers").GetComponent<TwitchActionController> ();
		available_verbs = new Dictionary<string,Verb> ();
		available_verbs.Add ("Bear_Faster", Bear);        // Done - Not Tested
		available_verbs.Add ("Bear_Spawn", Bear);         // Done - Not Tested
		available_verbs.Add ("Bear_Stronger", Bear);      // Done - Not Tested
		available_verbs.Add ("Boulder_Monster", Boulder); //
		available_verbs.Add ("Boulder_Spawn", Boulder);   // Done - Not Tested
		available_verbs.Add ("Chicken_Crazed", Chicken);  // Done - Not Tested
		available_verbs.Add ("Chicken_Faster", Chicken);  // Done - Not Tested
		available_verbs.Add ("Chicken_Shrink", Chicken);  // Done - Not Tested
		available_verbs.Add ("Hex_Wall", Hex);            //
		available_verbs.Add ("Tree_Fall", Tree);          // Done - Not Tested
		available_verbs.Add ("Tree_Smite", Tree);         // Done - Not Tested
		available_verbs.Add ("Tree_Spawn", Tree);         // Done - Not Tested
		purchased_verbs = new List<string> ();
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

	public static void Do(string command, string effect, string hex) {
		string verb = "";
		switch (command) {
		case "bear":
			switch (effect) {
			case "faster":     verb = "Bear_Faster";         break;
			case "stronger":   verb = "Bear_Stronger";       break;
			case "spawn":      verb = "Bear_Spawn";          break;
			} break;
		case "boulder":
			switch (effect) {
			case "monster":    verb = "Boulder_Monster";     break;
			case "stronger":   verb = "Boulder_Stronger";    break;
			} break;
		case "chicken":
			switch (effect) {
			case "craze":      verb = "Chicken_Crazed";      break;
			case "faster":     verb = "Chicken_Faster";      break;
			case "shrink":     verb = "Chicken_Shrink";      break;
			} break;
		case "hex":
			switch (effect) {
			case "wall":       verb = "Hex_Wall";            break;
			} break;
		case "tree":
			switch (effect) {
			case "fall":       verb = "Tree_Fall";           break;
			case "smite":      verb = "Tree_Smite";          break;
			case "spawn":      verb = "Tree_Spawn";          break;
			} break;
		default:               verb = "Verb DNE";            break;
		}
		if (debug_on) Debug.Log (verb);
		if (purchased_verbs.Contains (verb)) {
			int exit_status = available_verbs [verb].Invoke (command, effect, hex);
			DecreaseAP (5);
		}
	}

	public static void Purchase (string s) {
		if (available_verbs.ContainsKey (s))
			purchased_verbs.Add (s);
		else if (debug_on)
			Debug.Log ("Cannot purchase verb: " + s);
	}

	static int Bear (string command, string effect, string hex) {
		GameObject[] bears = WorldContainer.GetAllInstances ("Bear");
		if (bears != null) {
			switch (effect) {
			case "faster":
				if (debug_on) Debug.Log ("Bear: effect = " + effect);
				if (curr_ap >= 3) {
					if (bears.Length > 0) {
						foreach (GameObject bear in bears) bear.GetComponent<BearNMA> ().Rage ("faster");
						return 3;
					}
				} break;
			case "stronger":
				if (debug_on) Debug.Log ("Bear: effect = " + effect);
				if (curr_ap >= 3) {
					if (bears.Length > 0) {
						foreach (GameObject bear in bears) bear.GetComponent<BearNMA> ().Rage ("stronger");
						return 3;
					}
				} break;
			case "spawn":
				if (debug_on) Debug.Log ("Bear: effect = " + effect);
				if (curr_ap >= 2) {
					Spawn (hex, "Bear");
					return 2;
				} break;
			default:
				if (debug_on) Debug.Log ("Bear defaulted"); break;
			}
		}
		return 0;
	}

	static int Boulder (string command, string effect, string hex) {
		switch (effect) {
		case "spawn":
			if (curr_ap >= 1) {
				Spawn (hex, "Boulder");
				return 1;
			} break;
		default:
			if (debug_on) Debug.Log ("Boulder defaulted"); break;
		}
		return 0;
	}

	static int Chicken (string command, string effect, string hex) {
		GameObject[] chickens = WorldContainer.GetAllInstances ("Chicken");
		if (chickens != null) {
			switch (effect) {
			case "craze":
				if (debug_on) Debug.Log ("Chicken: effect = " + effect);
				if (curr_ap >= 3) {
					if (chickens.Length > 0) {
						foreach (GameObject chicken in chickens) chicken.GetComponent<Chicken> ().Craze ();
						return 3;
					}
				} break;
			case "faster":
				if (debug_on) Debug.Log ("Chicken: effect = " + effect);
				if (curr_ap >= 3) {
					if (chickens.Length > 0) {
						foreach (GameObject chicken in chickens) chicken.GetComponent<Chicken> ().DoubleSpeed ();
						return 3;
					}
				} break;
			case "shrink":
				if (debug_on) Debug.Log ("Chicken: effect = " + effect);
				if (curr_ap >= 3) {
					if (chickens.Length > 0) {
						foreach (GameObject chicken in chickens) chicken.GetComponent<Chicken> ().Shrink ();
						return 3;
					}
				} break;
			default:
				if (debug_on) Debug.Log ("Chicken defaulted"); break;
			}
		}
		return 0;
	}

	static int Hex (string command, string effect, string hex) {
        if (hex.Equals ("random")) hex = WorldContainer.hexes [WorldContainer.RandomChance (WorldContainer.hexes.Length)];
		GameObject Hex = GameObject.Find (hex);

                switch(effect) {
                case "wall":
                        Hex.GetComponent<HexControl>().Wall();
                        break;
                }

		return 0;
	}

	static int Tree (string command, string effect, string hex) {
		if (hex.Equals ("random")) hex = WorldContainer.hexes [WorldContainer.RandomChance (WorldContainer.hexes.Length)];
		GameObject Hex = GameObject.Find (hex);
		if (Hex == null) return 0;
		Tree[] trees = Hex.GetComponentsInChildren<Tree> ();
		if (trees != null) {
			switch (effect) {
		    case "fall":
				if (debug_on) Debug.Log ("Tree: effect = " + effect);
				if (curr_ap < 3 || trees.Length == 0) break;
				else {
					foreach (Tree tree in trees) tree.Fall ();
					return 3;
				}
			case "smite":
				if (debug_on) Debug.Log ("Tree: effect = " + effect);
				if (curr_ap < 3 || trees.Length == 0) break;
				else {
					trees [WorldContainer.RandomChance (trees.Length)].GetSmitten ();
					return 3;
				}
			case "spawn":
				if (debug_on) Debug.Log ("Tree: effect = " + effect);
				if (curr_ap < 1) break;
				else {
                                        if (debug_on) Debug.Log ("Tree: Hex = " + hex + " " + Hex.name);
					Hex.GetComponent<HexControl>().SwapTree();
					return 1;
				}
			default:
				if (debug_on) Debug.Log ("Tree defaulted"); break;
			}
		}
		return 0;
	}

	static void Spawn(string hex, string tag) {
		if (debug_on) Debug.Log ("Spawn: hex: " + hex);
		GameObject Hex;
		if (hex.Equals ("random")) {
			Transform room = GameObject.Find ("Map").transform;
			Hex = room.GetChild (WorldContainer.RandomChance (room.childCount)).gameObject;
		} else Hex = GameObject.Find (hex);
		GameObject spawn = WorldContainer.Create(tag, Hex.transform.position, Quaternion.identity);
		spawn.transform.SetParent (Hex.transform);
		//spawn.GetComponent<Animal>().SkyDrop ();
	}

	void IncreaseAP () {
		if (curr_ap + 1 <= max_ap) {
			AP [curr_ap++].color = active_clr;
			//TODO More OJ to satisfy Bill later
		}
	}

	static void IncreaseAP (int v) {
		while (v-- > 0) self.IncreaseAP ();
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
