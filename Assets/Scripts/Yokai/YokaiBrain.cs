using System.Collections.Generic;
using UnityEngine;

public class YokaiBrain : MonoBehaviour {

    private static List<Transform> spawnPositions = new();

    public void SetValidSpawnPositions(List<Transform> positions) {

        spawnPositions.Clear();
        if (positions != null) {
            spawnPositions = positions;
        }
    }

    public static List<Transform> GetValidSpawnPositions() {

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
}