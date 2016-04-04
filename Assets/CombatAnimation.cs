using UnityEngine;
using System.Collections;

public class CombatAnimation : MonoBehaviour {

	public Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void endSwordAnim ()
	{
		Debug.Log ("endSwordAnim()");
		anim.SetBool ("sword", false);
	}

	public void endSpearAnim ()
	{
		Debug.Log ("endSpearAnim()");
		anim.SetBool ("spear", false);
	}
}
