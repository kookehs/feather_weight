using UnityEngine;
using UnityEditor;
using System.Collections;

public class RenameObjects : MonoBehaviour {

	public string nameSet = "";

	// Use this for initialization
	void Start () {
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild (i).gameObject;
			child.name = nameSet + " (" + i + ")";

			//Used for grass
			child.layer = LayerMask.NameToLayer ("Foliage");
			child.tag = "Grass";
			Destroy(child.GetComponent<MeshCollider>());
		}
	}
}