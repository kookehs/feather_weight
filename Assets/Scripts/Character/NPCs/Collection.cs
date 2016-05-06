using UnityEngine;
using System.Collections;

public class Collection : MonoBehaviour
{

	public float delay = 0.1f;
	public Color defaultCol;
	public Behaviour halo;

	private bool playerNearObject = false;
	public bool onMouseOver = false;

	private PlayerMovementRB player;
	private InventoryController inventoryController;
	private WeaponController wc;
	private Camera camera;

	void Start ()
	{
		GameObject playerObj = GameObject.FindGameObjectWithTag ("Player");

		if (playerObj != null) {
			player = playerObj.GetComponent<PlayerMovementRB> ();
			wc = playerObj.GetComponentInChildren<WeaponController> ();
		}

		if (GameObject.FindGameObjectWithTag ("InventoryUI") != null)
			inventoryController = GameObject.FindGameObjectWithTag ("InventoryUI").GetComponent<InventoryController> ();

		if (gameObject.tag != "River") {
			if (GetComponentInChildren<SpriteRenderer> () != null)
				defaultCol = GetComponentInChildren<SpriteRenderer> ().color;
			else
				defaultCol = GetComponent<Renderer> ().material.color;
		}

		camera = Camera.main;
	}

	void OnGUI ()
	{
		if (player == null)
			return;
		// the following line should be optimized about 10% cpu usage
		//display the objects name when time has been reached
		// Debug.Log(name);
		// string regex_name = name.Split (new string[] {"("," "}, System.StringSplitOptions.RemoveEmptyEntries)[0];
		string regex_name = name.Split (' ') [0];
		if (onMouseOver) {
			GUI.Box (new Rect (Event.current.mousePosition.x - 55, Event.current.mousePosition.y, 50, 25), regex_name);
		}

		if (Vector3.Distance (transform.position, player.transform.position) < 5f) {
			playerNearObject = true;
		} else {
			playerNearObject = false;
		}
	}

	void OnMouseEnter ()
	{
		if (gameObject.tag != "Chicken" || (gameObject.tag == "Chicken" && gameObject.GetComponent<Chicken>().IsPickupStunned())) {
			wc.hovering = true;
			enabled = true;
			camera.GetComponent<CollectionCursor> ().SetHover ();

			if (gameObject.tag != "River") {
				if (GetComponentInChildren<SpriteRenderer> () != null)
					GetComponentInChildren<SpriteRenderer> ().color = Color.red;
				else {
					GetComponent<Renderer> ().material.color = Color.red;//GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.005f);
				}
				if (halo != null)
					halo.enabled = true;

				StartCoroutine ("DisplayObjectName"); //delay before showing the object name
			}
		}
	}

	void OnMouseOver() {
		//	If I am not a chicken, or if I am a chicken that is stunned...
		if (gameObject.tag != "Chicken" || (gameObject.tag == "Chicken" && gameObject.GetComponent<Chicken> ().IsPickupStunned ())) {
			if (wc != null) wc.hovering = true;
			enabled = true;
			halo.enabled = true;
			camera.GetComponent<CollectionCursor> ().SetHover ();
		} else {
			enabled = false;
			if (wc != null) wc.hovering = false;
			halo.enabled = false;
			GetComponentInChildren<SpriteRenderer> ().color = defaultCol;
		}
	}

	void OnMouseExit ()
	{
		if (wc != null) wc.hovering = false;
		Camera.main.GetComponent<CollectionCursor> ().SetDefault ();

		if (gameObject.tag != "River") {
			if (GetComponentInChildren<SpriteRenderer> () != null)
				GetComponentInChildren<SpriteRenderer> ().color = defaultCol;
			else {
				GetComponent<Renderer> ().material.color = defaultCol;//GetComponent<Renderer> ().sharedMaterial.SetFloat("_Outline", 0.0f);
			}
			if (halo != null)
				halo.enabled = false;

			onMouseOver = false;
			if (gameObject.tag!="Chicken")
				enabled = false;
		}
	}

	void OnMouseDown ()
	{
		//camera.GetComponent<CollectionCursor> ().SetHold ();

		if (playerNearObject && gameObject.tag != "River") {
			if (enabled == true) {
				player.GetComponent<PlayerMovementRB> ().TriggerCollectAnim ();
				inventoryController.AddNewObject (gameObject); //collect the object in inventory
                                WorldContainer.UpdateCountCount(gameObject.name);
                        }
                }

		//collect some water first see if player has a water skin to add fill
		if (gameObject.tag == "River") {
			GameObject[] waterSkin = GameObject.FindGameObjectsWithTag ("WaterSkin");
			foreach (GameObject obj in waterSkin) {
				if (!obj.GetComponent<WaterSkin> ().waterFull) {
					obj.GetComponent<WaterSkin> ().Fill ();
					break;
				}
			}
		}
	}

	//to delay display of the object name
	IEnumerator DisplayObjectName ()
	{
		yield return new WaitForSeconds (delay);
		onMouseOver = true;
	}
}
//http://blogs.unity3d.com/2016/04/20/best-practices-for-rewarded-video-ads-2/