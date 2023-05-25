using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiAnimator : MonoBehaviour {

    private Animator animator;
    private NavMeshAI agent;

    private const string SPEED = "Speed";

    private void Awake() {
        
        animator = GetComponent<Animator>();
        agent = GetComponentInParent<NavMeshAI>();
    }

    private void Update() {

        float speed = agent.GetCurrentSpeed();
        animator.SetFloat(SPEED, speed);
    }
}
