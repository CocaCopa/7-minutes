using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YokaiBehaviour : MonoBehaviour {

    [SerializeField] private GameObject yokaiVisuals;
    [SerializeField] private List<GameObject> interactables;

    private NavMeshAgent navMeshAgent;
    private GameObject equippedItem;

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

    public void ChasePlayer(float rangeToKill) {

        if (!navMeshAgent.enabled) {
            return;
        }

        Vector3 playerPosition = YokaiObserver.Instance.GetPlayerTransform().position;
        navMeshAgent.acceleration = 8;
        navMeshAgent.speed = 4f;
        navMeshAgent.destination = playerPosition;

        bool nearPlayer = Vector3.Distance(transform.position, playerPosition) < rangeToKill;

        if (nearPlayer) {

            KillPlayer();
        }
    }

    public void EquipItem(List<GameObject> equipableItems) {

        int randomIndex = Random.Range(0, equipableItems.Count);
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

        int randomIndex = Random.Range(0, interactables.Count);

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
    }
    
    public void RandomBehaviour() {


    }

    public float GetCurrentSpeed() => navMeshAgent.velocity.magnitude;
}
