using UnityEngine;
using System.Collections;

public class DistancePoints : MonoBehaviour {

	public float isNearest = float.MaxValue;
	public bool pointUsed = false;
	
	public void SetPoint(float distance){
		//set point to the distance value it is away from the player
		isNearest = distance;
	}
}
