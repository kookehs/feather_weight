﻿using UnityEngine;
using System.Collections;

public class WaterPoint : MonoBehaviour
{
	bool _player_is_near;

	public bool player_is_near {
		get { return _player_is_near;  }
		set { _player_is_near = value; }
	}

	void Awake() {
		_player_is_near = false;
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag.Equals("Player")) _player_is_near = true;
	}

	void OnTriggerExit(Collider other) {
		if (other.tag.Equals("Player")) _player_is_near = false;
	}
}
