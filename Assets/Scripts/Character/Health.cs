using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{

	public float health = 100;

	public bool hungry = false;
	public bool thirsty = false;
	public float malnutritionLossInterval = 5;
	public float malnutritionTimer;

	private Animator anim;

	// Use this for initialization
	void Start ()
	{
		anim = GetComponent<Animator> ();
		malnutritionTimer = malnutritionLossInterval;
	}

	// Update is called once per frame
	void Update ()
	{
		if (hungry || thirsty) {
			malnutritionTimer -= Time.deltaTime;
			if (malnutritionTimer <= 0) {
				Decrease (10f);
				malnutritionTimer = malnutritionLossInterval;
			}
		}

		if (health <= 0) {
			if (anim != null) {
				anim.SetBool ("isDead", true);
				AnimatorStateInfo current_state = anim.GetCurrentAnimatorStateInfo (0);

				if (current_state.nameHash == Animator.StringToHash ("Base Layer.death")) {
					if (gameObject.tag.Equals ("Player"))
						StartCoroutine ("GameOver", current_state.length);
					
					Destroy (gameObject, current_state.length);
				}
			} else {
				Destroy (gameObject);
			}
		}

	}

	public void Increase ()
	{
		health = Mathf.Min (100f, health + 10f);
	}

	public void Increase (float d)
	{
		health = Mathf.Min (100f, health + d);
	}

	public void Decrease ()
	{
		health = Mathf.Max (0f, health - 10f);
	}

	public void Decrease (float d)
	{
		health = Mathf.Max (0f, health - d);
	}

	public bool IsDead ()
	{
		return health <= 0;
	}

	IEnumerator GameOver (int duration)
	{
		yield return new WaitForSeconds (duration);
		Application.LoadLevel ("PlayerDeath");
	}
}
