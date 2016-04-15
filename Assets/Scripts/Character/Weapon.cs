using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {

    }

    protected abstract void OnTriggerEnter(Collider other);

    protected virtual void OnEnable()
    {
        //  This is the old animation: The sword starts off vertical and swings downward
        //GetComponent<Animator> ().Play ("sword_swing");

        //  In this new animation, the sword simply lays flat and then disables itself in the last frame
        GetComponent<Animator>().Play("sword_swing_new");
    }

    protected void disableMe()
    {
        if (gameObject.layer.Equals(0))
            gameObject.SetActive(false);
    }
		
}
