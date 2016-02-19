using UnityEngine;
using System.Collections;

public class SampleAgentScript : MonoBehaviour {

	NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (agent.hasPath);
	}

	public void setTarget(GameObject t) {
		agent.SetDestination (t.transform.position);
	}

	public bool hasPath() {
		return agent.hasPath;
	}
}
