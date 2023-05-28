using System;
using UnityEngine;
using Cinemachine;

public class MenuManager : MonoBehaviour {

    [SerializeField] private float delayGameStart = 1.85f;
    [SerializeField] CinemachineVirtualCamera mainVirtualCamera;
    [SerializeField] GameObject cameraUI;
    [SerializeField] AudioSource ambientRain;
    [SerializeField] private float ambientRainGameplayPitch = 0.75f;
    [SerializeField] AudioSource ambientEnv;

    private void Awake() {

        Time.timeScale = 0;
    }

    public void StartGame() {

        mainVirtualCamera.Priority = 10;
        Invoke(nameof(DelayGameStart), delayGameStart);
        Time.timeScale = 1;
        
    }

    private void DelayGameStart() {

        cameraUI.SetActive(true);
        ambientRain.pitch = ambientRainGameplayPitch;
        ambientEnv.Play();
    }
}
