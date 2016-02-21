using UnityEngine;
using System.Collections;

public class PlayerFallen : MonoBehaviour {

        private WorldContainer the_world;

        void Start() {
                the_world = GameObject.Find("WorldContainer").GetComponent<WorldContainer>();
        }

	void OnTriggerEnter(Collider obj){
		if(obj.tag != "Ground")
			the_world.Remove(obj.gameObject);

		//Reload game for now till a failed game scene is made
		if (obj.tag == "Player")
			Application.LoadLevel (Application.loadedLevel);
	}
}
