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

    public void SetValidSpawnPositions(Transform[] positions) {

        spawnPositions.Clear();

        foreach (var position in positions) {

            spawnPositions.Add(position);
        }
}