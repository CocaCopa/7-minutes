using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAI : MonoBehaviour {

    [SerializeField] private Transform playerTransform;
    private NavMeshAgent navMeshAgent;

    private void Awake() {

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {

        Vector3 destination = playerTransform.position;
        navMeshAgent.destination = destination;
    }

    public float GetCurrentSpeed() => navMeshAgent.velocity.magnitude;
}
