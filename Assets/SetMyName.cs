using UnityEngine;
using System.Collections;

public class SetMyName : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame

	void Update ()
	{
		GetComponent<TextMesh> ().text = transform.parent.name;
		transform.LookAt (Camera.main.transform);
		transform.Rotate (new Vector3 (0, 180, 0));
	}
		
}
