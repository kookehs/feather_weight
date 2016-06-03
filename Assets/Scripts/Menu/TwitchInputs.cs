using UnityEngine;
using System.Collections;

public class TwitchInputs : MonoBehaviour {
	private static TwitchInputs instance = null;

	public static TwitchInputs Instance {
		get { return instance; }
	}

    public void Awake () {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}

		DontDestroyOnLoad (this.gameObject);
    }

	public void Start(){
		TwitchIRC.nickname = gameObject.GetComponent<SaveTwitchData> ().nickname;
		TwitchIRC.o_auth_token = gameObject.GetComponent<SaveTwitchData> ().o_auth_token;
	}

	public void Update(){
		if(TwitchIRC.channel_name != gameObject.GetComponent<SaveTwitchData> ().channel_name)
			TwitchIRC.channel_name = gameObject.GetComponent<SaveTwitchData> ().channel_name;
	}
}
