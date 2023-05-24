using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemOpen : MonoBehaviour, IInteracteable {

    [SerializeField] protected float openSpeed = 3.5f;
    [SerializeField] protected Vector3 offsetAmount = new(0, 60, 0);
    [SerializeField] protected AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    protected Vector3 defaultPosition;
    protected Vector3 currentPosition;
    protected Vector3 targetPosition;
    protected float animationPoints = 0;
    protected bool isOpen = false;

    public abstract void Interact();

    protected bool DisableUpdateMethod() => animationPoints <= 1;
}
