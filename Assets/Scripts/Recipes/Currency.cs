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

		if (Input.anyKey)
			Application.LoadLevel ("Credits");
	}

	void OnDestroy(){
		if (Application.loadedLevelName.Equals("ShopCenter")) {
			transform.parent.transform.SetParent(GameObject.Find ("PlayerUICurrent").transform);
		}
	}
}
