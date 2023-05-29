using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiAudio : MonoBehaviour {

    [SerializeField] private AudioClip doorJumpscare;
    [SerializeField] private AudioClip getTheKeySFX;
    [SerializeField] private float delayTime;
    [SerializeField] private AudioClip laughterSFX;

    private AudioSource audioSource;

    private void Awake() {
        
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {

        YokaiObserver.Instance.OnDoorOpenJumpscare += Observer_OnDoorOpenJumpscare;
        YokaiObserver.Instance.OnBasementEventJumpscare += Observer_OnBasementEventJumpscare;
        YokaiObserver.Instance.OnBasementEventComplete += Observer_OnBasementEventComplete;
    }

    private void Observer_OnBasementEventComplete(object sender, System.EventArgs e) {

        audioSource.PlayOneShot(laughterSFX, 1.35f);
    }

    private void Observer_OnBasementEventJumpscare(object sender, System.EventArgs e) {

        Invoke(nameof(DelayAudioClipGetTheKey), delayTime);
    }

    private void DelayAudioClipGetTheKey() {

        audioSource.PlayOneShot(getTheKeySFX, 1.5f);
    }

    private void Observer_OnDoorOpenJumpscare(object sender, System.EventArgs e) {
        
        audioSource.PlayOneShot(doorJumpscare);
    }
}
