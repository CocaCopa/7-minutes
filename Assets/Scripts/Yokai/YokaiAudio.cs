using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiAudio : MonoBehaviour {

    [SerializeField] private AudioClip doorJumpscare;

    private AudioSource audioSource;

    private void Awake() {
        
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {

        YokaiObserver.Instance.OnDoorOpenJumpscare += Observer_OnDoorOpenJumpscare;
    }

    private void Observer_OnDoorOpenJumpscare(object sender, System.EventArgs e) {
        
        audioSource.PlayOneShot(doorJumpscare);
    }
}
