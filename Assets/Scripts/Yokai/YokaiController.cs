using System;
using System.Collections.Generic;
using UnityEngine;

public class YokaiController : MonoBehaviour {

    public event EventHandler OnYokaiJumpscare;

    #region Variables:
    [Header("--- Movement ---")]
    [SerializeField] private float rangeToKillPlayer;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float upstairsHallEventSpeed;
    [SerializeField] private float acceleration;

    [Header("--- Interact with Environment ---")]
    [SerializeField] private GameObject[] equipableItems;
    [SerializeField] private float throwForce;
    [Space(4)]
    [SerializeField] private GameObject[] doorObjects;
    [SerializeField] private Vector2 doorInteractTime;

    [Header("--- Chase ---")]
    [Tooltip("The minimum distance from player the Yokai can spawn")]
    [SerializeField] private float chaseMinSpawnDistance;
    [Tooltip("For how long should the Yokai chase before it despawns")]
    [SerializeField] private float chaseTime;
    [Tooltip("How long is the player allowed to stay in the same room as the Yokai, before the Yokai starts chasing him")]
    [SerializeField] private float sameRoomChaseTime;
    [Tooltip("How long should the Yokai stay in the room it spawned, when the player has left the room")]
    [SerializeField] private float despawnFromRoomTime;

    [Header("--- Chance To Spawn On Room Enter ---")]
    [SerializeField, Range(0, 100)] private int chanceToSpawn;
    [SerializeField] private List<Transform> spawnsFloor_1;
    [SerializeField] private List<Transform> spawnsFloor_2;

    [Header("--- Basement Event ---")]
    [SerializeField] private Transform sofaSitPosition;
    [SerializeField] private GameObject dungeonKey;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private GameObject pointLight;

    private YokaiBehaviour behaviour;
    private DungeonKeyItem dungeonKeyItem;
    private GameObject roomYokaiIsIn;
    private Transform yokaiTransform;
    private Transform playerTransform;
    private bool chasePlayer = false;
    private bool isChasing = false;
    private bool isSittingInSofa = false;
    private bool doorJumpscareEvent = false; 
    
    private float chaseTimer = 0;
    private float sameRoomChaseTimer = 0;
    private float despawnFromRoomTimer = 0;
    private float doorInteractTimer = 0, interactTime;
    public bool GetIsChasing() => isChasing;
    public bool GetIsSitting() => isSittingInSofa;
    #endregion

    private void Awake() {

        behaviour = FindObjectOfType<YokaiBehaviour>();
        dungeonKeyItem = FindObjectOfType<DungeonKeyItem>();
        yokaiTransform = behaviour.transform;

        interactTime = UnityEngine.Random.Range(doorInteractTime.x, doorInteractTime.y);
    }

    private void Start() {

        playerTransform = YokaiObserver.Instance.GetPlayerTransform();
        YokaiObserver.Instance.OnRunEventChase += Observer_OnRunEventChase;
        YokaiObserver.Instance.OnValidRoomEnter += Observer_OnValidRoomEnter;
        YokaiObserver.Instance.OnDoorOpenJumpscare += Observer_OnDoorOpen;
        YokaiObserver.Instance.OnUpstairsHallJumpscare += Observer_OnUpstairsHallJumpscare;
        YokaiObserver.Instance.OnBasementEventJumpscare += Observer_OnBasementEventJumpscare;
        dungeonKeyItem.OnDungeonKeyPickedUp += DungeonKeyItem_OnDungeonKeyPickedUp;
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
    }

    private void InteractWithDoors() {

        doorInteractTimer += Time.deltaTime;

        if (doorInteractTimer >= interactTime) {

            doorInteractTimer = 0;
            interactTime = UnityEngine.Random.Range(doorInteractTime.x, doorInteractTime.y);

            behaviour.OpenRandomDoor();
        }
    }

    private bool CanAttackPlayer() {

        bool nearPlayer = Vector3.Distance(transform.position, playerTransform.position) < rangeToKillPlayer;

        return nearPlayer && behaviour.IsActing() && !isSittingInSofa;
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

        if (randomNumber > 0 && randomNumber <= chanceToSpawn) {

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

            case 1:
            spawns = spawnsFloor_1;
            break;
            case 2:
            spawns = spawnsFloor_2;
            break;
        }

        spawnPosition = YokaiBrain.SelectPosition(spawns, chaseMinSpawnDistance, playerPosition);

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

        isSittingInSofa = true;
        behaviour.SetStats(0, 0, 0, false);
        behaviour.SpawnAtPosition(sofaSitPosition, false);
        dungeonKey.SetActive(true);
        spotLight.SetActive(true);
        pointLight.SetActive(true);

    }

    private void DungeonKeyItem_OnDungeonKeyPickedUp(object sender, EventArgs e) {

        isSittingInSofa = false;
        spotLight.SetActive(false);
        pointLight.SetActive(false);
        behaviour.DespawnCharacter();
    }

    private void Observer_OnDoorOpen(object sender, System.EventArgs e) {

        Transform jumpscareTransform = YokaiBrain.GetJumpscareDoorTransform();

        behaviour.SpawnAtPosition(jumpscareTransform);
        behaviour.RunTowardsPosition(jumpscareTransform.position);
        doorJumpscareEvent = true;
        Invoke(nameof(Disappear), 4);

        OnYokaiJumpscare?.Invoke(this, EventArgs.Empty);
    }

    private void Disappear() {

        doorJumpscareEvent = false;
        behaviour.DespawnCharacter();
    }
}
