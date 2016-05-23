using UnityEngine;
using System.Collections;

public class BombChicken : Chicken
{
	void OnCollisionEnter(Collider other) {
	}

	public override bool receiveHit (Collider other, float damage, float knock_back_force, string hitter)
	{
		Destroy (gameObject);
		Instantiate (Resources.Load("Temp_Explosion"), new Vector3(transform.position.x, 3f, transform.position.z), Quaternion.identity);
		return true;
	}
}

