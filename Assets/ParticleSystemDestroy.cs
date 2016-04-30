using UnityEngine;
using System.Collections;

public class ParticleSystemDestroy : MonoBehaviour {

    public float duration = 1f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, duration);
	}
	
	// Update is called once per frame
	void Update () {
        	
	}
}
