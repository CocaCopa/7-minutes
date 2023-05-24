using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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

    private void Input_OnInteractPerformed(object sender, EventArgs e) {

        Vector3 rayOrigin = Camera.main.transform.position;
        Vector3 rayDirection = Camera.main.transform.forward;
        Ray ray = new (rayOrigin, rayDirection);


        Physics.Raycast(ray, out RaycastHit hit, interactDistance, intercatLayer);
        if (hit.transform != null && hit.transform.TryGetComponent(out IInteracteable interactable)) {

            interactable.Interact();
        }
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.R)) {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        controller.Move(playerMovement.HandleMovement());
    }
}
