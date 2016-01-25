using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
    private Vector3 offset;
    public float smoothing = 5.0f;
    public Transform target;

    private void
    Awake() {
        // Default camera offset is distance from camera to player
        offset = transform.position - target.position;
    }

    private void
    FixedUpdate() {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothing * Time.deltaTime);
    }
}
