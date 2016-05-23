using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
	Vector3 dy = new Vector3 (0, 1f, 0),
	        _height,
	        _start,
	        _destination;
	float   journey_loc = 0,
	        journey_inc;
	bool is_quitting = false;

	// must call the function below to initialize start location and destination
	public void Initialize (Vector3 start, Vector3 destination) {
		_start = start;
		_destination = destination;
		journey_inc = Time.deltaTime / 3f;
		journey_loc += journey_inc;
	}

	void OnApplicationQuit() {
		is_quitting = true;
	}

	void OnDestroy() {
		if (!is_quitting)
			Instantiate (Resources.Load ("Temp_Explosion"), transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
	}

	void Update() {
		if (journey_loc > 1) Destroy (gameObject);
		else {
			transform.position = Vector3.Lerp (_start, _destination, journey_loc);
			if (journey_loc <= 0.47f)
				_height += dy;
			else
				_height -= dy;
			transform.position += _height;
			journey_loc += journey_inc;
		}
	}
}

