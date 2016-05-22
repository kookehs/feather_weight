using UnityEngine;
using System.Collections;

public class DropCollectable : MonoBehaviour
{
	bool is_quitting = false;
	// Must initialize the following two arrays with the given setters in the object script
	string[] _collectables;
	double[] _drop_rates;

	public string[] collectables {
		set { _collectables = value; }
	}

	public double[] drop_rates {
		set { _drop_rates = value; }
	}

	void OnApplicationQuit() {
		is_quitting = true;
	}

	void OnDestroy() {
		if (!is_quitting) {
			/*
			for (int i = 0; i < _collectables.Length; ++i)
				if (WorldContainer.RandomChance () < _drop_rates [i])
					Instantiate (Resources.Load (_collectables [i]), gameObject.transform.position, Quaternion.identity);
					*/
		}
	}
}

