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
	static Sprite inactive_Img;
	static Sprite active_Img;

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
		verbs_hashtable.Add ("Faster Bear", Bear);        // Done
		verbs_hashtable.Add ("Spawn Bear", Bear);         // Done
		verbs_hashtable.Add ("Stronger Bear", Bear);      // Done
		verbs_hashtable.Add ("Spawn Monster", Hex);       // Done
		verbs_hashtable.Add ("Spawn Boulder", Boulder);   // Done 
		//verbs_hashtable.Add ("Craze Chicken", Chicken);   // Done
		verbs_hashtable.Add ("Faster Chicken", Chicken);  // Done - Not Tested
		verbs_hashtable.Add ("Shrink Chicken", Chicken);  // Done
		verbs_hashtable.Add ("Lower Hex", Hex);
		verbs_hashtable.Add ("Raise Hex", Hex);
		verbs_hashtable.Add ("Wall Hex", Hex);            // Done
		verbs_hashtable.Add ("Fall Tree", Tree);          // Done
		verbs_hashtable.Add ("Smite Tree", Tree);         // Done
		verbs_hashtable.Add ("Spawn Tree", Tree);         // Done
		verbs_hashtable.Add ("Spawn Wolf", Wolf);
		verbs_hashtable.Add ("Faster Wolf", Wolf);        // Done - Not Tested
		verbs_hashtable.Add ("Stronger Wolf", Wolf);      // Done - Not Tested

		verbs_available = new List<string> ();
		string verb;
		StreamReader reader = new StreamReader ("Assets/Scripts/Scenarios/Verbs.txt", Encoding.Default);
		try {
			using (reader) {
				do {
					verb = reader.ReadLine ();
					if (verb != null) {
						verbs_available.Add (verb);
						//verbs_purchased.Add(verb);
					}
				} while (verb != null);

				reader.Close();
			}
		} catch (System.Exception e) {
			Debug.LogError (e.Message);
		}
        verbs_purchased = new List<string>();
        verbs_purchased.Add("Shrink Chicken");  // Done
        verbs_purchased.Add("Lower Hex");
        verbs_purchased.Add("Raise Hex");

    }

	// Use this for initialization
	void Start ()
	{
		AP [0] = GameObject.Find ("AP_1").GetComponent<Image> ();
		AP [1] = GameObject.Find ("AP_2").GetComponent<Image> ();
		AP [2] = GameObject.Find ("AP_3").GetComponent<Image> ();
		AP [3] = GameObject.Find ("AP_4").GetComponent<Image> ();
		AP [4] = GameObject.Find ("AP_5").GetComponent<Image> ();
		inactive_Img = AP [0].sprite;
		active_Img = Resources.Load ("APfull", typeof(Sprite)) as Sprite;

		SetAPFillSpeed ();
	}

	public static void SetAPFillSpeed ()
	{
		self.CancelInvoke ();
		ap_regen = TwitchController.max_captured_time / 5;
                DecreaseAP(5);
		self.InvokeRepeating ("GuiIncreaseAP", ap_regen, ap_regen);
	}

	void OnApplicationQuit() {
		CancelInvoke ();
	}

	public static void Do(string command, string effect, string hex) {
		int exit_status = -2;
		string verb = "";
		if (effect.Equals ("random") && hex.Equals ("random") && command.Equals ("hex"))
			exit_status = -1;
		else {
			switch (command) {
			case "bear":
				switch (effect) {
				case "faster":     verb = "Faster Bear";         break;
				case "stronger":   verb = "Stronger Bear";       break;
				case "spawn":      verb = "Spawn Bear";          break;
				} break;
			case "boulder":
				switch (effect) {
				case "spawn":      verb = "Spawn Boulder";       break;
				} break;
			case "chicken":
				switch (effect) {
				case "craze":      verb = "Craze Chicken";      break;
				case "faster":     verb = "Faster Chicken";      break;
				case "shrink":     verb = "Shrink Chicken";      break;
				} break;
			case "hex":
				switch (effect) {
				case "lower":      verb = "Lower Hex";           break;
				case "raise":      verb = "Raise Hex";           break;
				case "wall":       verb = "Wall Hex";            break;
				} break;
			case "monster":
				switch (effect) {
				case "spawn":      verb = "Spawn Monster";     break;
				} break;
			case "tree":
				switch (effect) {
				case "fall":       verb = "Fall Tree";           break;
				case "smite":      verb = "Smite Tree";          break;
				case "spawn":      verb = "Spawn Tree";          break;
				} break;
			case "wolf":
				switch (effect) {
				case "spawn":      verb = "Spawn Wolf";          break;
				case "faster":     verb = "Faster Wolf";         break;
				case "stronger":   verb = "Stronger Wolf";       break;
				} break;
			default:               verb = "Verb DNE";            break;
			}
		}
		if (debug_on) Debug.Log (verb);
		if (verbs_purchased.Contains (verb)) {
			exit_status = verbs_hashtable [verb].Invoke (command, effect, hex);
		}
		if (exit_status == 1)
			TwitchController.AddToBannerQueue ("~Twitch performed: " + verb + "!~");
		else if (exit_status == 0)
			TwitchController.AddToBannerQueue ("~Twitch tried to: "  + verb + " but failed!~");
		else if (exit_status == -1)
			TwitchController.AddToBannerQueue ("~Twitch is divided~");
		else if (exit_status == -2)
			TwitchController.AddToBannerQueue ("This is not possible: " + effect + " " + command + " " + hex);
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
		GameObject[] bears = GameObject.FindGameObjectsWithTag ("Bear");
		switch (effect) {
		case "faster":
			if (debug_on) Debug.Log ("Bear: effect = " + effect);
			if (bears.Length == 0) return 0;
			foreach (GameObject bear in bears) {
				bear.GetComponent<Animal> ().Rage ("faster");
				Instantiate (Resources.Load("Particle Effects/TwitchAction"), bear.transform.position, Quaternion.identity);
			}
			return 1;
		case "stronger":
			if (debug_on) Debug.Log ("Bear: effect = " + effect);
			if (bears.Length == 0) return 0;
			foreach (GameObject bear in bears) {
				bear.GetComponent<Animal> ().Rage ("stronger");
				Instantiate (Resources.Load("Particle Effects/TwitchAction"), bear.transform.position, Quaternion.identity);
			}
			return 1;
		case "spawn":
			if (debug_on) Debug.Log ("Bear: effect = " + effect);
			Spawn (hex, "Bear");
			return 1;
		default:
			if (debug_on) Debug.Log ("Bear defaulted");
			return -2;
		}
	}

	static int Boulder (string command, string effect, string hex) {
		if (hex.Equals ("random")) hex = WorldContainer.hexes [WorldContainer.RandomChance (WorldContainer.hexes.Length)];
		GameObject Hex = GameObject.Find (hex);
		switch (effect) {
		case "spawn":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().SwapRocks ();
			Instantiate (Resources.Load("Particle Effects/TwitchAction"), Hex.transform.position, Quaternion.identity);
			return 1;
		default:
			if (debug_on) Debug.Log ("Boulder defaulted");
			return -2;
		}
	}

	static int Chicken (string command, string effect, string hex) {
		GameObject[] chickens = WorldContainer.GetAllInstances ("Chicken");
		switch (effect) {
		case "craze":
			if (debug_on) Debug.Log ("Chicken: effect = " + effect);
			if (chickens.Length == 0) return 0;
			foreach (GameObject chicken in chickens) {
				if (chicken == null)
					return 0;
				chicken.GetComponent<Chicken> ().Craze ();
				Instantiate (Resources.Load("Particle Effects/TwitchAction"), chicken.transform.position, Quaternion.identity);
			}
			return 1;
		case "faster":
			if (debug_on) Debug.Log ("Chicken: effect = " + effect);
			if (chickens.Length == 0) return 0;
			foreach (GameObject chicken in chickens) {
				chicken.GetComponent<Chicken> ().DoubleSpeed ();
				Instantiate (Resources.Load("Particle Effects/TwitchAction"), chicken.transform.position, Quaternion.identity);
			}
			return 1;
		case "shrink":
			if (debug_on) Debug.Log ("Chicken: effect = " + effect);
			if (chickens.Length == 0) return 0;
			foreach (GameObject chicken in chickens) {
				chicken.GetComponent<Chicken> ().Shrink ();
				Instantiate (Resources.Load("Particle Effects/TwitchAction"), chicken.transform.position, Quaternion.identity);
			}
			return 1;
		default:
			if (debug_on) Debug.Log ("Chicken defaulted");
			return -2;
		}
	}

	static int Hex (string command, string effect, string hex) {
        if (hex.Equals ("random")) hex = WorldContainer.hexes [WorldContainer.RandomChance (WorldContainer.hexes.Length)];
		GameObject Hex = GameObject.Find (hex);
		switch(effect) {
		case "lower":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().Lower ();
			Instantiate (Resources.Load("Particle Effects/TwitchAction"), Hex.transform.position, Quaternion.identity);
			return 1;
		case "raise":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().Raise ();
			Instantiate (Resources.Load("Particle Effects/TwitchAction"), Hex.transform.position, Quaternion.identity);
			return 1;
		case "spawn":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().SwapMonster ();
			Instantiate (Resources.Load("Particle Effects/TwitchAction"), Hex.transform.position, Quaternion.identity);
			return 1;
		case "wall":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().Wall ();
			Instantiate (Resources.Load("Particle Effects/TwitchAction"), Hex.transform.position, Quaternion.identity);
			return 1;
		default:
			return -2;
		}
	}

	static int Tree (string command, string effect, string hex) {
		if (hex.Equals ("random")) hex = WorldContainer.hexes [WorldContainer.RandomChance (WorldContainer.hexes.Length)];
		GameObject Hex = GameObject.Find (hex);
		if (Hex == null) return 0;
		Tree[] trees = Hex.GetComponentsInChildren<Tree> ();
		switch (effect) {
		case "fall":
			if (trees.Length == 0) return 0;
			if (debug_on) Debug.Log ("Tree: effect = " + effect);
			foreach (Tree tree in trees) tree.Fall ();
			Instantiate (Resources.Load("Particle Effects/TwitchAction"), Hex.transform.position, Quaternion.identity);
			return 1;
		case "smite":
			if (trees.Length == 0) return 0;
			if (debug_on) Debug.Log ("Tree: effect = " + effect);
			GameObject[] Trees = GameObject.FindGameObjectsWithTag ("Tree");
			Tree the_tree = Trees[WorldContainer.RandomChance (Trees.Length)].GetComponent<Tree>();
			Instantiate (Resources.Load("Particle Effects/TwitchAction"), the_tree.transform.position, Quaternion.identity);
			the_tree.beginBurn ();
			return 1;
		case "spawn":
			if (debug_on) Debug.Log ("Tree: effect = " + effect);
			if (debug_on) Debug.Log ("Tree: Hex = " + hex + " " + Hex.name);
			Hex.GetComponent<HexControl> ().SwapTree ();
			Instantiate (Resources.Load("Particle Effects/TwitchAction"), Hex.transform.position, Quaternion.identity);
			return 1;
		default:
			if (debug_on) Debug.Log ("Tree defaulted");
			return -2;
		}
	}

	static int Wolf (string command, string effect, string hex) {
		GameObject[] wolves = GameObject.FindGameObjectsWithTag ("Wolf");
		switch (effect) {
		case "faster":
			if (debug_on) Debug.Log ("Wolf: effect = " + effect);
			if (wolves.Length == 0) return 0;
			foreach (GameObject wolf in wolves) {
				wolf.GetComponent<Animal> ().Rage ("faster");
				Instantiate (Resources.Load("Particle Effects/TwitchAction"), wolf.transform.position, Quaternion.identity);
			}
			return 1;
		case "stronger":
			if (debug_on) Debug.Log ("Wolf: effect = " + effect);
			if (wolves.Length == 0) return 0;
			foreach (GameObject wolf in wolves) {
				wolf.GetComponent<Animal> ().Rage ("stronger");
				Instantiate (Resources.Load("Particle Effects/TwitchAction"), wolf.transform.position, Quaternion.identity);
			}
			return 1;
		case "spawn":
			if (debug_on) Debug.Log ("Wolf: effect = " + effect);
			Spawn (hex, "Wolf");
			return 1;
		default:
			if (debug_on) Debug.Log ("Wolf defaulted");
			return -2;
		}
	}

	static void Spawn(string hex, string tag) {
		if (debug_on) Debug.Log ("Spawn: hex: " + hex);
		GameObject Hex;
		if (hex.Equals ("random")) {
			Transform room = GameObject.Find ("Map").transform;
			Hex = room.GetChild (WorldContainer.RandomChance (room.childCount)).gameObject;
		} else Hex = GameObject.Find (hex);
		if (Hex == null) return;
		GameObject spawn = WorldContainer.Create(tag, Hex.transform.position, Quaternion.identity);
                Instantiate (Resources.Load("Particle Effects/TwitchAction"), Hex.transform.position, Quaternion.identity);
		spawn.transform.SetParent (Hex.transform);
	}

	void GuiIncreaseAP () {
		IncreaseAP ();
	}

	void IncreaseAP () {
		if (curr_ap + 1 <= max_ap) {
			AP [curr_ap++].sprite = active_Img;
		}
	}

	static void IncreaseAP (int v) {
		while (v-- > 0) self.IncreaseAP ();
	}

	static void DecreaseAP () {
		if (curr_ap != 0) {
			AP [--curr_ap].sprite = inactive_Img;
		}
	}

	static void DecreaseAP (int v) {
		while (v-- > 0) DecreaseAP ();
	}
}
