using UnityEngine;

public abstract class ItemOpen : MonoBehaviour, IInteractable {

    [SerializeField] protected float openSpeed = 3.5f;
    [SerializeField] protected Vector3 offsetAmount = new(0, 60, 0);
    [SerializeField] protected AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    protected Vector3 defaultPosition;
    protected Vector3 currentPosition;
    protected Vector3 targetPosition;
    protected float animationPoints = 0;
    protected bool isOpen = false;
    protected AudioSource audioSource;

    public abstract void Interact();

    public virtual string GetInteractableText() {

        return "Open/Close";
    }

    protected bool DisableUpdateMethod() => animationPoints <= 1;
    public bool GetIsOpen() => isOpen;
}
