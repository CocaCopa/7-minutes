using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YokaiBehaviour : MonoBehaviour {

    public event EventHandler OnStartRunning;
    [SerializeField] private GameObject yokaiVisuals;
    [SerializeField] private List<GameObject> interactables;

    private NavMeshAgent navMeshAgent;
    private GameObject equippedItem;

    private bool isRunning;

    private void Awake() {

        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
    }

    public bool IsActing() {

        return yokaiVisuals.activeInHierarchy;
    }

    public void SpawnAtPosition(Transform m_transform) {

        transform.position = m_transform.position;
        transform.forward = m_transform.forward;
        yokaiVisuals.SetActive(true);
        navMeshAgent.enabled = true;
    }

    public void DespawnCharacter() {

        navMeshAgent.enabled = false;
        yokaiVisuals.SetActive(false);
    }

    public void ChasePlayer() {

        if (!navMeshAgent.enabled) {
            return;
        }

        

        Vector3 playerPosition = YokaiObserver.Instance.GetPlayerTransform().position;
        navMeshAgent.acceleration = 8;
        navMeshAgent.speed = 4f;
        navMeshAgent.destination = playerPosition;
    }

    public void EquipItem(List<GameObject> equipableItems) {

        int randomIndex = UnityEngine.Random.Range(0, equipableItems.Count);
        GameObject randomItem = equipableItems[randomIndex];
        Rigidbody rb = randomItem.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        equippedItem = randomItem;
    }

    public void ThrowItem(Vector3 direction, float throwForce) {

        Rigidbody rb = equippedItem.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(direction * throwForce, ForceMode.Impulse);
    }

    public void OpenRandomDoor() {

        int randomIndex = UnityEngine.Random.Range(0, interactables.Count);

        interactables[randomIndex].GetComponent<IInteractable>().Interact();
    }

    private void Update() {
        
        if (Input.GetKeyDown(KeyCode.Q)) {
            OpenRandomDoor();
        }
    }
    public void KillPlayer() {


    }

    public void RunTowardsPosition(Vector3 position) {

        navMeshAgent.acceleration = 3f;
        navMeshAgent.speed = 3.5f;
        navMeshAgent.destination = position;
        OnStartRunning?.Invoke(this, EventArgs.Empty);
    }
    
    public void RandomBehaviour() {


    }

    public float GetCurrentSpeed() => navMeshAgent.velocity.magnitude;
    public bool GetIsRunning() => isRunning = navMeshAgent.velocity.magnitude > 0 ? true : false;
}
