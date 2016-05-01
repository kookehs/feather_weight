using UnityEngine;
using System.Collections;

public class TimedSelfdestruct : MonoBehaviour
{
	public float countdown;

	// Use this for initialization
	void Start ()
	{
		Destroy (gameObject, countdown);
	}
}

