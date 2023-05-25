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
    [SerializeField] private List<Transform> allSpawnTransforms;

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

        Vector3 playerPosition = YokaiObserver.Instance.GetPlayerTransform().position;
        Transform spawnPosition = null;
        int min_idx;

        for (int i = 0; i < allSpawnTransforms.Count - 1; i++) {

            min_idx = i;

            for (int j = i + 1; j < allSpawnTransforms.Count; j++) {

                if (Vector3.Distance(allSpawnTransforms[j].position, playerPosition) < Vector3.Distance(allSpawnTransforms[min_idx].position, playerPosition)) {

                    min_idx = j;
                }
            }

            if (min_idx != i) {

                (allSpawnTransforms[i], allSpawnTransforms[min_idx]) = (allSpawnTransforms[min_idx], allSpawnTransforms[i]);
            }
        }

        foreach (var spawnPoint in allSpawnTransforms) {

            float distanceFromPlayer = Vector3.Distance(spawnPoint.position, playerPosition);

            if (distanceFromPlayer > chaseMinSpawnDistance) {

                spawnPosition = spawnPoint;
                break;
            }
            else {

                spawnPosition = allSpawnTransforms[allSpawnTransforms.Count - 3];
            }
        }

        behaviour.SpawnAtPosition(spawnPosition);
        behaviour.ChasePlayer(rangeToKillPlayer);
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
