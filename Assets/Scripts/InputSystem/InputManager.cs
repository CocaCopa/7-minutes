using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour {

    public event EventHandler OnInteractPerformed;
    public static InputManager Instance { get; private set; }
    
    private PlayerControls playerControls;

    private void Awake() {
        
        SetInstance();
        playerControls = new PlayerControls();
    }

    private void SetInstance() {

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        }
        else {
            Instance = this;
        }
    }

    private void OnEnable() {

        playerControls.Enable();
        playerControls.Player.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {

        OnInteractPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void OnDisable() {

        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement() => playerControls.Player.Movement.ReadValue<Vector2>();
    public Vector2 GetMouseDelta() => playerControls.Player.Look.ReadValue<Vector2>();
    public bool Jump() => playerControls.Player.Jump.triggered;
    public bool Sprint() => playerControls.Player.Sprint.ReadValue<float>() > 0;
}
