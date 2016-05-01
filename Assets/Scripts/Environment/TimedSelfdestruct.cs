using UnityEngine;
using System.Collections;

public class TimedSelfdestruct : MonoBehaviour
{
	public float countdown;

	// Use this for initialization
	void Start ()
	{
		StartCoroutine (SelfDestruct ());
	}
	
	public IEnumerator SelfDestruct() {
		yield return new WaitForSeconds(countdown);
		Destroy (gameObject);
	}
}

