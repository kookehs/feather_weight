using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIActions : MonoBehaviour {
	
	public GameObject buttons;
	public GameObject inputs;
	public Text loading;

	private GameObject twitch;

	private bool authoGiven = false;
	private bool channelGiven = false;

	public void StartGame(){
		//add here passing in values to save user inuppt data
		//make sure feilds are filled out before allowing game play
		if (twitch.GetComponent<TwitchIRC> ().nickname.Equals (string.Empty))
			twitch.GetComponent<TwitchIRC> ().nickname = twitch.GetComponent<TwitchIRC> ().channel_name;
		
		if (authoGiven && channelGiven) {
			loading.GetComponent<CanvasGroup> ().alpha = 1;
			Application.LoadLevel ("final_world");
		}
	}

	public void EnableEditables(){
		twitch = GameObject.FindGameObjectWithTag ("TwitchData");
		if (!twitch.GetComponent<TwitchIRC> ().o_auth_token.Equals (string.Empty) && !twitch.GetComponent<TwitchIRC> ().channel_name.Equals (string.Empty)) {
			authoGiven = true;
			channelGiven = true;
			StartGame ();
		} else {
			//turn off canvasgroup
			buttons.GetComponent<CanvasGroup> ().alpha = 0.0f;
			buttons.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			buttons.GetComponent<CanvasGroup> ().interactable = false;

			//turn on canvasgroup
			inputs.GetComponent<CanvasGroup> ().alpha = 1.0f;
			inputs.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			inputs.GetComponent<CanvasGroup> ().interactable = true;
		}
	}

	public void SetChannelName(InputField channel){
		//set the name
		twitch.GetComponent<TwitchIRC>().channel_name = channel.text;
		if(!channel.text.Equals(string.Empty))
			channelGiven = true;
	}

	public void SetAuthoName(InputField autho){
		//set autho
		twitch.GetComponent<TwitchIRC>().o_auth_token = autho.text.ToString();
		if(!autho.ToString().Equals(string.Empty))
			authoGiven = true;
	}

	public void SetNickName(InputField nickname){
		//set nickname if one is not given then channel_name is used
		twitch.GetComponent<TwitchIRC>().nickname = nickname.text.ToString();
	}

	public void MenuScreen(){
		Application.LoadLevel ("MenuScreen");
	}

	public void Credits(){
		Application.LoadLevel ("Credits");
	}

	public void Quit(){
		Application.Quit ();
	}
}
