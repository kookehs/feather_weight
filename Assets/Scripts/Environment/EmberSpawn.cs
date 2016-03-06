using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EmberSpawn : MonoBehaviour {

    public GameObject ember;
    public WorldContainer the_world;

	// Use this for initialization
	void Start () {
        the_world = GameObject.Find("WorldContainer").GetComponent<WorldContainer>();
		fireEmber();
		fireEmber();
		fireEmber();
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
		int randomX = the_world.RandomChance(200,600);
        int randomY = the_world.RandomChance(500,750);
		int randomZ = the_world.RandomChance (200, 600);
        if (randomX % 2 == 0) randomX = -randomX;
        if (randomZ % 2 == 0) randomZ = -randomZ;
		Vector3 randomForce = new Vector3 (randomX, randomY, randomZ);
		Debug.Log (randomForce);
        e.GetComponent<Rigidbody>().AddForce(randomForce);
        Destroy(e, 1f);
    }
}
