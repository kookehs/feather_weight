using UnityEngine;
using System.Collections;

public class TwicthAction : MonoBehaviour
{

	private float startPos;

	public GameObject explosion;
	//public GameObject ground;
	public float dropRate = 2.5f;

	private bool moveDown = true;

	void Awake ()
	{
		startPos = transform.localPosition.y;
	}

	void Update(){
		if (moveDown)
			transform.Translate ((Vector3.down) * dropRate * Time.deltaTime);
	}

	// Update is called once per frame
	public void OnTriggerEnter (Collider obj)
	{
		if (obj.gameObject.tag.Equals ("Ground") || obj.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			moveDown = false;
			explosion.SetActive (true);
			StartCoroutine ("EndExplosion");
		}
	}

	IEnumerator EndExplosion ()
	{
		yield return new WaitForSeconds (0.5f);
		transform.parent.gameObject.SetActive (false);
	}
}
