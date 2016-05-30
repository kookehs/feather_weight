using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public Transform A;
	public Transform B;

	public Vector3 anchorPos;
	public Vector3 currentPos;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//	Place the arrow at the midpoint between the two desired objects
		anchorPos = new Vector3 (A.transform.position.x, A.transform.position.y, A.transform.position.z);
		currentPos = new Vector3 (B.transform.position.x, B.transform.position.y, B.transform.position.z);
		Vector3 midPointVector = (currentPos + anchorPos) / 2;
		transform.position = midPointVector;

		//	Resize the arrow and point it in the correct direction
		Vector3 relative = currentPos - anchorPos;
		float maggy = relative.magnitude;
		float angle = Mathf.Atan2 (relative.y, relative.x) * Mathf.Rad2Deg - 90;
		Quaternion q = Quaternion.AngleAxis (angle, Vector3.forward);
		transform.rotation = q;
	
	}
}
