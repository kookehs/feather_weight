using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollingCredits : MonoBehaviour {

	public int speed = 1;
	public AudioSource audio;
	public Text credits;

	public void Start(){
		GameObject twitchHUD = GameObject.FindGameObjectWithTag ("TwitchData");

		//add extra diolag for when it is a gameover versus just viewing the credits
		if (twitchHUD != null && twitchHUD.GetComponent<EnterCredits> ().isGameOver) {
			credits.text = credits.text.Insert(0, twitchHUD.GetComponent<EnterCredits> ().extraGameOverDialogP+ '\n' + twitchHUD.GetComponent<EnterCredits> ().lineFeed + "\n\n");
			credits.text += ('\n' + twitchHUD.GetComponent<EnterCredits> ().lineFeed + "\n\n" + twitchHUD.GetComponent<EnterCredits> ().extraGameOverDialogT);
			twitchHUD.GetComponent<EnterCredits> ().isGameOver = false;
		}
	}

	public void Update(){
		transform.Translate ((Vector3.up) * speed * Time.deltaTime);
		StartCoroutine ("EndCredits");
	}

	IEnumerator EndCredits(){
		yield return new WaitForSeconds (audio.clip.length - 20.0f);

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
