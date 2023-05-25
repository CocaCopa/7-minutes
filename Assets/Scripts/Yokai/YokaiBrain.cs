using System.Collections.Generic;
using UnityEngine;

public class YokaiBrain : MonoBehaviour {

    private YokaiObserver observer;
    private List<Transform> spawnPositions = new();
    private int floorIndex;

    private void Awake() {

        observer = FindObjectOfType<YokaiObserver>();
    }

    private void Update() {
        
        floorIndex = observer.PlayerFloorIndex();
    }

    public void SetValidSpawnPositions(List<Transform> positions) {

        spawnPositions.Clear();
        if (positions != null) {
            spawnPositions = positions;
        }
    }

    public List<Transform> GetValidSpawnPositions() {

        return spawnPositions;
    }
}