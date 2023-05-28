using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour {

    [SerializeField] private AudioClip[] woodenSurface;
    [SerializeField] private AudioClip[] tileSurface;
    [SerializeField] private AudioClip[] concreteSurface;
    [Space(10)]
    [SerializeField] private float footstepTimeWalking;
    [SerializeField] private float footstepTimeRunning;

    private AudioSource audioSource;
    private AudioClip previousClip;
    private PlayerMovement playerMovement;

    private float footstepTimer = 0;

    private void Awake() {

        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update() {

        if (playerMovement.GetIsGrounded() && InputManager.Instance.GetPlayerMovement() != Vector2.zero) {

            footstepTimer += Time.deltaTime;

            float timer = playerMovement.GetIsRunning() ? footstepTimeRunning : footstepTimeWalking;

            if (footstepTimer >= timer) {

                if (Physics.Raycast(new Ray(transform.position + Vector3.up * 0.2f, Vector3.down), out RaycastHit hit, 0.37f)) {

                    AudioClip selectedAudioClip;

                    switch (hit.transform.tag) {

                        case "Floor/Wood":
                        int randomClipIndex = Random.Range(0, woodenSurface.Length);
                        selectedAudioClip = woodenSurface[randomClipIndex];
                        previousClip = selectedAudioClip;
                        break;

                        case "Floor/Tile":
                        randomClipIndex = Random.Range(0, tileSurface.Length);
                        selectedAudioClip = tileSurface[randomClipIndex];
                        previousClip = selectedAudioClip;
                        break;

                        case "Floor/Concrete":
                        randomClipIndex = Random.Range(0, concreteSurface.Length);
                        selectedAudioClip = concreteSurface[randomClipIndex];
                        previousClip = selectedAudioClip;
                        break;

                        default:;
                        selectedAudioClip = previousClip;
                        break;
                    }
                    audioSource.PlayOneShot(selectedAudioClip);
                    footstepTimer = 0;
                }

            }   
        }
    }
}
