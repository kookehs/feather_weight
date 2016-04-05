using UnityEngine;
using System.Collections;

public class PlayerHealth : Health
{
	private FoodLevel nutrition;
	private Hydration hydration;

	// Use this for initialization
	protected override void Initialize() {
		gui.maxValue = 100f;
		value = 100f;
		loss_timer = 5f;
		loss_frequency = 5f;
		nutrition = GetComponent<FoodLevel> ();
		hydration = GetComponent<Hydration> ();
		anim = GameObject.Find ("PlayerSprite").GetComponent<Animator> ();
	}

	// Update is called once per frame
	protected override void UpdateRoutine()
	{
		if (nutrition.IsZero () || hydration.IsZero ())
			_loss_over_time = true;
		else 
			_loss_over_time = false;
		if (IsZero()) OnDeath ();
	}

	protected override void OnDeath() {
		GetComponentInParent<PlayerMovementRB> ().enabled = false;
		if (anim != null) {
			anim.SetBool ("isDead", true);
			AnimatorStateInfo current_state = anim.GetCurrentAnimatorStateInfo (0);

			if (current_state.nameHash == Animator.StringToHash ("Base Layer.death")) {
				StartCoroutine ("GameOver", current_state.length);
				Destroy (gameObject, current_state.length);
			}
		}
	}

	IEnumerator GameOver (int duration)
	{
		yield return new WaitForSeconds (duration);
		Application.LoadLevel ("PlayerDeath");
	}
}

