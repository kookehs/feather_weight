using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//[RequireComponent (typeof(AudioSource))]
public class Video : MonoBehaviour {

	public MovieTexture movie;
	//private AudioSource audio;

	// Use this for initialization
	void Start () {
		GetComponent<RawImage> ().texture = movie as MovieTexture;
		//audio = GetComponent<AudioSource> ();
		//audio.clip = movie.audioClip;
		movie.loop = true;
		movie.Play ();
		//audio.Play ();
	}
}
