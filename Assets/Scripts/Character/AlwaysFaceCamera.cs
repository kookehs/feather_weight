using UnityEngine;
using System.Collections;

public class AlwaysFaceCamera : MonoBehaviour {

	public GameObject m_camera;

	// Use this for initialization
	void Start () {
		m_camera = GameObject.Find ("Camera");
		transform.LookAt (m_camera.transform);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
