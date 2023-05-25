using System;
using UnityEngine;

public class YokaiObserver : MonoBehaviour {

    public event EventHandler OnRunEventWarning;
    public event EventHandler OnRunEventChase;

    [Header("--- Library Event ---")]
    [SerializeField] private Transform[] jumpscareDoorsTransform;
    [SerializeField] private Transform runTowards;

    [Space(10)]

    [Header("--- Running Event ---")]
    [SerializeField] private float yokaiWarnInSeconds;
    [SerializeField] private float yokaiChaseInSeconds;

    private Transform playerTransform;
    private PlayerMovement playerMovement;
    private bool canFireOnRunEvent = true;
    private bool canFireOnChaseEvent = true;
    private float playerRunningTimer = 0;
    private float yokaiChaseTimer = 0;

    private void Awake() {

        playerMovement = FindObjectOfType<PlayerMovement>();
        playerTransform = playerMovement.gameObject.transform;
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

    /*private void StartTimer(float timer, float maxTime, out bool canFireEvent) {

        canFireEvent = false;
        timer += Time.deltaTime;

        if (timer >= maxTime) {

            canFireEvent = true;
        }
    }*/

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

    public int PlayerFloorIndex() {

        float playerY = playerTransform.position.y;

        bool dungeon    = playerY >= -4.97f && playerY < -2.90f;
        bool basement   = playerY >= -2.90f && playerY < 0.00f;
        bool floor1     = playerY >=  0.00f && playerY < 4.08f;
        bool floor2     = playerY >=  4.08f && playerY < 6.79f;
        bool floor3     = playerY >=  6.79f && playerY < 10.00f;

        if (dungeon)
            return -1;
        else if (basement)
            return 0;
        else if (floor1)
            return 1;
        else if (floor2)
            return 2;
        else if (floor3)
            return 3;

        // Game bugged
        return -100;
    }
}
