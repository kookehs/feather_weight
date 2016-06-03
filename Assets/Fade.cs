using UnityEngine;
using System.Collections;

// Thanks to Brackeys, who offered this script free of charge in their 2D Asset pack.
//	youtube.com/watch?v=0HwZQt94uHQ

public class Fade : MonoBehaviour {

	public Texture2D fadeOutTexture; // texture that overlays the screen
	public float fadeSpeed = 0.8f; // the fading speed

	private int drawDepth = -1000; // the texture's order in the draw hierarchy: a low number means it renders on top
	private float alpha = 1.0f; // the texture's alpha value between 0 and 1
	private int fadeDir = -1; // the direction to fade: in = -1, out = 1

	void OnGUI() {
		// Fade out/in the alpha value using a direction, a speed and a Time.deltaTime to convert the operation to seconds
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		// force(clamp) the number between 0 and 1 because GUI.color uses alpha values between 0 and 1
		alpha = Mathf.Clamp01(alpha);

		// set color of our GUI (in this case our texture). All color values remain the same & the Alpha is set to the alpha variable
		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

	// sets fadeDir to the direction parameter making the scene fade in if -1 and out if 1
	public float BeginFade(int direction) {
		fadeDir = direction;
		return (fadeSpeed);
	}

	// OnLevelWasLoaded is called when a level is loaded. It takes loaded level index (int) as a parameter so you can limit the fade in to certain scenes
	void OnLevelWasLoaded () {
		// alpha = 1;	// use this if the alpha is not set to 1 by default
		BeginFade(-1);	// call the fade in function
	}
}
