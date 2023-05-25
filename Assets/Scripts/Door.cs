using System;
using UnityEngine;

[System.Serializable]
public class Door : ItemOpen, IInteracteable {

    public event EventHandler<OnDoorOpenEventArgs> OnDoorOpen;
    public class OnDoorOpenEventArgs {
        public bool isOpen;
        public Transform jumpscareTransform;
    }

    [SerializeField] private bool fireEvent = false;
    [HideInInspector]
    [SerializeField] private Transform jumpscarePosition;

    #region Custom Editor:
    public bool GetFireEvent() => fireEvent;
    public Transform GetJumpscareTransform() => jumpscarePosition;
    public Transform SetJumpscarePosition(Transform value) => jumpscarePosition = value;
    #endregion

    private void Awake() {

        defaultPosition = transform.eulerAngles;
        enabled = false;
    }

    private void Update() {

        transform.localRotation = SlerpRotation();
        enabled = DisableUpdateMethod();
    }

    public override void Interact() {

        animationPoints = 0;
        currentPosition = transform.eulerAngles;
        targetPosition = isOpen ? defaultPosition : NewRotation();

        isOpen = !isOpen;

        if (fireEvent) {
            OnDoorOpen?.Invoke(this, new OnDoorOpenEventArgs {
                isOpen = isOpen,
                jumpscareTransform = jumpscarePosition
            });
        }

        enabled = true;
    }

    private Quaternion SlerpRotation() {

        Quaternion current = Quaternion.Euler(currentPosition);
        Quaternion target = Quaternion.Euler(targetPosition);
        animationPoints += openSpeed * Time.deltaTime;

        return Quaternion.Slerp(current, target, animCurve.Evaluate(animationPoints));
    }

    private float DotProduct() {

        Vector3 doorForward = transform.forward;
        Vector3 playerForward = Camera.main.transform.forward;

        return Vector3.Dot(doorForward, playerForward);
    }

    private Vector3 NewRotation() {

        // Open away from player
        if (DotProduct() > 0) { // same direction

            return defaultPosition + offsetAmount;
        }
        else {  // opposite direction

            return defaultPosition - offsetAmount;
        }
    }
}
