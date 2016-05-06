using UnityEngine;
using System.Collections;

public class PersistantPlayer : MonoBehaviour {

	private Vector3 playerStart;

	public void Awake () {
		DontDestroyOnLoad (gameObject);
	}

	public void Start(){
		playerStart = transform.localPosition;
	}

	public void OnLevelWasLoaded(){
		if (Application.loadedLevelName.Equals ("Credits"))
			Destroy (gameObject);
		
		GameObject[] player = GameObject.FindGameObjectsWithTag ("Player");
		//destroy any extra player huds the one with data set has high chances for survival
		if (player.Length > 1) {
			for (int i = player.Length - 1; i > 0; i--) {
				if (player [i] != null) {
					Destroy (player [i]);
				}
			}
			if (player.Length > 1) {
				for (int i = player.Length - 1; i > 0; i--) {
					Destroy (player [i]);
					if (player.Length <= 1)
						break;
				}
			}
		}

		if (player [0] != null) {
			player [0].transform.localPosition = new Vector3(playerStart.x, 0, playerStart.z);
			if(Application.loadedLevelName.Equals ("ShopCenter"))
				player [0].transform.FindChild("Main Camera").gameObject.SetActive(false);
			else
				player [0].transform.FindChild("Main Camera").gameObject.SetActive(true);
		}
	}
}
