﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TwitchActionController : MonoBehaviour
{
	static readonly int max_ap = 5;
	static int curr_ap = 0;

	static Image[] AP = new Image[max_ap];
	static readonly Color inactive_clr = new Color (1f, 1f, 1f, 1f);
	static readonly Color active_clr   = new Color(0.392f, 0.255f, 0.647f, 1f);

	static string[] cmd_separator = { "_" };

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

		InvokeRepeating ("IncreaseAP", 0, 5f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnApplicationQuit() {
		CancelInvoke ();
	}

	void IncreaseAP () {
		if (curr_ap + 1 <= max_ap) {
			AP [curr_ap++].color = active_clr;
			//TODO More OJ to satisfy Bill later
		}
	}

	void DecreaseAP () {
		if (curr_ap != 0) {
			AP [--curr_ap].color = inactive_clr;
			//TODO More OJ to satisfy Bill later
		}
	}

	void DecreaseAP (int v) {
		while (v-- > 0) DecreaseAP ();
	}

	// command synopsis: target_command_name[_argument...]
	// --target: the target object, e.g. "chicken", or "hex" if hex-based
	// --command: the command we want to do
	// --name: name of the object, e.g. name of the chicken, or hex number if hex-based
	// --argument: any additional argument(s)
	public int Do(string command) {
		string[] argv = command.Split (cmd_separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (argv [0]) {
		case "chicken":
			DoChicken (argv);
			return 0;
		default:
			Debug.Log ("Incorrect Command: " + command);
			return 0;
		}
	}

	void DoChicken(string[] argv) {
		Chicken chicken = GameObject.Find (argv [2]).GetComponent<Chicken> ();
		switch (argv [1]) {
		case "speed":
			chicken.DoubleSpeed ();
			break;
		case "jump":
			chicken.Craze ();
			break;
		case "shrink":
			//TODO chicken.Shrink();
		default:
			break;
		}
	}
}