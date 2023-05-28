using System;
using System.Collections.Generic;
using UnityEngine;

public class YokaiObserver : MonoBehaviour {

    public static YokaiObserver Instance { get; private set; }

    public class OnValidRoomEnterEventArgs { 
        public GameObject room;
    }
    public class OnUpstairsHallJumpscareEventArgs {

        public Transform startTransform;
        public Transform endTransform;
    }

    public event EventHandler OnRunEventWarning;
    public event EventHandler OnRunEventChase;
    public event EventHandler OnDoorOpenJumpscare;
    public event EventHandler OnBasementEventJumpscare;
    public event EventHandler OnBasementEventComplete;
    public event EventHandler<OnUpstairsHallJumpscareEventArgs> OnUpstairsHallJumpscare;
    public event EventHandler<OnValidRoomEnterEventArgs> OnValidRoomEnter;

    [Header("--- Running Event ---")]
    [SerializeField] private float yokaiWarnInSeconds;
    [SerializeField] private float yokaiChaseInSeconds;

    [Header("--- Basement Event ---")]
    [SerializeField] private GameObject dungeonKey;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private GameObject pointLight;

    [Header("--- Upstairs Hall Event ---")]
    [SerializeField, Range(0,100)] private float chanceToTrigger;
    [SerializeField] private GameObject[] yokaiPathHolders;

    private Transform playerTransform;
    private PlayerMovement playerMovement;
    private YokaiController controller;
    private DungeonKeyItem dungeonKeyItem;

    private GameObject roomPlayerIsIn;
    private bool canFireOnRunEvent = true;
    private bool canFireOnChaseEvent = true;
    private float playerRunningTimer = 0;
    private float yokaiChaseTimer = 0;
    private bool canTriggerBasementEvent = false;
    private bool basementEventActive = false;

    public Transform GetPlayerTransform() => playerTransform;
    public GameObject GetRoomPlayerIsIn() => roomPlayerIsIn;

    private void Awake() {

        Instance = this;
        playerMovement = FindObjectOfType<PlayerMovement>();
        controller = FindObjectOfType<YokaiController>();
        dungeonKeyItem = FindObjectOfType<DungeonKeyItem>();
        playerTransform = playerMovement.gameObject.transform;
    }

    private void Start() {

        playerMovement.OnRoomEnter += PlayerMovement_OnRoomEnter;
        playerMovement.OnUpstairsHallEvent += PlayerMovement_OnUpstairsHallEvent;
        playerMovement.OnFoundDungeonDoor += PlayerMovement_OnFoundDungeonDoor;
        dungeonKeyItem.OnDungeonKeyPickedUp += DungeonKeyItem_OnDungeonKeyPickedUp;

        Door[] doors = FindObjectsOfType<Door>();

        foreach (var door in doors) {
            if (door.GetFireEvent()) {
                door.OnDoorOpen += Door_OnDoorOpen;
            }
        }
    }

    private void Update() {

        PlayerRun_ChaseEvent();
    }

    private void PlayerMovement_OnFoundDungeonDoor(object sender, EventArgs e) {

        basementEventActive = true;
        canTriggerBasementEvent = true;
        playerMovement.OnFoundDungeonDoor -= PlayerMovement_OnFoundDungeonDoor;
    }

    private void Door_OnDoorOpen(object sender, Door.OnDoorOpenEventArgs e) {

        if (controller.GetIsChasing() || controller.GetIsBehindPlayer()) {
            return;
        }

        bool playerInsideEventRoom = Vector3.Dot(e.jumpscareTransform.forward, playerTransform.forward) > 0;

        if (playerInsideEventRoom) {
            return;
        }

        if (e.isOpen) {

            YokaiBrain.SetJumpscareDoorTransform(e.jumpscareTransform);

            Door[] doors = FindObjectsOfType<Door>();
            foreach (var door in doors) {
                door.OnDoorOpen -= Door_OnDoorOpen;
            }

            OnDoorOpenJumpscare?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PlayerMovement_OnUpstairsHallEvent(object sender, EventArgs e) {

        GameObject selectedSide = null;
        Transform startTransform;
        Transform endTransform;

        int chance = UnityEngine.Random.Range(0, 101);

        if (chance > 0 && chance <= chanceToTrigger) {

            foreach (var side in yokaiPathHolders) {

                bool playerLooksSide = Vector3.Dot(side.transform.forward, playerTransform.forward) < 0;

                if (playerLooksSide) {

                    selectedSide = side;
                    break;
                }
            }

            startTransform = selectedSide.transform.GetChild(0);
            endTransform = selectedSide.transform.GetChild(1);

            OnUpstairsHallJumpscare?.Invoke(this, new OnUpstairsHallJumpscareEventArgs {

                startTransform = startTransform,
                endTransform = endTransform
            });
        }
    }

    private void PlayerMovement_OnRoomEnter(object sender, PlayerMovement.OnRoomEnterEventArgs e) {

        roomPlayerIsIn = e.currentRoom;
        GameObject spawnsHolder = GameObject.Find(e.currentRoom.name + "_Spawns");

        if (spawnsHolder) {

            List<Transform> validSpawns = new();

            for (int i = 0; i < spawnsHolder.transform.childCount; i++) {

                validSpawns.Add(spawnsHolder.transform.GetChild(i));
            }

            YokaiBrain.SetValidRoomSpawnPositions(validSpawns);
            OnValidRoomEnter?.Invoke(this, new OnValidRoomEnterEventArgs {
                room = e.currentRoom
            });
        }
        else {

            YokaiBrain.SetValidRoomSpawnPositions(null);
        }

        // if player enters the "BasementEntrance" observer will fire an event
        TriggerBasementEvent(e.currentRoom);


        //Debug.Log("Floor: " + PlayerFloorIndex() + " -- Room: " + e.currentRoom.name);
    }

    private void TriggerBasementEvent(GameObject currentRoom) {

        if (canTriggerBasementEvent) {

            if (currentRoom.name == "BasementEntrance") {

                canTriggerBasementEvent = false;
                dungeonKey.SetActive(true);
                spotLight.SetActive(true);
                pointLight.SetActive(true);
                OnBasementEventJumpscare?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void DungeonKeyItem_OnDungeonKeyPickedUp(object sender, EventArgs e) {

        basementEventActive = false;
        spotLight.SetActive(false);
        pointLight.SetActive(false);
        OnBasementEventComplete?.Invoke(this, EventArgs.Empty);
    }

    private bool PlayerRun_WarningEvent() {

        bool playerIsRunning = playerMovement.GetIsRunning();
        bool yokaiIsChasing = controller.GetIsChasing();

        if (playerIsRunning && !yokaiIsChasing && !basementEventActive) {

            playerRunningTimer += Time.deltaTime;

            if (playerRunningTimer > yokaiWarnInSeconds) {

                // Warn player that chase event will happen if he/she continues running
                if (canFireOnRunEvent) {

                    OnRunEventWarning?.Invoke(this, EventArgs.Empty);
                    canFireOnRunEvent = false;
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

    private void PlayerRun_ChaseEvent() {

        if (PlayerRun_WarningEvent()) {

            yokaiChaseTimer += Time.deltaTime;

            if (yokaiChaseTimer > yokaiChaseInSeconds) {

                // Inform that yoikai should start chasing
                if (canFireOnChaseEvent) {

                    OnRunEventChase?.Invoke(this, EventArgs.Empty);
                    canFireOnChaseEvent = false;
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
        Debug.LogWarning("YokaiObserver: Is your character flying? :O");
        return -100;
    }
}
