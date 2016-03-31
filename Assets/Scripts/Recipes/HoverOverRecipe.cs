using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HoverOverRecipe : MonoBehaviour {

	public float delay = 0.1f;

	private bool onMouseOver = false;

	private RecipesController recipeController;

	void Start(){
		if (GameObject.Find("RecipesContainer") != null)
			recipeController = GameObject.Find("RecipesContainer").GetComponent<RecipesController>();
	}

	void OnMouseEnter()
	{
		enabled = true;
		Debug.Log ("hover");
		StartCoroutine ("DisplayObjectName"); //delay before showing the object name
	}

	void OnMouseExit()
	{
		onMouseOver = false;
		enabled = false;
		//recipeController.ShowItemRequirements (null);
	}

	//to delay display of the object name
	IEnumerator DisplayObjectName(){
		yield return new WaitForSeconds(delay);
		onMouseOver = true;
		int s = 0;
		if(int.TryParse(transform.GetComponentInChildren<Text>().text.ToString(), out s))
			recipeController.ShowItemRequirements(recipeController.currentlySelected);
	}
}
