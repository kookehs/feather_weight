using UnityEngine;
using System.Collections;

public class ScrollingCredits : MonoBehaviour {

	public int speed = 1;

	public void Update(){
		//float yMax = GetComponent<RectTransform> ().rect.yMax;
		//GetComponent<RectTransform> ().rect.yMax = yMax * Time.deltaTime;

		//if(GetComponent<RectTransform> ().rect.yMax == 315) Application.LoadLevel ("MenuScreen");
		transform.Translate ((Vector3.up) * speed * Time.deltaTime);

		StartCoroutine ("EndCredits");
	}

	IEnumerator EndCredits(){
		yield return new WaitForSeconds (22.0f);
		Application.LoadLevel ("MenuScreen");
	}
}
