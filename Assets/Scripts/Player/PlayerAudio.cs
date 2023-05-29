using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour {

    [SerializeField] private AudioClip[] woodenSurface;
    [SerializeField] private AudioClip[] tileSurface;
    [SerializeField] private AudioClip[] concreteSurface;
    [SerializeField] private AudioClip[] carpetSurface;
    [Space(10)]
    [SerializeField] private float footstepTimeWalking;
    [SerializeField] private float footstepTimeRunning;
    [Space(10)]
    [SerializeField] private AudioClip[] screamEffects;
    [SerializeField] private AudioClip heavyBreathing;

    private AudioSource audioSource;
    private AudioClip previousClip;
    private PlayerMovement playerMovement;
    private YokaiBehaviour yokaiBehaviour;
    private YokaiController yokaiController;

    private float footstepTimer = 0;
    private bool yokaiIsChasing = false;

    private void Awake() {

        audioSource = GetComponent<AudioSource>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        yokaiBehaviour = FindObjectOfType<YokaiBehaviour>();
        yokaiController = FindObjectOfType<YokaiController>();
    }

    private void Start() {

        YokaiObserver.Instance.OnBasementEventJumpscare += Observer_OnBasementEventJumpscare;
        YokaiObserver.Instance.OnRunEventChase += Observer_OnRunEventChase;
        yokaiBehaviour.OnYokaiSpawn += YokaiBehaviour_OnYokaiSpawn;
        yokaiBehaviour.OnYokaiDespawn += YokaiBehaviour_OnYokaiDespawn;
    }

    private void Update() {

        PlayFootstepAudio();

        if (yokaiIsChasing) {

            Vector3 playerPosition = transform.position + Vector3.up;
            Vector3 yokaiPosition = yokaiController.GetYokaiTransform().position;
            Vector3 direction = (playerPosition - yokaiPosition).normalized;
            float distance = Vector3.Distance(yokaiPosition, playerPosition);
            Physics.Raycast(yokaiPosition, direction, out RaycastHit hit, distance);
            Debug.DrawRay(yokaiPosition, direction * distance, Color.red);

            if (hit.transform != null && hit.transform.CompareTag("Player")) {

                int randomScream = Random.Range(0, screamEffects.Length);
                audioSource.Stop();
                audioSource.PlayOneShot(screamEffects[randomScream], 1.5f);
                yokaiIsChasing = false;
            }
        }
    }

    private void Observer_OnRunEventChase(object sender, System.EventArgs e) {

        yokaiIsChasing = true;
    }

    private void YokaiBehaviour_OnYokaiSpawn(object sender, System.EventArgs e) {

        audioSource.PlayOneShot(heavyBreathing);
    }

    private void YokaiBehaviour_OnYokaiDespawn(object sender, System.EventArgs e) {

        yokaiIsChasing = false;
    }

    private void Observer_OnBasementEventJumpscare(object sender, System.EventArgs e) {

        audioSource.PlayOneShot(heavyBreathing);
    }

    private void PlayFootstepAudio() {

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

                        case "Floor/Carpet":
                        randomClipIndex = Random.Range(0, carpetSurface.Length);
                        selectedAudioClip = carpetSurface[randomClipIndex];
                        previousClip = selectedAudioClip;
                        break;

                        default:
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
