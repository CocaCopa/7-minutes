using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {

    public event EventHandler<OnRoomEnterEventArgs> OnRoomEnter;
    public event EventHandler OnUpstairsHallEvent;
    public event EventHandler OnFoundDungeonDoor;

    public class OnRoomEnterEventArgs {

        public GameObject currentRoom;
        public GameObject previousRoom;
    }

    [SerializeField] LayerMask stairsLayer;
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 7.0f;
    [SerializeField] private float accelerationTime = 5.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    private CharacterController controller;
    private Transform cameraTransform;
    private Vector3 playerVelocity;
    private Vector3 moveDirection;
    private bool groundedPlayer;
    private float currentSpeed;
    public bool GetIsRunning() => currentSpeed > walkSpeed + 0.2f;
    public bool GetIsGrounded() => groundedPlayer;

    private void Awake() {

        currentSpeed = walkSpeed;
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    public Vector3 HandleMovement() {

        groundedPlayer = IsGrounded();

        if (groundedPlayer && playerVelocity.y <= 0) {
            playerVelocity.y = 0f;
        }

        Vector2 moveInput = InputManager.Instance.GetPlayerMovement();

        if (moveInput != Vector2.zero) {

            moveDirection = new (moveInput.x, 0, moveInput.y);
            moveDirection = cameraTransform.forward * moveDirection.z + cameraTransform.right * moveDirection.x;
            moveDirection.y = 0;
        }

        currentSpeed = CalculateSpeed(moveInput);
        Vector3 groundMovement = currentSpeed * Time.deltaTime * moveDirection;

        if (InputManager.Instance.Jump() && groundedPlayer) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        if (ClimbingStair(moveInput)) {
            
            playerVelocity.y += walkSpeed * Time.deltaTime;
        }
        else {

            playerVelocity.y += gravityValue * Time.deltaTime;
        }

        Vector3 aerialMovement = playerVelocity * Time.deltaTime;

        Vector3 playerEulerAngles = transform.eulerAngles;
        playerEulerAngles.y = Camera.main.transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(playerEulerAngles);

        return groundMovement + aerialMovement;
    }    

    private float CalculateSpeed(Vector3 input) {

        float targetSpeed;

        if (input != Vector3.zero) {

            bool isSprinting = InputManager.Instance.Sprint();
            targetSpeed = isSprinting && !ClimbingStair(input) ? sprintSpeed : walkSpeed;
        }
        else {

            targetSpeed = 0;
        }

        if (currentSpeed < 0.05f) {

            currentSpeed = 0;
        }

        return Mathf.Lerp(currentSpeed, targetSpeed, accelerationTime * Time.deltaTime);
    }

    private bool ClimbingStair(Vector3 input) {

        bool onStairs = Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, 1, stairsLayer);
        bool movingForward = input.y != 0;
        bool facingUp = Vector3.Dot(Camera.main.transform.forward, Vector3.down) < 0;

        return onStairs && movingForward && facingUp;
    }

    private bool IsGrounded() {

        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = -transform.up;
        float radius = 0.31f; // Player collider radius
        float maxDistance = 0.73f; // As much as it needs to hit the ground

        return Physics.SphereCast(new Ray(origin, direction), radius, maxDistance, ~LayerMask.NameToLayer("Player"), QueryTriggerInteraction.Ignore);
    }

    GameObject previousRoom;
    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("Trigger/EnteredRoom")) {
            
            bool enteredDifferentRoom = other.gameObject != previousRoom;

            if (enteredDifferentRoom) {

                OnRoomEnter?.Invoke(this, new OnRoomEnterEventArgs {

                    currentRoom = other.gameObject.transform.parent.gameObject,
                    previousRoom = previousRoom
                });
            }

            previousRoom = other.gameObject;
        }
        else if (other.CompareTag("Trigger/YokaiBasementEvent")) {

            OnFoundDungeonDoor?.Invoke(this, EventArgs.Empty);
        }
        else if (other.CompareTag("Trigger/UpstairsHallEvent")) {

            OnUpstairsHallEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
