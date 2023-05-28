using System;
using System.Collections.Generic;
using UnityEngine;

public class YokaiController : MonoBehaviour {

    public event EventHandler OnYokaiJumpscare;
    public event EventHandler OnSpawnBehindPlayer;

    #region Variables:
    [Header("--- Movement ---")]
    [SerializeField] private float rangeToKillPlayer;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float upstairsHallEventSpeed;
    [SerializeField] private float acceleration;

    [Header("--- Interact with Environment ---")]
    [SerializeField] private Vector2 environmentInteractTime;

    [Header("--- Chase ---")]
    [Tooltip("The minimum distance from player the Yokai can spawn")]
    [SerializeField] private float chaseMinSpawnDistance;
    [Tooltip("For how long should the Yokai chase before it despawns")]
    [SerializeField] private float chaseTime;
    [Tooltip("How long is the player allowed to stay in the same room as the Yokai, before the Yokai starts chasing him")]
    [SerializeField] private float sameRoomChaseTime;
    [Tooltip("How long should the Yokai stay in the room it spawned, when the player has left the room")]
    [SerializeField] private float despawnFromRoomTime;
    [SerializeField] private List<Transform> spawnsFloor_0;
    [SerializeField] private List<Transform> spawnsFloor_1;
    [SerializeField] private List<Transform> spawnsFloor_2;

    [Header("--- Spawn Behind Player ---")]
    [SerializeField, Range(0, 100)] private float chanceToSpawnBehindPlayer;
    [Tooltip("Roll the dice to spawn behind player every 'x' seconds")]
    [SerializeField] private float chanceTime;
    [SerializeField] private float stopFollowingTime;

    [Header("--- On Room Enter ---")]
    [SerializeField, Range(0, 100)] private int chanceToSpawnOnRoomEnter;

    [Header("--- Basement Event ---")]
    [SerializeField] private Transform sofaSitPosition;

    [Header("-- On Door Open Event ---")]
    [SerializeField] private float timeToDisappearFromDoor;


    private YokaiBehaviour behaviour;
    private GameObject roomYokaiIsIn;
    private Transform yokaiTransform;
    private Transform playerTransform;

    private bool chasePlayer = false;
    private bool doorJumpscareEvent = false;
    private bool isChasing = false;
    private bool isBehindPlayer = false;
    
    private float chaseTimer = 0;
    private float sameRoomChaseTimer = 0;
    private float despawnFromRoomTimer = 0;
    private float environmentInteractTimer = 0, interactTime;
    private float chanceTimer = 0;
    private float stopFollowingTimer = 0;
    public bool GetIsChasing() => isChasing;
    public bool GetIsBehindPlayer() => isBehindPlayer;
    #endregion

    private void Awake() {

        behaviour = FindObjectOfType<YokaiBehaviour>();
        yokaiTransform = behaviour.transform;

        interactTime = UnityEngine.Random.Range(environmentInteractTime.x, environmentInteractTime.y);
    }

    private void Start() {

        playerTransform = YokaiObserver.Instance.GetPlayerTransform();
        YokaiObserver.Instance.OnRunEventChase += Observer_OnRunEventChase;
        YokaiObserver.Instance.OnValidRoomEnter += Observer_OnValidRoomEnter;
        YokaiObserver.Instance.OnDoorOpenJumpscare += Observer_OnDoorOpen;
        YokaiObserver.Instance.OnUpstairsHallJumpscare += Observer_OnUpstairsHallJumpscare;
        YokaiObserver.Instance.OnBasementEventJumpscare += Observer_OnBasementEventJumpscare;
        YokaiObserver.Instance.OnBasementEventComplete += Observer_OnBasementEventComplete;
    }

    private void Update() {

        if (!doorJumpscareEvent) {

            ChasePlayerAction();
            SpawnedInRoomAction();
        }

        if (CanAttackPlayer()) {

            behaviour.KillPlayer();
        }

        if (!behaviour.IsActing()) {

            InteractWithDoors();
        }

        SpawnBehindPlayer();
    }

    private void SpawnBehindPlayer() {

        if (!behaviour.IsActing() && !YokaiObserver.Instance.GetBasementEventActive()) {

            chanceTimer += Time.deltaTime;

            if (chanceTimer >= chanceTime) {

                chanceTimer = 0;
                int spawnChance = UnityEngine.Random.Range(0, 101);

                if (spawnChance > 0 && spawnChance < chanceToSpawnBehindPlayer) {

                    Transform playerTransform = YokaiObserver.Instance.GetPlayerTransform();
                    behaviour.SpawnAtPosition(playerTransform, false);
                    yokaiTransform.position = playerTransform.position - playerTransform.forward * 1.2f;
                    isBehindPlayer = true;
                    OnSpawnBehindPlayer?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        if (behaviour.IsActing() && isBehindPlayer) {

            yokaiTransform.position = playerTransform.position - (playerTransform.position - yokaiTransform.position).normalized * 1.2f;
            yokaiTransform.forward = (playerTransform.position - yokaiTransform.position).normalized;
            Vector3 newHeight = yokaiTransform.position;
            newHeight.y = playerTransform.position.y + 1;
            yokaiTransform.position = newHeight;
            Vector3 newEulerAngles = yokaiTransform.eulerAngles;
            newEulerAngles.x = newEulerAngles.z = 0;
            yokaiTransform.eulerAngles = newEulerAngles;

            stopFollowingTimer += Time.deltaTime;

            if (stopFollowingTimer > stopFollowingTime) {

                stopFollowingTimer = 0;
                isBehindPlayer = false;
                behaviour.DespawnCharacter();
            }
        }
    }

    private void InteractWithDoors() {

        environmentInteractTimer += Time.deltaTime;

        if (environmentInteractTimer >= interactTime) {

            environmentInteractTimer = 0;
            interactTime = UnityEngine.Random.Range(environmentInteractTime.x, environmentInteractTime.y);

            behaviour.RandomBehaviour();
        }
    }

    private bool CanAttackPlayer() {

        bool nearPlayer = Vector3.Distance(transform.position, playerTransform.position) < rangeToKillPlayer;

        return nearPlayer && behaviour.IsActing() && !YokaiObserver.Instance.GetBasementEventActive();
    }

    private void ChasePlayerAction() {

        if (chasePlayer) {

            isChasing = true;
            chaseTimer += Time.deltaTime;

            behaviour.SetStats(runSpeed, acceleration, 4);
            behaviour.ChasePlayer();

            if (chaseTimer > chaseTime) {

                chasePlayer = false;
                isChasing = false;
                chaseTimer = 0;

                behaviour.DespawnCharacter();
            }
        }
    }

    private void SpawnedInRoomAction() {

        if (!isChasing) {

            bool playerInTheSameRoom = YokaiObserver.Instance.GetRoomPlayerIsIn() == roomYokaiIsIn;
            bool playerLeftTheRoom = roomYokaiIsIn != null && YokaiObserver.Instance.GetRoomPlayerIsIn() != roomYokaiIsIn;

            if (playerInTheSameRoom) {

                sameRoomChaseTimer += Time.deltaTime;
                despawnFromRoomTimer = 0;

                if (sameRoomChaseTimer >= sameRoomChaseTime) {

                    roomYokaiIsIn = null;
                    sameRoomChaseTimer = 0;
                    chasePlayer = true;
                }
            }
            else if (playerLeftTheRoom) {

                despawnFromRoomTimer += Time.deltaTime;
                sameRoomChaseTimer = 0;

                if (despawnFromRoomTimer >= despawnFromRoomTime) {

                    roomYokaiIsIn = null;
                    despawnFromRoomTimer = 0;
                    behaviour.DespawnCharacter();
                }
            }
        }
        else {
            roomYokaiIsIn = null;
            despawnFromRoomTimer = 0;
            sameRoomChaseTimer = 0;
        }
    }

    private void Observer_OnValidRoomEnter(object sender, YokaiObserver.OnValidRoomEnterEventArgs e) {

        if (isChasing || behaviour.IsActing()) {
            return;
        }

        int randomNumber = UnityEngine.Random.Range(0, 101);

        if (randomNumber > 0 && randomNumber <= chanceToSpawnOnRoomEnter) {

            behaviour.DespawnCharacter(); // For safery, make sure that the character is not in then scene before calling SpawnAtPosition()
            roomYokaiIsIn = e.room;
            List<Transform> validSpawns = YokaiBrain.GetValidRoomSpawnPositions();
            int randomIndex = UnityEngine.Random.Range(0, validSpawns.Count);
            behaviour.SpawnAtPosition(validSpawns[randomIndex]);
        }
    }

    private void Observer_OnRunEventChase(object sender, System.EventArgs e) {

        int floorIndex = YokaiObserver.Instance.PlayerFloorIndex();
        Transform playerTransform = YokaiObserver.Instance.GetPlayerTransform();
        Vector3 playerPosition = playerTransform.position;

        Transform spawnPosition;

        if (behaviour.IsActing()) {

            chasePlayer = true;
            return;
        }

        List<Transform> spawns = new();

        switch (floorIndex) {

            case -1:
            spawns = spawnsFloor_0;
            break;
            case 0:
            spawns = spawnsFloor_0;
            break;
            case 1:
            spawns = spawnsFloor_1;
            break;
            case 2:
            spawns = spawnsFloor_2;
            break;
        }

        spawnPosition = YokaiBrain.SelectPosition(spawns, chaseMinSpawnDistance, playerPosition);

        behaviour.SetStats(runSpeed, acceleration / 2, 0, true);
        behaviour.SpawnAtPosition(spawnPosition);
        chasePlayer = true;
    }

    private void Observer_OnUpstairsHallJumpscare(object sender, YokaiObserver.OnUpstairsHallJumpscareEventArgs e) {

        if (behaviour.IsActing()) {
            return;
        }

        Transform spawnTransform = e.startTransform;
        Vector3 goToPosition = e.endTransform.position;

        behaviour.SetStats(upstairsHallEventSpeed, acceleration, 1, true);
        behaviour.SpawnAtPosition(spawnTransform);
        behaviour.RunTowardsPosition(goToPosition, true);
    }

    private void Observer_OnBasementEventJumpscare(object sender, EventArgs e) {

        behaviour.SetStats(0, 0, 0, false);
        behaviour.SpawnAtPosition(sofaSitPosition, false);
    }

    private void Observer_OnBasementEventComplete(object sender, EventArgs e) {

        behaviour.DespawnCharacter();
    }

    private void Observer_OnDoorOpen(object sender, System.EventArgs e) {

        Transform jumpscareTransform = YokaiBrain.GetJumpscareDoorTransform();

        behaviour.SpawnAtPosition(jumpscareTransform);
        behaviour.RunTowardsPosition(jumpscareTransform.position);
        doorJumpscareEvent = true;
        Invoke(nameof(DisableJumpScareEvent), timeToDisappearFromDoor);

        OnYokaiJumpscare?.Invoke(this, EventArgs.Empty);
    }

    private void DisableJumpScareEvent() {

        doorJumpscareEvent = false;
        behaviour.DespawnCharacter();
    }
}
