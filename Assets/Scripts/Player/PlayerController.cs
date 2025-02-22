using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public event EventHandler OnMaskItemCollected;

    [SerializeField] private Transform respawnTransform;
    [SerializeField] private float interactDistance = 1.5f;
    [SerializeField] private LayerMask intercatLayer;

    private CharacterController controller;
    private PlayerMovement playerMovement;

    private void Awake() {
        
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Start() {

        InputManager.Instance.OnInteractPerformed += Input_OnInteractPerformed;
    }

    private void Update() {

        controller.Move(playerMovement.HandleMovement());
    }

    private void Input_OnInteractPerformed(object sender, EventArgs e) {

        if (InteractableItem(out IInteractable interactable)) {

            interactable.Interact();

            if (interactable.GetType() == typeof(MaskItem)) {
                OnMaskItemCollected?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private bool InteractableItem(out IInteractable interactable) {

        Vector3 rayOrigin = Camera.main.transform.position;
        Vector3 rayDirection = Camera.main.transform.forward;
        Ray ray = new (rayOrigin, rayDirection);

        Physics.Raycast(ray, out RaycastHit hit, interactDistance, intercatLayer);

        if (hit.transform != null && hit.transform.TryGetComponent(out interactable)) {

            return true;
        }
        else {

            interactable = null;
            return false;
        }
    }

    public IInteractable GetInteractObject() {

        Vector3 rayOrigin = Camera.main.transform.position;
        Vector3 rayDirection = Camera.main.transform.forward;
        Ray ray = new (rayOrigin, rayDirection);

        Physics.Raycast(ray, out RaycastHit hit, interactDistance, intercatLayer);

        if (hit.transform != null && hit.transform.TryGetComponent(out IInteractable interactable)) {

            return interactable;
        }

        return null;
    }
}
