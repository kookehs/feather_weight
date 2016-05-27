using UnityEngine;
using System.Collections;

public class Chicken_Tutorial : Chicken {

	private bool beenHit = false;

	// Use this for initialization
	void Start () {
		base.Start ();
		gameObject.name = "HIT ME";
	}
	
	public override IEnumerator WaitAndEnableCollection ()
	{
		if (beenHit == false) {
			Destroy(transform.FindChild ("Mouse").gameObject);
			beenHit = true;
		}
		gameObject.name = "COLLECT ME";
		return base.WaitAndEnableCollection ();
	}

	public override void DisableCollection ()
	{
		base.DisableCollection ();
		gameObject.name = "HIT ME";
	}


}
