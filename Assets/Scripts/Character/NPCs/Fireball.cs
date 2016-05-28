using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
	Vector3 _height,
	        _start,
	        _destination,
	        max_height = new Vector3 (0, 9f, 0),
	        min_height = Vector3.zero;
	float   journey_loc = 0,
		    height_loc = 0.39444f,
	        journey_inc,
	        height_inc,
	        height_dec;
	bool is_quitting = false;
	bool going_up = true;

	// must call the function below to initialize start location and destination
	public void Initialize (Vector3 start, Vector3 destination) {
		_start = start;
		_destination = destination;
		_destination.y = 0;
		journey_inc = Time.deltaTime / 1f;
		height_dec = Time.deltaTime / .6f;
		height_inc = 0.60556f * Time.deltaTime / .4f;
		//Debug.Log (height_dec);
		journey_loc += journey_inc;
	}

	void OnApplicationQuit() {
		is_quitting = true;
	}

	void OnDestroy() {
		if (!is_quitting)
			Instantiate (Resources.Load ("ChickenSplosion"), transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
	}

	void Update() {
		if (journey_loc > 1) Destroy (gameObject);
		else {
			transform.position = Vector3.Lerp (_start, _destination, journey_loc);
			if (going_up) {
				_height = Vector3.Lerp (min_height, max_height, height_loc);
				height_loc = Mathf.Min (1, height_loc + height_inc);
				if (height_loc == 1)
					going_up = false;
			} else {
				_height = Vector3.Lerp (min_height, max_height, height_loc);
				height_loc = Mathf.Max (0, height_loc - height_dec);
			}
			//Debug.Log (height_loc + " " + _height + " " + journey_loc);
			transform.position += _height;
			journey_loc += journey_inc;
		}
	}
}

