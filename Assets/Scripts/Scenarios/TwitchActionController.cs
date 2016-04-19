using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TwitchActionController : MonoBehaviour
{
	static readonly int max_ap = 5;
	static int curr_ap = 0;
	static float ap_regen = 1000f;

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

		IncreaseAP (5);
		Do ("bear_powerup");
		InvokeRepeating ("IncreaseAP", 0, ap_regen);
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

	void IncreaseAP (int v) {
		while (v-- > 0) IncreaseAP ();
		Debug.Log (curr_ap);
	}

	void DecreaseAP () {
		if (curr_ap != 0) {
			AP [--curr_ap].color = inactive_clr;
			Debug.Log ("DecreaseAP");
			//TODO More OJ to satisfy Bill later
		}
	}

	void DecreaseAP (int v) {
		while (v-- > 0) DecreaseAP ();
		Debug.Log (curr_ap);
	}

	// command synopsis: target_command_name[_argument...]
	// --target: the target object, e.g. "chicken", or "hex" if hex-based
	// --command: the command we want to do
	// --name: name of the object, e.g. name of the chicken, or hex number if hex-based
	// --argument: any additional argument(s)
	public void Do(string command) {
		string[] argv = command.Split (cmd_separator, System.StringSplitOptions.RemoveEmptyEntries);
		switch (argv [0]) {
		case "bear":
			DecreaseAP (DoBear (argv));
			break;
		case "chicken":
			DecreaseAP (DoChicken (argv));
			break;
		default:
			Debug.Log ("Incorrect Command: " + command);
			break;
		}
	}

	int DoBear(string[] argv) {
		GameObject[] bears = WorldContainer.GetAllInstances ("Bear", WorldContainer._2D);
		if (bears != null && bears.Length > 0) {
			switch (argv [1]) {
			case "powerup":
				if (curr_ap >= 3)
					foreach (GameObject bear in bears)
						bear.GetComponent<BearNMA> ().rage ();
				return 3;
			default:
				break;
			}
		}
		return 0;
	}

	int DoChicken(string[] argv) {
		Chicken chicken = GameObject.Find (argv [2]).GetComponent<Chicken> ();
		switch (argv [1]) {
		case "speed":
			chicken.DoubleSpeed ();
			break;
		case "jump":
			chicken.Craze ();
			break;
		case "shrink":
			chicken.Shrink ();
			break;
		default:
			break;
		}
		return 0;
	}
}