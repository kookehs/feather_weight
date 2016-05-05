using UnityEngine;
using System.Collections;

public class Campfire : MonoBehaviour {

	public bool isActive = false;
	public float distance = float.MaxValue;

	private GameObject player;

	public void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
	}

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Tree") {
            GameObject fire = gameObject.transform.GetChild(0).gameObject;

            if (fire.activeSelf) {
                WorldContainer the_world = GameObject.Find("WorldContainer").GetComponent<WorldContainer>();

                //if (WorldContainer.RandomChance(100) < 15)
                //    fire.SetActive(false);
            }
        }
    }

	public void CampDistance(){
		if(isActive)
			distance = Vector3.Distance (player.transform.position, transform.position);
		else
			distance = float.MaxValue;
	}
}
