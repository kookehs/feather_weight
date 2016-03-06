using UnityEngine;
using System.Collections;

public class UIActions : MonoBehaviour {

	public void StartGame(){
		Application.LoadLevel ("final_world");
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
