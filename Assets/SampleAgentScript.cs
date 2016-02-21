using UnityEngine;
using System.Collections;

public class SampleAgentScript : MonoBehaviour {

	NavMeshAgent agent;
	public Vector3 targetPos;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		agent.SetDestination (targetPos);
		Debug.Log (hasPath ());
	}

	public void setTarget(GameObject t) {
		targetPos = t.transform.position;
	}

	public bool hasPath() {
		return agent.hasPath;
	}
}
