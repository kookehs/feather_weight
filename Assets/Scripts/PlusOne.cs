using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlusOne : MonoBehaviour {

    public Text myText;
    public float speed = 1;
    public float goalDistance = 60;
    public float initialY;

	// Use this for initialization
	void Start () {

        initialY = transform.position.y;
        myText = GetComponent<Text>();
	
	}
	
	// Update is called once per frame
	void Update () {
        /*if (gameObject.activeSelf == true)
        {
            if (transform.position.y <= initialY + goalDistance)
            {
                transform.Translate(new Vector3(0, speed, 0));
            }
            else
            {
                StartCoroutine(FadeOut());
            }
        }*/
        //transform.Translate(new Vector3(0, 1, 0));
	}

    void changeText(string s)
    {
        myText.text = s;
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(.5f);
        transform.Translate(new Vector3(0, -goalDistance, 0));
        gameObject.SetActive(false);

    }
}
