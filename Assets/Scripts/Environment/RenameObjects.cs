using UnityEngine;
using System.Collections;

public class RenameObjects : MonoBehaviour {

	// Use this for initialization
	void Start () {
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild (i).gameObject;
			//Vector3 childPos = child.transform.position;
			//Quaternion childRot = child.transform.rotation;

			//child = Instantiate (objType, child.transform.position, child.transform.rotation) as GameObject;
			//child = transform.GetChild (i).gameObject;
			child.name = "Flower (" + i + ")";
		}
	}
}
