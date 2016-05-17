using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ScrollingCredits : MonoBehaviour {

	public int speed = 1;
	public AudioSource audio;
	public Text credits;
	public Button playAgain;

	public void Start(){
		GameObject twitchData = GameObject.FindGameObjectWithTag ("TwitchData");
		EventSystem.current.UpdateModules ();

		//add extra diolag for when it is a gameover versus just viewing the credits
		if (twitchData != null && twitchData.GetComponent<EnterCredits> ().isGameOver == 2) {
			credits.text = credits.text.Insert (0, twitchData.GetComponent<EnterCredits> ().extraGameOverDialogT + '\n' + twitchData.GetComponent<EnterCredits> ().lineFeed + "\n\n"); //insert at top

			credits.text += ('\n' + twitchData.GetComponent<EnterCredits> ().lineFeed + "\n\n" + twitchData.GetComponent<EnterCredits> ().extraGameOverDialogB); //insert at bottom

			twitchData.GetComponent<EnterCredits> ().isGameOver = 0;
			playAgain.gameObject.SetActive(true);
		} else if(twitchData != null && twitchData.GetComponent<EnterCredits> ().isGameOver == 1){
			if(twitchData.GetComponent<SaveTwitchData>().nickname != "")
				twitchData.GetComponent<EnterCredits> ().twitchGameOverDialogB = twitchData.GetComponent<EnterCredits> ().twitchGameOverDialogB.Replace("Streamer", twitchData.GetComponent<SaveTwitchData>().nickname);
			
			credits.text = credits.text.Insert (0, twitchData.GetComponent<EnterCredits> ().twitchGameOverDialogT + '\n' + twitchData.GetComponent<EnterCredits> ().lineFeed + "\n\n");
			credits.text += ('\n' + twitchData.GetComponent<EnterCredits> ().lineFeed + "\n\n" + twitchData.GetComponent<EnterCredits> ().twitchGameOverDialogB);
			twitchData.GetComponent<EnterCredits> ().isGameOver = 0;
			playAgain.gameObject.SetActive(true);
		}else if(twitchData != null){
			playAgain.gameObject.SetActive(false);
		}

		audio.Play ();
	}

	public void Update(){
		transform.Translate ((Vector3.up) * speed * Time.deltaTime);
		StartCoroutine ("EndCredits");
	}

	IEnumerator EndCredits(){
		float duration = audio.clip.length;
		if (duration > 65.0f)
			duration = 65.0f;
		
		yield return new WaitForSeconds (duration - 12.0f);

		//fade the music out at the end as it is a bit too long
		float audio1Volume = audio.volume;
		if(audio.volume > 0.1f)
		{
			audio1Volume -= 0.1f * Time.deltaTime;
			audio.volume = audio1Volume;
		}
		else
			Application.LoadLevel ("MenuScreen");
	}
}
