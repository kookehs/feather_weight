using UnityEngine;
using System.Collections;

public class Cliff : MonoBehaviour {
    private GameObject player;
    private GameObject[] points;

    private void
    Awake() {
        player = GameObject.Find("Player");
        points = GameObject.FindGameObjectsWithTag("CliffPoint");
    }

    private void
    Update() {
        if (player == null)
            return;

        foreach (GameObject obj in points) {
            Vector3 player_position = player.transform.position;
            player_position.y -= player.GetComponent<BoxCollider>().size.y * 0.5f;
            obj.GetComponent<DistancePoints>().SetPoint(Vector3.Distance(player_position, obj.transform.position));
        }
    }
}
