using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YokaiBehaviour : MonoBehaviour {

    public event EventHandler OnYokaiSpawn;
    public event EventHandler OnYokaiDespawn;
    public event EventHandler OnChasePlayer;
    public event EventHandler OnKillPlayer;

    [SerializeField] private GameObject yokaiVisuals;
    [SerializeField] private List<GameObject> doors;
    [SerializeField] private List<GameObject> throwables;
    [SerializeField] private Transform killPlayerTransform;

    private NavMeshAgent navMeshAgent;
    private GameObject equippedItem;

    private bool canFireKillEvent = true;
    private bool canFireChaseEvent = true;
    private bool isRunning = false;
    private bool disappearOnTargetPosition = false;
    private Vector3 targetPosition;

    private void Awake() {

        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
    }

    private void Update() {

        if (disappearOnTargetPosition) {

            bool reachedPosition = Vector3.Distance(transform.position, targetPosition) < 1.5f;

            if (reachedPosition) {

                disappearOnTargetPosition = false;
                Invoke(nameof(DespawnCharacter), 1.2f);
            }
        }
    }

    public bool IsActing() {

        return yokaiVisuals.activeInHierarchy;
    }

    public void SpawnAtPosition(Transform m_transform, bool agentEnabled = true) {

        canFireKillEvent = true;
        transform.position = m_transform.position;
        transform.forward = m_transform.forward;
        yokaiVisuals.SetActive(true);
        navMeshAgent.enabled = agentEnabled;

        OnYokaiSpawn?.Invoke(this, EventArgs.Empty);
    }

    public void SetStats(float movementSpeed, float acceleration, float stoppingDistance, bool autobraking = false) {

        navMeshAgent.speed = movementSpeed;
        navMeshAgent.acceleration = acceleration;
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.autoBraking = autobraking;
    }

    public void DespawnCharacter() {

        canFireChaseEvent = true;
        navMeshAgent.enabled = false;
        yokaiVisuals.SetActive(false);

        transform.localPosition = Vector3.zero;
        OnYokaiDespawn?.Invoke(this, EventArgs.Empty);
    }

    public void ChasePlayer() {

        if (!navMeshAgent.enabled) {
            return;
        }

        if (canFireChaseEvent) {
            canFireChaseEvent = false;
            OnChasePlayer?.Invoke(this, EventArgs.Empty);
        }

        Vector3 playerPosition = YokaiObserver.Instance.GetPlayerTransform().position;
        navMeshAgent.destination = playerPosition;
    }

    public void EquipItem(List<GameObject> equipableItems) {

        int randomIndex = UnityEngine.Random.Range(0, equipableItems.Count);
        GameObject randomItem = equipableItems[randomIndex];
        Rigidbody rb = randomItem.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        equippedItem = randomItem;
    }

    public void ThrowItem() {

        Vector3 direction = Vector3.zero;
        int randomDirection = UnityEngine.Random.Range(0, 4);

        switch (randomDirection) {

            case 0:
            direction = Vector3.up + Vector3.forward;
            break;

            case 1:
            direction = Vector3.up + Vector3.right;
            break;

            case 2:
            direction = Vector3.up + Vector3.left;
            break;

            case 3:
            direction = Vector3.up + Vector3.back;
            break;
        }

        int randomItem = UnityEngine.Random.Range(0, throwables.Count);
        Rigidbody rb = throwables[randomItem].GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(direction * 30, ForceMode.Impulse);
    }

    public void OpenRandomDoor() {

        int randomIndex = UnityEngine.Random.Range(0, doors.Count);

        doors[randomIndex].GetComponent<IInteractable>().Interact();
    }

    public void KillPlayer(float rangeToKill, bool behindPlayer) {

        bool kill = false;

        if (behindPlayer) {

            bool playerLooksBehind = Vector3.Dot(transform.forward, YokaiObserver.Instance.GetPlayerTransform().forward) < 0;

            if (playerLooksBehind) {

                kill = true;
            }
        }

        if (PlayerInRange(rangeToKill) && !behindPlayer) {

            kill = true;
        }

        if (kill && canFireKillEvent) {

            OnKillPlayer?.Invoke(this, EventArgs.Empty);
            canFireKillEvent = false;
            navMeshAgent.enabled = false;
            transform.position = killPlayerTransform.position;
            transform.rotation = killPlayerTransform.rotation;
        }
    }

    private bool PlayerInRange(float rangeToKill) {

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        Ray ray = new(origin, direction);
        Physics.Raycast(ray, out RaycastHit hit, rangeToKill);

        return hit.transform != null && hit.transform.CompareTag("Player");
    }

    public void RunTowardsPosition(Vector3 position, bool despawnOnPositionReach = false) {

        navMeshAgent.destination = position;

        if (despawnOnPositionReach) {

            disappearOnTargetPosition = true;
            targetPosition = position;
        }
    }
    
    public void RandomBehaviour() {

        int randomBehaviour = UnityEngine.Random.Range(0, 2);

        switch (randomBehaviour) {

            case 0:
            OpenRandomDoor();
            break;

            case 1:
            ThrowItem();
            break;
        }
    }

    public float GetCurrentSpeed() => navMeshAgent.velocity.magnitude;
    public bool GetIsRunning() => isRunning = navMeshAgent.velocity.magnitude > 0 ? true : false;
}
