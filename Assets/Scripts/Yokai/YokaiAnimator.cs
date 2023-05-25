using UnityEngine;

public class YokaiAnimator : MonoBehaviour {

    private Animator animator;
    private YokaiBehaviour agent;

    private const string SPEED = "Speed";

    private void Awake() {

        animator = GetComponent<Animator>();
        agent = GetComponentInParent<YokaiBehaviour>();
    }

    private void Update() {

        float speed = agent.GetCurrentSpeed();
        animator.SetFloat(SPEED, speed);
    }
}
