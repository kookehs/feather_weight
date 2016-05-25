using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmberSpawn : MonoBehaviour {

    public GameObject ember;

	// Use this for initialization
	void Start () {
		fireEmber();
		fireEmber();
		fireEmber();
		fireEmber ();
		fireEmber ();
		fireEmber ();
		fireEmber ();
		fireEmber ();
		fireEmber ();
		fireEmber ();
		Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void fireEmber()
    {
        GameObject e = (GameObject)Instantiate(ember, transform.position, Quaternion.identity) as GameObject;
		int randomX = WorldContainer.RandomChance(200,600);
        int randomY = WorldContainer.RandomChance(500,750);
		int randomZ = WorldContainer.RandomChance (200, 600);
        if (randomX % 2 == 0) randomX = -randomX;
        if (randomZ % 2 == 0) randomZ = -randomZ;
		Vector3 randomForce = new Vector3 (randomX, randomY, randomZ);
        e.GetComponent<Rigidbody>().AddForce(randomForce);
        Destroy(e, 1f);
    }
}
