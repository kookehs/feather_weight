using UnityEngine;
using System.Collections;

public class PortalVid : MonoBehaviour {

	public MovieTexture movie;

	// Use this for initialization
	void Start () {
		movie.loop = true;
		movie.Play ();

		GetComponent<Material> ().mainTexture = movie as MovieTexture;
	}
}
