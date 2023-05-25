using UnityEngine;

public class YokaiBrain : MonoBehaviour {

    private YokaiObserver observer;
    private int floorIndex;

    private void Awake() {

        observer = FindObjectOfType<YokaiObserver>();
    }

    private void Update() {
        
        floorIndex = observer.PlayerFloorIndex();
    }
}