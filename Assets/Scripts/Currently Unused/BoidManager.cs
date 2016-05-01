using UnityEngine;
using System.Collections;

public class BoidManager : MonoBehaviour
{

	GameObject[] boids;
	float distance = 4;
	float ruleThreeConstant = 8;
	float speed = 50;

	// Use this for initialization
	void Start ()
	{

		boids = GameObject.FindGameObjectsWithTag ("Boid");
	
	}
	
	// Update is called once per frame
	void Update ()
	{

		foreach (GameObject b in boids) {
			Vector3 v1 = RuleOne (b),
			v2 = RuleTwo (b),
			v3 = RuleThree (b),
			v4 = v1 + v2 + v3;
			v4.y = 0;

			Debug.Log (v4);
			b.GetComponent<Rigidbody> ().AddForce (v4*speed);
		}
			

	}

	//	Rule 1
	//		Takes a boid as an argument, returns a normalized direction vector toward the center of mass of the group.
	Vector3 RuleOne (GameObject me)
	{
		Vector3 result = Vector3.zero;

		foreach (GameObject b in boids) {
			if (b != me) {
				result += b.transform.position;
			}
		}

		//	This is the average position
		result = result / (boids.Length - 1);

		//	This is the direction vector toward the average position
		result = Vector3.Normalize (result - me.transform.position);

		return result;
	}

	//	Rule 2
	//		Takes a boid as an argument, returns an average "nudge me away from everybody" vector
	Vector3 RuleTwo (GameObject me)
	{
		Vector3 result = Vector3.zero;

		foreach (GameObject b in boids) {
			if (b != me) {
				if (Vector3.Distance (me.transform.position, b.transform.position) < distance) {
					result = result - (b.transform.position - me.transform.position);
				}
			}
		}
			
		return result;
	}

	//	Rule 3
	//		Takes a boid as an argument, returns a velocity nudge toward the average velocity of the group
	Vector3 RuleThree (GameObject me)
	{
		Vector3 result = Vector3.zero;

		foreach (GameObject b in boids) {
			if (b != me) {
				result = result + b.GetComponent<Rigidbody> ().velocity;
			}
		}

		result = result / (boids.Length - 1);

		result = (result - me.GetComponent<Rigidbody> ().velocity) / ruleThreeConstant;

		return result;
	}
}
