using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YokaiBehaviour : MonoBehaviour {

    private NavMeshAgent navMeshAgent;

    private void Awake() {

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void SpawnAtPosition(Vector3 position) {

    }

    public void DisableCharacter() {


    }

    public void KillPlayer() {


    }

    public void FollowPlayer(Transform playerTransform) {

        navMeshAgent.destination = playerTransform.position;
    }

    public void ChasePlayer(Transform playerTransform) {

        navMeshAgent.destination = playerTransform.position;
    }

    bool m_bool = false;
    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Q)) {

            m_bool = !m_bool;

            if (m_bool) {

                navMeshAgent.acceleration = 7f;
                navMeshAgent.speed = 3.5f;
                Debug.Log("--");
            }
            else {

                //navMeshAgent.acceleration = 0.22f;
                navMeshAgent.speed = 0.15f;
            }
        }
    }
}
