using System.Collections.Generic;
using UnityEngine;

public class YokaiBrain : MonoBehaviour {

    private static List<Transform> spawnPositions = new();

    private static Transform jumpscareDoorTransform;

    public static Transform GetJumpscareDoorTransform() => jumpscareDoorTransform;
    public static void SetJumpscareDoorTransform(Transform value) => jumpscareDoorTransform = value;

    public static void SetValidRoomSpawnPositions(List<Transform> positions) {

        spawnPositions.Clear();
        if (positions != null) {
            spawnPositions = positions;
        }
    }

    public static List<Transform> GetValidRoomSpawnPositions() {

        return spawnPositions;
    }

    private static List<Transform> SortList(List<Transform> spawns, Vector3 playerPosition) {

        int min_idx;

        // Sort list's positions based on their distance to player ...
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

        return spawns;
    }

    public static Transform SelectPosition(List<Transform> spawns, float minDistance, Vector3 playerPosition) {

        spawns = SortList(spawns, playerPosition);

        foreach (var spawnPoint in spawns) {

            float distanceFromPlayer = Vector3.Distance(spawnPoint.position, playerPosition);

            if (distanceFromPlayer > minDistance) {

                return spawnPoint;
            }
        }

        // If no preferable spawn point found, return a random spawn point:
        return spawns[Random.Range(0,spawns.Count)];
    }

    public static Vector3 CalculatePositionBehindPlayer(Transform playerTransform, Transform yokaiTransform, out Vector3 eulerAngles, out Vector3 lookDirection) {

        float backOffset = 1.2f;
        Vector3 directionTowardsPlayer = (playerTransform.position - yokaiTransform.position).normalized;
        Vector3 positionXZ = playerTransform.position - directionTowardsPlayer * backOffset;
        positionXZ.y = 0;
        Vector3 positionY = new Vector3 (0, playerTransform.position.y + 1, 0);
        Vector3 newYokaiPosition = positionXZ + positionY;

        Vector3 newEulerAngles = yokaiTransform.eulerAngles;
        newEulerAngles.x = newEulerAngles.z = 0;

        lookDirection = directionTowardsPlayer;
        eulerAngles = newEulerAngles;
        return newYokaiPosition;
    }
}