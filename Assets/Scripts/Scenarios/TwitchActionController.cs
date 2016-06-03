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
	static Dictionary<string,Verb>         verbs_hashtable;
	static Dictionary<string,List<string>> verbs_tiergraph;
	static List<string>                    verbs_available,
								           verbs_purchased;

	static TwitchActionController self;
	public TwitchActionController instance {
		get { return self; }
	}

        public static List<string> verbs {
                get { return verbs_purchased; }
        }

	void Awake () {
		self = GameObject.Find ("Controllers").GetComponent<TwitchActionController> ();
		verbs_hashtable = new Dictionary<string,Verb> ();
		verbs_hashtable.Add ("Faster Bear", Bear);        // Done
		verbs_hashtable.Add ("Spawn Bear", Bear);         // Done
		verbs_hashtable.Add ("Stronger Bear", Bear);      // Done
		verbs_hashtable.Add ("Spawn Monster", Hex);       // Done
		verbs_hashtable.Add ("Spawn Rock", Boulder);   // Done
		verbs_hashtable.Add ("Craze Chicken", Chicken);   // Done
		verbs_hashtable.Add ("Decoy Chicken", Hex);
		//verbs_hashtable.Add ("Faster Chicken", Chicken);  // Done - Not Tested
		verbs_hashtable.Add ("Shrink Chicken", Chicken);  // Done
		verbs_hashtable.Add ("Super Chicken", Hex);
		verbs_hashtable.Add ("Ice Hex", Hex);
		verbs_hashtable.Add ("Lava Hex", Hex);
		verbs_hashtable.Add ("Lower Hex", Hex);
		verbs_hashtable.Add ("Raise Hex", Hex);
		verbs_hashtable.Add ("Wall Hex", Hex);            // Done
		verbs_hashtable.Add ("Fall Tree", Tree);          // Done
		verbs_hashtable.Add ("Smite Tree", Tree);         // Done
		verbs_hashtable.Add ("Spawn Tree", Tree);         // Done
		verbs_hashtable.Add ("Spawn Wolf", Wolf);
		verbs_hashtable.Add ("Faster Wolf", Wolf);        // Done - Not Tested
		verbs_hashtable.Add ("Stronger Wolf", Wolf);      // Done - Not Tested

		verbs_tiergraph = new Dictionary<string, List<string>> ();
		verbs_available = new List<string> ();
		verbs_purchased = new List<string> ();

        verbs_purchased.Add("Craze Chicken");
        verbs_purchased.Add("Shrink Chicken");

		string line;
		StreamReader reader = new StreamReader ("Assets/Scripts/Scenarios/Verbs.txt", Encoding.Default);
		try {
			using (reader) {
				do {
					line = reader.ReadLine ();
					string[] inputs = new string[]{};
					if (line != null) {
						inputs = line.Split(new string[]{"#"}, System.StringSplitOptions.RemoveEmptyEntries);
					}
					string verb = "";
					if (inputs.Length > 0) {
						verb = inputs[0];
						List<string> unlocks = new List<string>();
						if (inputs.Length > 2)
							for (int i = 2; i < inputs.Length; ++i) 
								unlocks.Add(inputs[i]);
						verbs_tiergraph.Add(verb, unlocks);
						if (inputs[1].Equals("0")) verbs_available.Add (verb);
					}
				} while (line != null);

				reader.Close();
			}
		} catch (System.Exception e) {
			Debug.LogError (e.Message);
		}
	}

	// Use this for initialization
	void Start ()
	{
		//AP [0] = GameObject.Find ("AP_1").GetComponent<Image> ();
		//AP [1] = GameObject.Find ("AP_2").GetComponent<Image> ();
		//AP [2] = GameObject.Find ("AP_3").GetComponent<Image> ();
		//AP [3] = GameObject.Find ("AP_4").GetComponent<Image> ();
		//AP [4] = GameObject.Find ("AP_5").GetComponent<Image> ();
		//inactive_Img = AP [0].sprite;
		//active_Img = Resources.Load ("APfull", typeof(Sprite)) as Sprite;

		//SetAPFillSpeed ();
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
			case "rock":
				switch (effect) {
				case "spawn":      verb = "Spawn Rock";          break;
				} break;
			case "chicken":
				switch (effect) {
				case "craze":      verb = "Craze Chicken";       break;
				case "decoy":      verb = "Decoy Chicken";       break;
				case "faster":     verb = "Faster Chicken";      break;
				case "shrink":     verb = "Shrink Chicken";      break;
				case "super":      verb = "Super Chicken";       break;
				} break;
			case "hex":
				switch (effect) {
				case "ice":        verb = "Ice Hex";             break;
				case "lava":       verb = "Lava Hex";            break;
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
			if (verbs_tiergraph [s].Count > 0)
				foreach (string unlock in verbs_tiergraph[s])
					verbs_available.Add (unlock);
			UpdateStringReaderRegex (s);
		} else if (debug_on)
			Debug.Log ("Cannot purchase verb: " + s);
	}

	static void UpdateStringReaderRegex(string s) {
		switch (s) {
		case "Faster Bear":    StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "bear");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "faster");   break;
		case "Spawn Bear":     StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "bear");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "spawn");    break;
		case "Stronger Bear":  StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "bear");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "stronger"); break;
		case "Spawn Monster":  StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "monster");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "spawn");    break;
		case "Spawn Rock":     StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "rock");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "spawn");    break;
		case "Decoy Chicken":  StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "decoy");    break;
		case "Super Chicken":  StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "super");    break;
		case "Ice Hex":        StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "ice");      break;
		case "Lava Hex":       StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "lava");     break;  
		case "Lower Hex":      StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "lower");    break;
		case "Raise Hex":      StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "raise");    break;
		case "Wall Hex":       StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "wall");     break;
		case "Fall Tree":      StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "tree");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "fall");     break;
		case "Smite Tree":     StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "tree");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "smite");    break;
		case "Spawn Tree":     StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "tree");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "spawn");    break;
		case "Spawn Wolf":     StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "wolf");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "spawn");    break;
		case "Faster Wolf":    StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "wolf");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "faster");   break;
		case "Stronger Wolf":  StringReader.AddToRegex (ref StringReader.commandregex, ref StringReader.commandlist, "wolf");
			                   StringReader.AddToRegex (ref StringReader.modregex,     ref StringReader.modlist,     "stronger"); break;
		}
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
		GameObject[] bears = GameObject.FindGameObjectsWithTag("Bear"); //WorldContainer.GetAllInstances ("Bear");
		switch (effect) {
		case "faster":
			if (debug_on) Debug.Log ("Bear: effect = " + effect);
			if (bears.Length == 0) return 0;
			foreach (GameObject bear in bears) {
				bear.GetComponent<Animal> ().Rage ("faster");
				SpawnTwitchActionParticle (bear.transform.position);
			}
			return 1;
		case "stronger":
			if (debug_on) Debug.Log ("Bear: effect = " + effect);
			if (bears.Length == 0) return 0;
			foreach (GameObject bear in bears) {
				bear.GetComponent<Animal> ().Rage ("stronger");
				SpawnTwitchActionParticle (bear.transform.position);
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
			SpawnTwitchActionParticle(Hex.transform.position);
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
				SpawnTwitchActionParticle (chicken.transform.position);
			}
			return 1;
		case "faster":
			if (debug_on) Debug.Log ("Chicken: effect = " + effect);
			if (chickens.Length == 0) return 0;
			foreach (GameObject chicken in chickens) {
				chicken.GetComponent<Chicken> ().DoubleSpeed ();
				SpawnTwitchActionParticle (chicken.transform.position);
			}
			return 1;
		case "shrink":
			if (debug_on) Debug.Log ("Chicken: effect = " + effect);
			if (chickens.Length == 0) return 0;
			foreach (GameObject chicken in chickens) {
				chicken.GetComponent<Chicken> ().Shrink ();
				SpawnTwitchActionParticle (chicken.transform.position);
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
		case "decoy":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().state = HexState.DECOY;
			SpawnTwitchActionParticle (Hex.transform.position);
			return 1;
		case "ice":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().SwapIce ();
			SpawnTwitchActionParticle (Hex.transform.position);
			return 1;
		case "lava":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().SwapLava ();
			SpawnTwitchActionParticle(Hex.transform.position);
			return 1;
		case "lower":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().Lower ();
			SpawnTwitchActionParticle(Hex.transform.position);
			return 1;
		case "raise":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().Raise ();
			SpawnTwitchActionParticle(Hex.transform.position);
			return 1;
		case "spawn":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().SwapMonster ();
			SpawnTwitchActionParticle(Hex.transform.position);
			return 1;
		case "super":
			if (Hex == null || GameObject.FindGameObjectsWithTag("Torchick").Length > 0) return 0;
			Hex.GetComponent<HexControl> ().SwapTorchick();
			SpawnTwitchActionParticle (Hex.transform.position);
			return 1;
		case "wall":
			if (Hex == null) return 0;
			Hex.GetComponent<HexControl> ().Wall ();
			SpawnTwitchActionParticle(Hex.transform.position);
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
			SpawnTwitchActionParticle(Hex.transform.position);
			return 1;
		case "smite":
			if (trees.Length == 0) return 0;
			if (debug_on) Debug.Log ("Tree: effect = " + effect);
			if (hex.Equals ("random")) {
				GameObject[] Trees = GameObject.FindGameObjectsWithTag ("Tree");
				foreach (GameObject o in Trees) {
					Tree tree = o.GetComponent<Tree> ();
					if (!(tree.hasBurned || tree.hasFallen)) {
						tree.beginBurn ();
						SpawnTwitchActionParticle (tree.transform.position);
						return 1;
					}
				}
			}else foreach (Tree tree in trees) {
				if (!(tree.hasBurned || tree.hasFallen)) {
					tree.beginBurn ();
					SpawnTwitchActionParticle(tree.transform.position);
					return 1;
				}
			}
			return 0;
		case "spawn":
			if (debug_on) Debug.Log ("Tree: effect = " + effect);
			if (debug_on) Debug.Log ("Tree: Hex = " + hex + " " + Hex.name);
			Hex.GetComponent<HexControl> ().SwapTree ();
			SpawnTwitchActionParticle(Hex.transform.position);
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
				SpawnTwitchActionParticle(wolf.transform.position);
			}
			return 1;
		case "stronger":
			if (debug_on) Debug.Log ("Wolf: effect = " + effect);
			if (wolves.Length == 0) return 0;
			foreach (GameObject wolf in wolves) {
				wolf.GetComponent<Animal> ().Rage ("stronger");
				SpawnTwitchActionParticle(wolf.transform.position);
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
		SpawnTwitchActionParticle(Hex.transform.position);
		spawn.transform.SetParent (Hex.transform);
	}

	static void SpawnTwitchActionParticle(Vector3 where) {
		where += new Vector3 (0f, 15f, 0f);
		Instantiate (Resources.Load("TwitchAction"), where, Quaternion.identity);
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
