using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YokaiBehaviour : MonoBehaviour {

    [SerializeField] private GameObject yokaiVisuals;

    [Space(10)]

    [SerializeField] private float rangeToKillPlayer;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    private NavMeshAgent navMeshAgent;

    private void Awake() {

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SpawnAtPosition(Transform m_transform) {

        transform.position = m_transform.position;
        transform.forward = m_transform.forward;
        yokaiVisuals.SetActive(true);
    }

    public void DisableCharacter() {


    }

    public void ChasePlayer(Transform playerTransform) {

        navMeshAgent.destination = playerTransform.position;

        bool nearPlayer = Vector3.Distance(transform.position, playerTransform.position) < rangeToKillPlayer;

        if (nearPlayer) {

            KillPlayer();
        }
    }

    public void KillPlayer() {


    }

    public void RunTowardsPosition(Vector3 position) {

        navMeshAgent.acceleration = 3f;
        navMeshAgent.speed = 3.5f;
        navMeshAgent.destination = position;
    }

    public float GetCurrentSpeed() => navMeshAgent.velocity.magnitude;
}
