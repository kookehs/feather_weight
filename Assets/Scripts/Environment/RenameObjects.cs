using UnityEngine;
using UnityEditor;
using System.Collections;

public class RenameObjects : MonoBehaviour {

	//public GameObject prefab;
	public string nameSet = "";
	public GameObject prefab;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild (i).gameObject;
			child.name = nameSet + " (" + i + ")";
			//child.layer = LayerMask.NameToLayer ("Foliage");
			//child.tag = "Grass";
			//Destroy(child.GetComponent<MeshCollider>());
			//GameObject repoS = Instantiate (prefab, child.transform.position, child.transform.rotation) as GameObject;
			//repoS.transform.localScale = child.transform.localScale;
			//repoS.name = "Flower (" + i + ")";
			//Destroy (child);
			//set a new instantce of a prefab to position of the child
			//destroy the child
			//PrefabUtility.ReconnectToLastPrefab(repoS);
		}
	}
}