using UnityEngine;

public class Drawer : ItemOpen, IInteractable {

    [Header("--- SFX ---")]
    [SerializeField] private AudioClip openSFX;
    [SerializeField] private AudioClip closeSFX;

    private void Awake() {

        audioSource = GetComponentInChildren<AudioSource>();
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

        AudioClip sfx = isOpen ? closeSFX : openSFX;
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(sfx, 0.65f);

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
