using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {

	public float health;

	// Use this for initialization
	void Start () {
		health = 100;
	}
	
	// Update is called once per frame
	void Update () {
	
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
