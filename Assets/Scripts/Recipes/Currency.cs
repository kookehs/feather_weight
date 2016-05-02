using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Currency : MonoBehaviour {

	public int currency = 0;

	// Use this for initialization
	void Start () {
		GetComponentInChildren<Text> ().text = currency.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		GetComponentInChildren<Text> ().text = currency.ToString ();

		if (Input.GetKeyUp(KeyCode.Space))
			Application.LoadLevel ("ShopCenter");
	}
}
