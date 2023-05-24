using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : ItemOpen, IInteracteable {

    private void Awake() {

        defaultPosition = transform.localPosition;
        enabled = false;
    }

    private void Update() {

        transform.localPosition = LerpLocalPosition();
        enabled = DisableUpdateMethod();
    }

    public override void Interact() {

        animationPoints = 0;
        currentPosition = transform.localPosition;
        targetPosition = isOpen ? defaultPosition : defaultPosition + transform.forward * offsetAmount.magnitude;

        isOpen = !isOpen;
        enabled = true;
    }

    private Vector3 LerpLocalPosition() {

        Vector3 current = currentPosition;
        Vector3 target = targetPosition;
        animationPoints += openSpeed * Time.deltaTime;
        
        return Vector3.Lerp(current, target, animCurve.Evaluate(animationPoints));
    }
}
