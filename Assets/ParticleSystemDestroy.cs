using UnityEngine;
using System.Collections;

public class ParticleSystemDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ParticleSystem thisSys = GetComponent<ParticleSystem> ();
		Destroy (gameObject, thisSys.duration);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
