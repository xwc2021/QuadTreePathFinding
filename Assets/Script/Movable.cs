using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(NavMeshAgent))]
public class Movable : NetworkBehaviour
{
    [SerializeField]
    Transform target;
    NavMeshAgent agent;

    bool useAgent = false;
    // Use this for initialization
    void Start () {
        if (!isLocalPlayer)
            return;

        agent = GetComponent<NavMeshAgent>();
        target.parent = null;

        EnableAgent();

        var ShootingRayInstance =FindObjectOfType<ShootingRay>();
        ShootingRayInstance.target = target;
    }

    public void EnableAgent()
    {
        useAgent = true;
        target.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {

        if(useAgent)
            agent.destination = target.position;
    }
}
