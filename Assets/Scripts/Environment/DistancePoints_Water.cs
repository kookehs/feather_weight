using UnityEngine;
using System.Collections;

public class DistancePoints_Water : DistancePoints
{
	private bool _is_bridge_point = false;

	public bool is_bridge_point {
		get { return _is_bridge_point;  }
		set { _is_bridge_point = value; }
	}
}

