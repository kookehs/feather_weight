using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class InputFocus : MonoBehaviour {

	public GameObject channel;
	public GameObject nickname;
	public GameObject autho;

	public GameObject playButton;
	public GameObject initialButtons;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Tab) || Input.GetKeyUp (KeyCode.Return)) {
			GameObject currentlySelectedInput = EventSystem.current.currentSelectedGameObject;

			if (currentlySelectedInput != null) {
				switch (currentlySelectedInput.name) {
					case "Channel":
						EventSystem.current.SetSelectedGameObject (nickname, null);
						break;
					case "Nickname":
						EventSystem.current.SetSelectedGameObject (autho, null);
						break;
					case "Autho":
						EventSystem.current.SetSelectedGameObject (playButton, null);
						break;
					default:
						EventSystem.current.SetSelectedGameObject (channel, null);
						break;
				}
			}
		}

		if (Input.GetKeyUp (KeyCode.Escape)) {
			initialButtons.GetComponent<CanvasGroup> ().alpha = 1.0f;
			initialButtons.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			initialButtons.GetComponent<CanvasGroup> ().interactable = true;

			transform.GetComponent<CanvasGroup> ().alpha = 0.0f;
			transform.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			transform.GetComponent<CanvasGroup> ().interactable = false;
		}
	}
}
