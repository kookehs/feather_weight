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
        GameObject[] points = GameObject.FindGameObjectsWithTag("CliffPoint");

        foreach (GameObject obj in points) {
            obj.GetComponent<DistancePoints>().SetPoint(Vector3.Distance(player.transform.position, obj.transform.position));
        }
    }
}
