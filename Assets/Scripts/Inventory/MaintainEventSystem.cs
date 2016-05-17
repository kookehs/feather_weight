using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MaintainEventSystem : MonoBehaviour {



	// Update is called once per frame
	void Update () {
		if(gameObject.activeSelf && (EventSystem.current == null || EventSystem.current != transform.GetComponent<EventSystem>()))
			EventSystem.current = transform.GetComponent<EventSystem>();
	}
}
