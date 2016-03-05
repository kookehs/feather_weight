using UnityEngine;
using System.Collections;

public class UIActions : MonoBehaviour {

	public void StartGame(){
		Application.LoadLevel ("final_world");
	}

	public void Quit(){
		Application.Quit ();
	}
}
