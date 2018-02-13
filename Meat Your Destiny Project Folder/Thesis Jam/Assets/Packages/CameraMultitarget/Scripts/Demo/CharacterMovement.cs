using UnityEngine;
using System.Collections;
using UnityEngine.AI;

/// <summary>
/// This is a basic controller for a WASD control scheme 
/// </summary>
[RequireComponent (typeof(Rigidbody))]
[RequireComponent (typeof(NavMeshAgent))]
public class CharacterMovement : MonoBehaviour {

	float timer;

	NavMeshAgent agent;
	GameObject targetsContainer;

	// Use this for initialization
	void Start () {		
		agent = GetComponent<NavMeshAgent>();
		targetsContainer = GameObject.Find("TargetPoints");
		agent.SetDestination( targetsContainer.transform.GetChild(Random.Range(0, targetsContainer.transform.childCount)).transform.position);
	}
		
	void Update()
	{			
		if (agent.remainingDistance <= 2)
		{
			agent.SetDestination( targetsContainer.transform.GetChild(Random.Range(0, targetsContainer.transform.childCount)).transform.position);
		}
	}
	
}
