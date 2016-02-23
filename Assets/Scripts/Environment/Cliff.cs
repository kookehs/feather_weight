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
                Transform cliff_points = GameObject.Find("CliffPoints").transform;

                for (int i = 0; i < cliff_points.childCount; i++) {
                        Transform child = cliff_points.GetChild(i);
                        child.GetComponent<DistancePoints>().SetPoint(Vector3.Distance(player.transform.position, child.position));
                }
        }
}
