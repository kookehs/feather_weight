using UnityEngine;
using System.Collections;

public class SpawnPlayer : MonoBehaviour {
        GameObject player;

        private void
        Awake() {
            player = GameObject.Find("Player");

            if (player == null) {
                    player = (GameObject)Instantiate(Resources.Load("Player"), Vector3.zero, Quaternion.identity);
                    player.name = "Player";
            }

            player.transform.position = transform.position;
            player.GetComponent<Rigidbody>().isKinematic = true;
            StartCoroutine("Kinematic");
        }

        private IEnumerator
        Kinematic() {
            yield return new WaitForSeconds(1.0f);
            player.GetComponent<Rigidbody>().isKinematic = false;
        }
}
