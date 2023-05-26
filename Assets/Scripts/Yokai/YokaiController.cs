using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class YokaiController : MonoBehaviour {

    #region Variables:
    [Header("--- Movement ---")]
    [SerializeField] private float rangeToKillPlayer;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [Header("--- Items ---")]
    [SerializeField] private GameObject[] equipableItems;
    [SerializeField] private float throwForce;

    [Header("--- Chase ---")]
    [SerializeField] private float chaseMinSpawnDistance;
    [SerializeField] private float chaseTime;

    [Header("--- Chance To Spawn On Room Enter ---")]
    [SerializeField] private int chanceToSpawn;
    [SerializeField] private List<Transform> spawnsFloor_1;
    [SerializeField] private List<Transform> spawnsFloor_2;

    private YokaiBehaviour behaviour;
    private float chaseTimer = 0;
    private bool chasePlayer = false;
    private bool isChasing = false;
    public bool GetIsChasing() => isChasing;
    #endregion

    private void Awake() {

        behaviour = FindObjectOfType<YokaiBehaviour>();
    }

    private void Start() {

        Door[] doors = FindObjectsOfType<Door>();

        foreach (var door in doors) {
            if (door.GetFireEvent()) {
                door.OnDoorOpen += Door_OnDoorOpen;
            }
        }
        YokaiObserver.Instance.OnRunEventChase += Observer_OnRunEventChase;
        YokaiObserver.Instance.OnValidRoomEnter += Observer_OnValidRoomEnter;
    }

    private void Observer_OnValidRoomEnter(object sender, System.EventArgs e) {

        if (behaviour.IsActing()) {
            return;
        }

        int randomNumber = Random.Range(0, 101);

        if (randomNumber >= 0 && randomNumber <= chanceToSpawn) {
                
            behaviour.DespawnCharacter();
            List<Transform> validSpawns = YokaiBrain.GetValidSpawnPositions();

            if (validSpawns.Count != 0) {

                int randomIndex = Random.Range(0, validSpawns.Count);

                behaviour.SpawnAtPosition(validSpawns[randomIndex]);
                Invoke(nameof(Temp), 5);
            }
        }
    }

    private void Observer_OnRunEventChase(object sender, System.EventArgs e) {

        isChasing = true;
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

    private void Update() {

        if (chasePlayer) {

            behaviour.ChasePlayer(rangeToKillPlayer);
            chaseTimer += Time.deltaTime;

            if (chaseTimer > chaseTime) {

                chasePlayer = false;
                isChasing = false;
                chaseTimer = 0;

                behaviour.DespawnCharacter();
            }
        }
    }

    private void Temp() {

        behaviour.DespawnCharacter();
    }

    private void Door_OnDoorOpen(object sender, Door.OnDoorOpenEventArgs e) {

        if (e.isOpen) {

            behaviour.SpawnAtPosition(e.jumpscareTransform);
            behaviour.RunTowardsPosition(e.jumpscareTransform.position);
        }
    }
}
