using UnityEngine;
using System.Collections;

public class Cliff : MonoBehaviour {
    private GameObject player;

    private void
    Awake() {
        player = GameObject.Find("Player");
    }

    private void
    Update() {
        if (player == null)
            return;

        GameObject[] points = GameObject.FindGameObjectsWithTag("CliffPoint");

        foreach (GameObject obj in points) {
            Vector3 player_position = player.transform.position;
            player_position.y -= player.GetComponent<BoxCollider>().size.y * 0.5f;
            obj.GetComponent<DistancePoints>().SetPoint(Vector3.Distance(player_position, obj.transform.position));
        }
    }
}
