using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class YokaiController : MonoBehaviour {

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

    [Header("--- Next Action ---")]
    [SerializeField] private float chooseActionInSeconds;
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    [SerializeField] private List<Transform> spawnsFloor_1;
    [SerializeField] private List<Transform> spawnsFloor_2;

    private YokaiBehaviour behaviour;
    private float chooseActionTimer;

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
    }

    private void Observer_OnRunEventChase(object sender, System.EventArgs e) {

        int floorIndex = YokaiObserver.Instance.PlayerFloorIndex();
        Debug.Log(floorIndex);
        Transform playerTransform = YokaiObserver.Instance.GetPlayerTransform();
        Vector3 playerPosition = playerTransform.position;
        Transform spawnPosition = null;
        int min_idx;

        List<Transform> spawns = new();

        switch (floorIndex) {

            case 1:
            spawns = spawnsFloor_1;
            break;
            case 2:
            spawns = spawnsFloor_2;
            break;
        }

        // Sort list based on distance to the player ...
        for (int i = 0; i < spawns.Count - 1; i++) {

            min_idx = i;

            for (int j = i + 1; j < spawns.Count; j++) {

                if (Vector3.Distance(spawns[j].position, playerPosition) < Vector3.Distance(spawns[min_idx].position, playerPosition)) {

                    min_idx = j;
                }
            }

            if (min_idx != i) {

                (spawns[i], spawns[min_idx]) = (spawns[min_idx], spawns[i]);
            }
        }

        /*for (int i = 0; i < spawns.Count; i++) {

            Debug.Log(Vector3.Distance(spawns[i].position, playerPosition));
        }*/

        // ... Pick a spawnPoint based on the chaseMinSpawnDistance
        foreach (var spawnPoint in spawns) {

            float distanceFromPlayer = Vector3.Distance(spawnPoint.position, playerPosition);

            if (distanceFromPlayer > chaseMinSpawnDistance/* && Vector3.Dot(playerTransform.forward, (playerPosition - spawnPoint.position).normalized) < 0*/) {

                spawnPosition = spawnPoint;
                break;
            }
            else {

                spawnPosition = spawns[^3];
            }
        }

        behaviour.SpawnAtPosition(spawnPosition);
        //behaviour.ChasePlayer(rangeToKillPlayer);
    }

    private void Update() {

        if (ChooseNextAction()) {

            behaviour.RandomBehaviour();
        }
    }

    private void Door_OnDoorOpen(object sender, Door.OnDoorOpenEventArgs e) {

        if (e.isOpen) {

            behaviour.SpawnAtPosition(e.jumpscareTransform);
            behaviour.RunTowardsPosition(e.jumpscareTransform.position);
        }
    }

    private bool ChooseNextAction() {

        chooseActionTimer += Time.deltaTime;

        return chooseActionTimer >= chooseActionInSeconds;
    }
}
