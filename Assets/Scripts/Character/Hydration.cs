using UnityEngine;
using System.Collections;

public class Hydration : MonoBehaviour {

	public float hydration;
	
	// Use this for initialization
	void Start () {
		hydration = 100;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void increasehydration() {
		if (hydration >= 90)
			hydration = 100f;
		else
			hydration += 10f;
	}
	
	public void decreasehydration() {
		if (hydration <= 10)
			hydration = 0f;
		else
			hydration -= 10f;
	}
		
}
