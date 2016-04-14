using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIActions : MonoBehaviour {
	
	public GameObject buttons;
	public GameObject inputs;
	public GameObject[] inputFieldText;
	public Text loading;

	private GameObject twitch;

	private bool authoGiven = false;
	private bool channelGiven = false;

	public void StartGame(){
		//add here passing in values to save user inuppt data
		//make sure feilds are filled out before allowing game play
		if (twitch != null && twitch.GetComponent<SaveTwitchData> ().nickname.Equals (string.Empty))
			twitch.GetComponent<SaveTwitchData> ().nickname = twitch.GetComponent<SaveTwitchData> ().channel_name;
		
		if (authoGiven && channelGiven) {
			loading.GetComponent<CanvasGroup> ().alpha = 1;
			Application.LoadLevel ("final_world");
		}
	}

	public void StartGameDifScene(){
		Application.LoadLevel ("final_world");
	}

	public void EnableEditables(){
		twitch = GameObject.FindGameObjectWithTag ("TwitchData");

		//turn off canvasgroup
		buttons.GetComponent<CanvasGroup> ().alpha = 0.0f;
		buttons.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		buttons.GetComponent<CanvasGroup> ().interactable = false;

		//turn on canvasgroup
		inputs.GetComponent<CanvasGroup> ().alpha = 1.0f;
		inputs.GetComponent<CanvasGroup> ().blocksRaycasts = true;
		inputs.GetComponent<CanvasGroup> ().interactable = true;

		if(twitch.GetComponent<SaveTwitchData> ().channel_name != "") inputs.transform.GetChild (0).GetComponent<InputField> ().text = twitch.GetComponent<SaveTwitchData> ().channel_name;
		if(twitch.GetComponent<SaveTwitchData> ().channel_name != "") inputs.transform.GetChild (1).GetComponent<InputField> ().text = twitch.GetComponent<SaveTwitchData> ().nickname;
		if(twitch.GetComponent<SaveTwitchData> ().channel_name != "") inputs.transform.GetChild (2).GetComponent<InputField> ().text = twitch.GetComponent<SaveTwitchData> ().o_auth_token;
	}

	public void SetChannelName(InputField channel){
		//set the name
		twitch.GetComponent<SaveTwitchData>().channel_name = channel.text;
		if(!channel.text.Equals(string.Empty))
			channelGiven = true;
	}

	public void SetAuthoName(InputField autho){
		//set autho
		twitch.GetComponent<SaveTwitchData>().o_auth_token = autho.text.ToString();
		if(!autho.ToString().Equals(string.Empty))
			authoGiven = true;
	}

	public void SetNickName(InputField nickname){
		//set nickname if one is not given then channel_name is used
		twitch.GetComponent<SaveTwitchData>().nickname = nickname.text.ToString();
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
