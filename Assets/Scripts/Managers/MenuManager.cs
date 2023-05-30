using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    [Header("--- Start Game ---")]
    [SerializeField] private float delayGameStart = 1.85f;
    [SerializeField] private GameObject crosshairCanvas;
    [SerializeField] CinemachineVirtualCamera mainVirtualCamera;
    [SerializeField] CinemachineInputProvider inputProvider;
    [SerializeField] GameObject cameraUI;
    [SerializeField] AudioSource ambientRain;
    [SerializeField] private float ambientRainGameplayPitch = 0.75f;
    [SerializeField] private float ambientRainMenuPitch = 2.5f;
    [SerializeField] AudioSource ambientEnv;

    [Header("--- How To Play ---")]
    [SerializeField] private RectTransform howToPlayUIObject;
    [SerializeField] private float timeToFade;

    [Header("--- Pause Game ---")]
    [SerializeField] private GameObject pauseMenu;

    private bool fadeHowToPlay = false;
    private bool startGame = false;
    private bool pauseGame = false;

    private void Awake() {

        Time.timeScale = 0;
    }

    private void Update() {

        FadeHowToPlay();

        if (!startGame) {
            return;
        }

        Cursor.visible = pauseGame;
        CursorLockMode mode = pauseGame ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.lockState = mode;

        if (Input.GetKeyDown(KeyCode.Escape)) {

            ResumeGame();
        }
    }

    public void StartGame() {

        mainVirtualCamera.Priority = 10;
        Invoke(nameof(GameState), delayGameStart);
        Time.timeScale = 1;
    }

    public void HowToPlay() {

        fadeHowToPlay = !fadeHowToPlay;
    }

    public void ExitGame() {

        Application.Quit();
    }

    public void ResumeGame() {

        pauseGame = !pauseGame;
        Time.timeScale = Convert.ToInt32(!pauseGame);
        inputProvider.enabled = !pauseGame;
        pauseMenu.SetActive(pauseGame);
    }

    private void FadeHowToPlay() {

        Vector3 currentScale = howToPlayUIObject.localScale;
        Vector3 targetScale = fadeHowToPlay ? Vector3.one : Vector3.zero;

        howToPlayUIObject.localScale = Vector3.Lerp(currentScale, targetScale, timeToFade * Time.unscaledDeltaTime);
    }

    private void GameState() {

        startGame = !startGame;
        cameraUI.SetActive(startGame);
        inputProvider.enabled = startGame;
        crosshairCanvas.SetActive(startGame);

        float pitch = startGame ? ambientRainGameplayPitch : ambientRainMenuPitch;
        ambientRain.pitch = pitch;

        if (!ambientEnv.isPlaying) {
            ambientEnv.Play();
        }
    }
}
