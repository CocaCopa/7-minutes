using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ItemOpen, IInteracteable {

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
