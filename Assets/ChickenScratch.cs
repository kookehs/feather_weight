using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChickenScratch : Weapon {

    public GameObject scratchParticle;

    //  This is here because it has to be according to Weapon.cs
    protected override void OnTriggerEnter(Collider other)
    {

    }

    protected override void OnEnable()
    {
        GetComponent<Animator>().Play("sword_swing");
        Instantiate(scratchParticle, transform.position, Quaternion.identity);
        List<GameObject> chickens = WorldContainer.GetAllObjectsNearPlayer("Chicken");
        foreach (GameObject c in chickens)
        {
            c.GetComponent<Chicken>().ReactToScratch();
        }

    }
}
