using UnityEngine;
using System.Collections;

public class AlwaysFaceCamera : MonoBehaviour {

	public GameObject mCamera;

	// Use this for initialization
	void Start () {
		mCamera = GameObject.Find ("Camera");
	}
	
	// Update is called once per frame
	void Update () {

		transform.LookAt (mCamera.transform);
	
	}
}
