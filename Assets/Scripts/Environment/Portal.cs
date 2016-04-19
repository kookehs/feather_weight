using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
    public string next_level;

    private void
    OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Application.LoadLevel(next_level);
        }
    }
}
