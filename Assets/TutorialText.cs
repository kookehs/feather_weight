using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialText : MonoBehaviour {

	public Text t;
	public GameObject arrow;

	void Awake(){
		arrow = GameObject.Find ("Tutorial_Arrow");
		arrow.SetActive (false);
	}

	// Use this for initialization
	void Start () {
		t = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ActivateArrow() {
		arrow.SetActive (true);
	}

	public void OnEnable() {
		InvokeRepeating ("ToggleText", .5f, 2f);
		InvokeRepeating ("ToggleArrow", .5f, 2f);
		StartCoroutine (WaitAndEndTextFlash ());
	}

	public void ToggleText() {
		if (t.text.Equals("")) {
			t.text = "ATTN:\nCHATTERS";
		}
		else t.text = "";
	}

	public void ToggleArrow() {
		if (arrow.GetComponent<Image> ().enabled == false)
			arrow.GetComponent<Image> ().enabled = true;
		else
			arrow.GetComponent<Image> ().enabled = false;
	}

	public IEnumerator WaitAndEndTextFlash() {
		yield return new WaitForSeconds (4.5f);
		CancelInvoke ();
		Destroy (arrow);
		Destroy (gameObject);
	}
}
