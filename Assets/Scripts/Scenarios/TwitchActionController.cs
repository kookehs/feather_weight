using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
	static Dictionary<string,Verb> verbs_hashtable;
	static List<string>            verbs_available,
								   verbs_purchased;

	static TwitchActionController self;
	public TwitchActionController instance {
		get { return self; }
	}

	void Awake () {
		self = GameObject.Find ("Controllers").GetComponent<TwitchActionController> ();
		verbs_hashtable = new Dictionary<string,Verb> ();
		verbs_hashtable.Add ("Bear_Faster", Bear);        // Done - Not Tested
		verbs_hashtable.Add ("Bear_Spawn", Bear);         // Done - Not Tested
		verbs_hashtable.Add ("Bear_Stronger", Bear);      // Done - Not Tested
		verbs_hashtable.Add ("Boulder_Monster", Hex);     // Done - Not Tested
		verbs_hashtable.Add ("Boulder_Spawn", Boulder);   // Done - Not Tested
		verbs_hashtable.Add ("Chicken_Crazed", Chicken);  // Done - Not Tested
		verbs_hashtable.Add ("Chicken_Faster", Chicken);  // Done - Not Tested
		verbs_hashtable.Add ("Chicken_Shrink", Chicken);  // Done - Not Tested
		verbs_hashtable.Add ("Hex_Lower", Hex);
		verbs_hashtable.Add ("Hex_Raise", Hex);
		verbs_hashtable.Add ("Hex_Wall", Hex);            // Done - Not Tested
		verbs_hashtable.Add ("Tree_Fall", Tree);          // Done - Not Tested
		verbs_hashtable.Add ("Tree_Smite", Tree);         // Done - Not Tested
		verbs_hashtable.Add ("Tree_Spawn", Tree);         // Done - Not Tested

		verbs_available = new List<string> ();
		verbs_purchased = new List<string> ();
		string verb;
		StreamReader reader = new StreamReader ("Assets/Scripts/Scenarios/Verbs.txt", Encoding.Default);
		try {
			using (reader) {
				do {
					verb = reader.ReadLine ();
					if (verb != null) {
						verbs_available.Add (verb);
						verbs_purchased.Add(verb);
					}
				} while (verb != null);

				reader.Close();
			}
		} catch (System.Exception e) {
			Debug.LogError (e.Message);
		}


	}

	// Use this for initialization
	void Start ()
	{
		AP [0] = GameObject.Find ("AP_1").GetComponent<Image> ();
		AP [1] = GameObject.Find ("AP_2").GetComponent<Image> ();
		AP [2] = GameObject.Find ("AP_3").GetComponent<Image> ();
		AP [3] = GameObject.Find ("AP_4").GetComponent<Image> ();
		AP [4] = GameObject.Find ("AP_5").GetComponent<Image> ();

		SetAPFillSpeed ();
	}

	public static void SetAPFillSpeed ()
	{
		self.CancelInvoke ();
		ap_regen = TwitchController.max_captured_time / 5;
		self.InvokeRepeating ("IncreaseAP", ap_regen, ap_regen);
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
			case "spawn":      verb = "Boulder_Spawn";       break;
			} break;
		case "chicken":
			switch (effect) {
			case "craze":      verb = "Chicken_Crazed";      break;
			case "faster":     verb = "Chicken_Faster";      break;
			case "shrink":     verb = "Chicken_Shrink";      break;
			} break;
		case "hex":
			switch (effect) {
			case "lower":      verb = "Hex_Lower";           break;
			case "raise":      verb = "Hex_Raise";           break;
			case "wall":       verb = "Hex_Wall";            break;
			} break;
		case "monster":
			switch (effect) {
			case "spawn":      verb = "Boulder_Monster";     break;
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
		int exit_status = -1;
		if (verbs_purchased.Contains (verb)) {
			exit_status = verbs_hashtable [verb].Invoke (command, effect, hex);
		}
		DecreaseAP (5);
	}

	public static void Purchase (string s) {
		if (verbs_hashtable.ContainsKey (s)) {
			verbs_purchased.Add (s);
			verbs_available.Remove (s);
		} else if (debug_on)
			Debug.Log ("Cannot purchase verb: " + s);
	}

	// Preconditions:
	// 	n == the number of random verbs desired
	// 	n >= the number of verbs still available for purchase
	// Postconditions:
	//	if preconditions are met, return a list of n random verbs
	//	return null otherwise
	public static List<string> VerbShop (int n) {
		if (verbs_available.Count  < n) return null;
		if (verbs_available.Count == n) return verbs_available;

		for (int i = 0; i < verbs_available.Count - 1; ++i) {
			int j = WorldContainer.RandomChance (verbs_available.Count - i);
			string temp = System.String.Copy(verbs_available [i]);
			verbs_available [i] = verbs_available [i + j];
			verbs_available [i + j] = temp;
		}

		List<string> choices = new List<string> ();
		while (n-- > 0) choices.Add (verbs_available [n]);
		return choices;
	}

	static int Bear (string command, string effect, string hex) {
		GameObject[] bears = WorldContainer.GetAllInstances ("Bear");
		if (bears != null) {
			switch (effect) {
			case "faster":
				if (debug_on) Debug.Log ("Bear: effect = " + effect);
				foreach (GameObject bear in bears) bear.GetComponent<BearNMA> ().Rage ("faster");
				break;
			case "stronger":
				if (debug_on) Debug.Log ("Bear: effect = " + effect);
				foreach (GameObject bear in bears) bear.GetComponent<BearNMA> ().Rage ("stronger");
				break;
			case "spawn":
				if (debug_on) Debug.Log ("Bear: effect = " + effect);
				Spawn (hex, "Bear");
				break;
			default:
				if (debug_on) Debug.Log ("Bear defaulted");
				break;
			}
		}
		return 0;
	}

	static int Boulder (string command, string effect, string hex) {
		if (hex.Equals ("random")) hex = WorldContainer.hexes [WorldContainer.RandomChance (WorldContainer.hexes.Length)];
		GameObject Hex = GameObject.Find (hex);
		if (Hex == null) return 0;
		switch (effect) {
		case "spawn":
			//Hex.GetComponent<HexControl> ().SwapRocks ();
			break;
		default:
			if (debug_on) Debug.Log ("Boulder defaulted");
			break;
		}
		return 0;
	}

	static int Chicken (string command, string effect, string hex) {
		GameObject[] chickens = WorldContainer.GetAllInstances ("Chicken");
		if (chickens != null) {
			switch (effect) {
			case "craze":
				if (debug_on) Debug.Log ("Chicken: effect = " + effect);
				foreach (GameObject chicken in chickens) chicken.GetComponent<Chicken> ().Craze ();
				break;
			case "faster":
				if (debug_on) Debug.Log ("Chicken: effect = " + effect);
				foreach (GameObject chicken in chickens) chicken.GetComponent<Chicken> ().DoubleSpeed ();
				break;
			case "shrink":
				if (debug_on) Debug.Log ("Chicken: effect = " + effect);
				foreach (GameObject chicken in chickens) chicken.GetComponent<Chicken> ().Shrink ();
				break;
			default:
				if (debug_on) Debug.Log ("Chicken defaulted");
				break;
			}
		}
		return 0;
	}

	static int Hex (string command, string effect, string hex) {
        if (hex.Equals ("random")) hex = WorldContainer.hexes [WorldContainer.RandomChance (WorldContainer.hexes.Length)];
		GameObject Hex = GameObject.Find (hex);

		switch(effect) {
		case "lower":
			Hex.GetComponent<HexControl> ().Lower ();
			break;
		case "raise":
			Hex.GetComponent<HexControl> ().Raise ();
			break;
		case "spawn":
			Hex.GetComponent<HexControl> ().SwapMonster ();
			break;
		case "wall":
			Hex.GetComponent<HexControl>().Wall();
			break;
		default:
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
				foreach (Tree tree in trees) tree.Fall ();
				break;
			case "smite":
				if (debug_on) Debug.Log ("Tree: effect = " + effect);
				if (trees.Length > 0)
					trees [WorldContainer.RandomChance (trees.Length)].GetSmitten ();
				break;
			case "spawn":
				if (debug_on) Debug.Log ("Tree: effect = " + effect);
				if (debug_on) Debug.Log ("Tree: Hex = " + hex + " " + Hex.name);
				Hex.GetComponent<HexControl> ().SwapTree ();
				break;
			default:
				if (debug_on) Debug.Log ("Tree defaulted");
				break;
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
                Instantiate (Resources.Load("TwitchAction"), Hex.transform.position, Quaternion.identity);
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
