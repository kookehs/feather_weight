using UnityEngine;
using System.Collections;

public class LadderController : MonoBehaviour {
        private PlayerMovementRB player;
        public bool usable = false;

        private void
        Awake() {
            if (GameObject.Find("Player"))
                player = GameObject.Find("Player").GetComponent<PlayerMovementRB>();
        }

        public void
        Dismount(string position) {
            for (int i = 0; i < transform.childCount; ++i) {
                Transform child = transform.GetChild(i);

                if (child.tag == position + "Dismount")
                    player.transform.position = child.position;
                }
        }
}
