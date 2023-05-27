using UnityEngine;

public class YokaiAnimator : MonoBehaviour {

    private Animator animator;
    private YokaiBehaviour agent;

    private const string RUN = "Run";
    private const string CRAWL_RUN = "CrawlRun";

    private bool chooseAnimation = false;

    private void Awake() {

        animator = GetComponent<Animator>();
        agent = GetComponentInParent<YokaiBehaviour>();
    }

    private void Update() {

        RandomRunAnimation(agent.GetCurrentSpeed());
    }

    private void RandomRunAnimation(float speed) {

        if (speed <= 0.01f) {

            chooseAnimation = true;
            animator.SetBool(RUN, false);
            animator.SetBool(CRAWL_RUN, false);
        }
        else if (chooseAnimation) {

            chooseAnimation = false;

            float rand = Random.Range(0, 2);

            if (rand == 0) {
                animator.SetBool(RUN, true);
            }
            else if (rand == 1) {

                animator.SetBool(CRAWL_RUN, true);
            }
        }
    }
}
