using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScene : MonoBehaviour {

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float transitionTime = 1;

    private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    private float animationPoints = 0;
    private bool fadeOut = false;

    private void Awake() {

        canvasGroup.alpha = 1;
    }

    private void Start() {
        
        ReloadScene(true);
    }

    private void Update() {

        if (fadeOut) {

            if (animationPoints <= 1)
            animationPoints += transitionTime * Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, curve.Evaluate(animationPoints));

            if (canvasGroup.alpha == 0) {

                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else {

            animationPoints += transitionTime * Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, curve.Evaluate(animationPoints));

            if (canvasGroup.alpha == 1) {

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
    
    public void ReloadScene(bool fadeout) {

        animationPoints = 0;
        fadeOut = fadeout;

        if (!fadeOut) {
                        
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
