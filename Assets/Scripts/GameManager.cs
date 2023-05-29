using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnPlayerSpawn;

    private YokaiBehaviour yokaiBehaviour;
    private YokaiController yokaiController;
    private PlayerController playerController;
    private InputManager inputManager;
    private CinemachineInputProvider inputProvider;
    private GameTimer timer;

    private Transform playerTransform;
    private Transform yokaiTransform;

    private List<MonoBehaviour> disableScripts = new();

    private void Awake() {

        Instance = this;

        disableScripts.Add(yokaiBehaviour = FindObjectOfType<YokaiBehaviour>());
        disableScripts.Add(playerController = FindObjectOfType<PlayerController>());
        disableScripts.Add(inputManager = FindObjectOfType<InputManager>());
        disableScripts.Add(inputProvider = FindObjectOfType<CinemachineInputProvider>());
        disableScripts.Add(yokaiController = FindObjectOfType<YokaiController>());
        //disableScripts.Add(timer = FindObjectOfType<GameTimer>());

        playerTransform = playerController.transform;
        yokaiTransform = yokaiBehaviour.transform;
    }

    private void Start() {

        disableScripts.Add(YokaiObserver.Instance);
        yokaiBehaviour.OnKillPlayer += YokaiBehaviour_OnKillPlayer;
    }

    private void YokaiBehaviour_OnKillPlayer(object sender, System.EventArgs e) {

        foreach (var script in disableScripts) {

            script.enabled = false;
        }

        Invoke(nameof(RespawnPlayer), 5);
    }

    private void RespawnPlayer() {

        foreach (var script in disableScripts) {

            script.enabled = true;
        }

        playerTransform.localPosition = Vector3.zero;
        playerTransform.eulerAngles = new Vector3(0, 180, 0);

        yokaiTransform.eulerAngles = Vector3.zero;
        yokaiTransform.localPosition = Vector3.zero;

        OnPlayerSpawn?.Invoke(this, EventArgs.Empty);
    }
}
