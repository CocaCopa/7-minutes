using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour {

    [SerializeField] private Volume postStaticEffect;
    [SerializeField] private float staticEffectTime;
    [SerializeField] private AudioClip staticSFX;

    private YokaiBehaviour yokaiBehaviour;
    private AudioSource camAudioSource;

    private void Awake() {

        yokaiBehaviour = FindObjectOfType<YokaiBehaviour>();
        camAudioSource = Camera.main.transform.GetComponent<AudioSource>();
    }

    private void Start() {

        yokaiBehaviour.OnYokaiSpawn += YokaiBehaviour_OnYokaiSpawnDespawn;
        yokaiBehaviour.OnYokaiDespawn += YokaiBehaviour_OnYokaiSpawnDespawn;
    }

    private void YokaiBehaviour_OnYokaiSpawnDespawn(object sender, System.EventArgs e) {

        StaticEffect();
        camAudioSource.pitch = Random.Range(0.85f, 1.15f);
        camAudioSource.PlayOneShot(staticSFX, 1.4f);
    }

    private void StaticEffect() {

        postStaticEffect.enabled = true;
        Invoke(nameof(DisableStaticEffect), staticEffectTime);
    }

    private void DisableStaticEffect() {

        postStaticEffect.enabled = false;
    }
}
