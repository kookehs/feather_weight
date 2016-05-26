using UnityEngine;
using System.Collections;
using System.Linq;

public class Electric_Antenna : MonoBehaviour {

	private GameObject[] bearCollection;
	private GameObject[] wolfCollection;
	private GameObject[] animals;
	private GameObject hexIn;
	private bool shockWave = false;
	private int preNumCreatures = 0;
	
	// Update is called once per frame
	void Update () {
		if (shockWave) {
			//if a new creature is added to the world then get a new list of possible creatures
			if (WorldContainer.counts_tracker.CountCount() > preNumCreatures) {
				GetNewCreatures ();
				preNumCreatures = WorldContainer.counts_tracker.CountCount();
			}

			for (int i = 0; i < animals.Length; i++) {
				//check distance if in range do knockback stun
				if (Vector3.Distance (transform.position, animals [i].transform.position) < hexIn.transform.localScale.x) {
					animals [i].GetComponent<Animal> ().base_knockback = 10f;
					//have electric wave particle effect
				}
			}
		}
	}

	public void EnableElectric_Antenna (GameObject hex){
		//get all bears and wolves
		GetNewCreatures();
		shockWave = true;
		hexIn = hex;
		preNumCreatures = WorldContainer.counts_tracker.CountCount();
	}

	private void GetNewCreatures(){
		bearCollection = GameObject.FindGameObjectsWithTag ("Bear");
		wolfCollection = GameObject.FindGameObjectsWithTag ("Wolf");

		animals = bearCollection;
		if (wolfCollection != null && wolfCollection.Length > 0)
			animals.Concat (wolfCollection);
	}
}
