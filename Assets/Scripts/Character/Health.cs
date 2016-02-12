using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float health = 100;

	public bool hungry = false;
	public bool thirsty = false;
	public float malnutritionLossInterval = 5;
	public float malnutritionTimer;

	// Use this for initialization
	void Start () {
		malnutritionTimer = malnutritionLossInterval;
	}
	
	// Update is called once per frame
	void Update () {

		if (hungry || thirsty) {
			malnutritionTimer -= Time.deltaTime;
			if (malnutritionTimer <= 0) {
				decreaseHealth ();
				malnutritionTimer = malnutritionLossInterval;
			}
		}

		if (health <= 0)
			Destroy (gameObject);
	
	}

	public void increaseHealth() {
		if (health >= 90)
			health = 100f;
		else
			health += 10f;
	}

	public void decreaseHealth() {
		if (health <= 10)
			health = 0f;
		else
			health -= 10f;
	}
		
}
