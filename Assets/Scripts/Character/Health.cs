using UnityEngine;
using System.Collections;

public class Health : SurvivalStat
{
	protected Animator anim;

	// Use this for initialization
	public void Start ()
	{
		anim = GetComponent<Animator> ();
		Initialize ();
	}

	// Update is called once per frame
	protected override void UpdateRoutine ()
	{
		if (IsZero()) OnDeath ();
	}

	protected virtual void Initialize() {
	}

	protected virtual void OnDeath() {
		if (anim != null) {
			anim.SetBool ("isDead", true);
			AnimatorStateInfo current_state = anim.GetCurrentAnimatorStateInfo (0);

			if (current_state.nameHash == Animator.StringToHash ("Base Layer.death")) {
				Destroy (gameObject, current_state.length);
			}
		} else {
			if (gameObject.tag.Equals ("Boss")) {
				GameObject.FindGameObjectWithTag("TwitchData").GetComponent<EnterCredits>().isGameOver = true;
				Application.LoadLevel("Credits");
			}
			Destroy (gameObject);
		}
	}
}
