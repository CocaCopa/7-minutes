using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour {

    [SerializeField] private CinemachineVirtualCamera mainVirtualCamera;
    [SerializeField] private float shakeAmount;
    [SerializeField] private Volume postStaticEffect;
    [SerializeField] private float staticEffectTime;
    [SerializeField] private AudioClip staticSFX;

    CinemachineBasicMultiChannelPerlin cameraNoise;
    private YokaiBehaviour yokaiBehaviour;
    private AudioSource camAudioSource;

    private void Awake() {

        yokaiBehaviour = FindObjectOfType<YokaiBehaviour>();
        camAudioSource = Camera.main.transform.GetComponent<AudioSource>();
        cameraNoise = mainVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start() {

        yokaiBehaviour.OnYokaiSpawn += YokaiBehaviour_OnYokaiSpawnDespawn;
        yokaiBehaviour.OnYokaiDespawn += YokaiBehaviour_OnYokaiSpawnDespawn;
        yokaiBehaviour.OnKillPlayer += YokaiBehaviour_OnKillPlayer;
        GameManager.Instance.OnPlayerSpawn += GameManager_OnPlayerSpawn;
    }

    private void GameManager_OnPlayerSpawn(object sender, System.EventArgs e) {

        cameraNoise.m_AmplitudeGain = 0;
    }

    private void YokaiBehaviour_OnKillPlayer(object sender, System.EventArgs e) {

        cameraNoise.m_AmplitudeGain = shakeAmount;
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
