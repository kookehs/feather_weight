using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class ShopButtons : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {

	private RecipesDisplay recDisplay;
	public int keyCodeNum = -1;

	public void Start(){
		recDisplay = GameObject.Find ("RecipesContainer").GetComponent<RecipesDisplay>();
		transform.GetChild(0).GetComponent<Image> ().enabled = false;
		transform.GetChild(0).GetComponentInChildren<Text> ().enabled = false;
		Debug.Log ("Yiis");
	}
    
    public virtual void OnPointerDown(PointerEventData eventData) {
		recDisplay.ForButtonPress ();
    }

	public virtual void OnPointerEnter(PointerEventData eventData){
		Debug.Log (keyCodeNum);
		recDisplay.hoverItem (keyCodeNum);
	}

	public virtual void OnPointerExit (PointerEventData eventData)
	{
		recDisplay.ExitHoverItem ();
	}
}
