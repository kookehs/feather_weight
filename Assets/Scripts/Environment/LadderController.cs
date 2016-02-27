using UnityEngine;
using System.Collections;

public class LadderController : MonoBehaviour {
        private PlayerMovementRB player;

        private void
        Awake() {
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
