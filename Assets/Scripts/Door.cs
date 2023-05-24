using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteracteable {

    [SerializeField] private bool startsOpened;
    [SerializeField] private float rotationSpeed = 3.5f;
    [SerializeField] private Vector3 rotationAmount = new(0, 60, 0);
    [SerializeField] private AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float animationPoints = 0;
    private Vector3 defaultRotationEuler;
    private Quaternion currentRotation;
    private Vector3 targetRotationEuler;
    private bool isOpen = false;

    private void Awake() {

        defaultRotationEuler = transform.eulerAngles;
        isOpen = startsOpened;
        enabled = false;
    }

    private void Update() {

        transform.localRotation = SlerpRotation();
    }

    public void Interact() {

        animationPoints = 0;
        currentRotation = transform.localRotation;
        targetRotationEuler = isOpen ? defaultRotationEuler : NewRotation();

        isOpen = !isOpen;
        enabled = true;
    }

    private Quaternion SlerpRotation() {

        Quaternion current = currentRotation;
        Quaternion target = Quaternion.Euler(targetRotationEuler);
        animationPoints += rotationSpeed * Time.deltaTime;

        return Quaternion.Slerp(current, target, animCurve.Evaluate(animationPoints));
    }

    private float DotProduct() {

        Vector3 doorForward = transform.forward;
        Vector3 playerForward = Camera.main.transform.forward;
        return Vector3.Dot(doorForward, playerForward);
    }

    private Vector3 NewRotation() {

        // Open away from player
        if (DotProduct() > 0) {

            return defaultRotationEuler + rotationAmount;
        }
        else {

            return defaultRotationEuler - rotationAmount;
        }
    }
}
