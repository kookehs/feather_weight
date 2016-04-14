using UnityEngine;
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

	// command synopsis: command_hex[_argument...]
	public void Do(string command) {
		string[] argv = command.Split (cmd_separator, System.StringSplitOptions.RemoveEmptyEntries);
	}
}

