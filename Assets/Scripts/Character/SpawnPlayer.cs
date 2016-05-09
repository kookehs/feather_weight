using UnityEngine;
using System.Collections;

public class SpawnPlayer : MonoBehaviour {
        void Awake() {
            GameObject player = GameObject.Find("Player");

            if (player == null) {
                    player = (GameObject)Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity);
                    player.name = "Player";
            }

            player.transform.position = transform.position;
        }
}
