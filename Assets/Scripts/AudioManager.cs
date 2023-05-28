using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] private GameObject ambientWhispers;
    [SerializeField] private AudioClip hitEffectJumpScare;
    [SerializeField] private AudioClip thunderEffect;
    [SerializeField] private float fadeOutAmbientWhispersTime;

    private AudioSource audioSource;
    private AudioSource ambientWhispersAudioSource;
    private bool fadeOutWhispers = false;

    private void Awake() {
        
        audioSource = GetComponent<AudioSource>();
        ambientWhispersAudioSource = ambientWhispers.GetComponent<AudioSource>();
    }

    private void Start() {

        YokaiObserver.Instance.OnBasementEventJumpscare += Observer_OnBasementEventJumpscare;
        YokaiObserver.Instance.OnBasementEventComplete += Observer_OnBasementEventComplete;
    }

    private void Update() {
        
        if (fadeOutWhispers) {

            float currentVolume = ambientWhispersAudioSource.volume;
            float targetVolume = 0;
            float lerpTime = fadeOutAmbientWhispersTime * Time.deltaTime;
            ambientWhispersAudioSource.volume = Mathf.Lerp(currentVolume, targetVolume, lerpTime);

            if (ambientWhispersAudioSource.volume <= 0.01f) {

                ambientWhispers.SetActive(false);
                audioSource.PlayOneShot(thunderEffect, 0.5f);
            }
        }
    }

    private void Observer_OnBasementEventComplete(object sender, System.EventArgs e) {

        fadeOutWhispers = true;
    }

    private void Observer_OnBasementEventJumpscare(object sender, System.EventArgs e) {

        audioSource.PlayOneShot(hitEffectJumpScare, 0.55f);
        ambientWhispers.SetActive(true);
    }
}
