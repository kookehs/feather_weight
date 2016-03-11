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
				decreaseHealth (10f);
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
                                if (gameObject.tag.Equals ("Boss")) {
                                        GameObject.FindGameObjectWithTag("TwitchData").GetComponent<EnterCredits>().isGameOver = true;
                                        Application.LoadLevel("Credits");
                                }

				Destroy (gameObject);
			}
		}

	}

	public void increaseHealth ()
	{
		if (health >= 90)
			health = 100f;
		else
			health += 10f;
	}

	public void decreaseHealth (float damage)
	{
		health = Mathf.Max (0f, health - damage);
	}

	public bool isDead ()
	{
		return health <= 0;
	}

	IEnumerator GameOver (int duration)
	{
		yield return new WaitForSeconds (duration);
		Application.LoadLevel ("PlayerDeath");
	}
}
