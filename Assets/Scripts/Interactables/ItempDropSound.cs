using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItempDropSound : MonoBehaviour {

    [SerializeField] private AudioClip[] dropSounds;

    private AudioSource audioSource;

    private void Awake() {
        
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision) {
        
        int randomSFX = Random.Range(0, dropSounds.Length);
        audioSource.pitch = Random.Range(0.85f, 1.15f);
        audioSource.PlayOneShot(dropSounds[randomSFX]);
    }
}
