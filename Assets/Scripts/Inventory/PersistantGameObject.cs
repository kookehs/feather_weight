using UnityEngine;
using System.Collections;

public class PersistantGameObject : MonoBehaviour {

	private static PersistantGameObject instance = null;

	public static PersistantGameObject Instance {
		get { return instance; }
	}

	// Use this for initialization
	void Awake () {
		if ((instance != null && instance != this && instance.name == gameObject.name)) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}

		DontDestroyOnLoad (this.gameObject);
	}

	void OnLevelWasLoaded (){
		if (Application.loadedLevelName.Equals ("Credits"))
			Destroy (gameObject);
	}

	void OnApplicationQuit(){
		instance = null;
	}
}

//this codes works for finding other instances of an object and destroying it so that it won't exist more than once