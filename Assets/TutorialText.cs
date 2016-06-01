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
		StartCoroutine (WaitAndEndTextFlash ());
	}

	public IEnumerator WaitAndEndTextFlash() {
		yield return new WaitForSeconds (4.5f);
		CancelInvoke ();
		Destroy (arrow);
		Destroy (gameObject);
	}
}
