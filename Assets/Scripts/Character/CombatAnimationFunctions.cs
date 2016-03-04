using UnityEngine;
using System.Collections;

public class CombatAnimationFunctions : MonoBehaviour {

	Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void killSwordAnim() {
		anim.SetBool ("sword", false);
	}

	public void killSpearAnim() {
		anim.SetBool ("spear", false);
	}
}
