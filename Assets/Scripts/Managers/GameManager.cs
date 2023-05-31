using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnPlayerSpawn;
    public event EventHandler OnGameCompleted;

    [Header("--- Game Completion ---")]
    [SerializeField] private GameObject[] entranceDoors = new GameObject[2];
    [SerializeField] private int totalMasksInGame;
    [SerializeField] private FadeScene fadeScene;
    [SerializeField] private float timeToRestart;

    private YokaiBehaviour yokaiBehaviour;
    private YokaiController yokaiController;
    private PlayerController playerController;
    private InputManager inputManager;
    private CinemachineInputProvider inputProvider;
    private PlayerMovement playerMovement;
    private GameTimer timer;

    private Transform playerTransform;
    private Transform yokaiTransform;

    private int masksCollected = 0;
    private bool playerCanRespawn = true; // Game completed if false

    private List<MonoBehaviour> disableScripts = new();

    private void Awake() {

        Instance = this;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        disableScripts.Add(yokaiBehaviour = FindObjectOfType<YokaiBehaviour>());
        disableScripts.Add(playerController = FindObjectOfType<PlayerController>());
        disableScripts.Add(inputManager = FindObjectOfType<InputManager>());
        disableScripts.Add(inputProvider = FindObjectOfType<CinemachineInputProvider>());
        disableScripts.Add(yokaiController = FindObjectOfType<YokaiController>());
        playerMovement = FindObjectOfType<PlayerMovement>();

        playerTransform = playerController.transform;
        yokaiTransform = yokaiBehaviour.transform;
    }

    private void Start() {

        disableScripts.Add(YokaiObserver.Instance);
        yokaiBehaviour.OnKillPlayer += YokaiBehaviour_OnKillPlayer;
        playerController.OnMaskItemCollected += PlayerController_OnMaskItemCollected;
        playerMovement.OnMansionExit += PlayerMovement_OnMansionExit;
    }

    private void PlayerMovement_OnMansionExit(object sender, EventArgs e) {

        OnGameCompleted?.Invoke(this, EventArgs.Empty);
        playerCanRespawn = false;
        Invoke(nameof(RestartGame), timeToRestart);
    }

    private void RestartGame() {

        fadeScene.ReloadScene(false);
    }

    private void PlayerController_OnMaskItemCollected(object sender, EventArgs e) {

        masksCollected++;

        if (masksCollected == totalMasksInGame) {
            
            foreach (var door in entranceDoors) {

                door.GetComponent<Door>().SetNeedsKey(false);
                door.GetComponent<IInteractable>().Interact();
            }
        }
    }

    private void YokaiBehaviour_OnKillPlayer(object sender, System.EventArgs e) {

        foreach (var script in disableScripts) {

            script.enabled = false;
        }

        if (playerCanRespawn) {

            Invoke(nameof(RespawnPlayer), 5);
        }
    }

    private void RespawnPlayer() {

        foreach (var script in disableScripts) {

            script.enabled = true;
        }

        GameObject player = GameObject.Find("Player");
        GameObject spawn = GameObject.Find("PlayerSpawn");
        player.SetActive(false);
        player.transform.position = spawn.transform.position;
        player.transform.rotation = spawn.transform.rotation;
        player.SetActive(true);
        yokaiTransform.eulerAngles = Vector3.zero;
        yokaiTransform.localPosition = Vector3.zero;

        OnPlayerSpawn?.Invoke(this, EventArgs.Empty);
    }
}
