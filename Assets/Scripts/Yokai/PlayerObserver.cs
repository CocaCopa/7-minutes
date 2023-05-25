using System;
using UnityEngine;

public class PlayerObserver : MonoBehaviour {

    public event EventHandler OnRunEventWarning;
    public event EventHandler OnRunEventChase;

    [Header("--- Library Event ---")]
    [SerializeField] private Transform[] jumpscareDoorsTransform;
    [SerializeField] private Transform runTowards;

    [Space(10)]

    [Header("--- Running Event ---")]
    [SerializeField] private float yokaiWarnInSeconds;
    [SerializeField] private float yokaiChaseInSeconds;

    private bool canFireOnRunEvent = true;
    private bool canFireOnChaseEvent = true;
    private float playerRunningTimer = 0;
    private float yokaiChaseTimer = 0;
    private PlayerMovement playerMovement;

    private void Awake() {

        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void Start() {

        foreach (var door in jumpscareDoorsTransform) {

            door.GetComponent<Door>().OnDoorOpen += PlayerObserver_OnDoorOpen;
        }
    }

    private void Update() {

        ChasePlayerIfRunning();
    }

    private void PlayerObserver_OnDoorOpen(object sender, Door.OnDoorOpenEventArgs e) {

        // -Observer knows that one of the doors opened
        // -Event arguments will tell it which door it is and which position should be
        // used to spawn the Yokai
    }

    private bool PlayerIsRunningForTooLong() {

        bool playerIsRunning = playerMovement.IsRunning();

        if (playerIsRunning) {

            playerRunningTimer += Time.deltaTime;

            if (playerRunningTimer > yokaiWarnInSeconds) {

                // Warn player that chase event will happen if he/she continues running
                if (canFireOnRunEvent) {

                    canFireOnRunEvent = false;
                    OnRunEventWarning?.Invoke(this, EventArgs.Empty);
                }
                return true;
            }
        }
        else {

            canFireOnRunEvent = true;
            playerRunningTimer = 0;
        }

        return false;
    }

    private void ChasePlayerIfRunning() {

        if (PlayerIsRunningForTooLong()) {

            yokaiChaseTimer += Time.deltaTime;

            if (yokaiChaseTimer > yokaiChaseInSeconds) {

                // Inform that yoikai should start chasing
                if (canFireOnChaseEvent) {

                    canFireOnChaseEvent = false;
                    OnRunEventChase?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        else {

            canFireOnChaseEvent = true;
            yokaiChaseTimer = 0;
        }
    }
}
