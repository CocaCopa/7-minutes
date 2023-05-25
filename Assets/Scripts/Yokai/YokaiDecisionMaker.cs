using UnityEngine;

public class YokaiDecisionMaker : MonoBehaviour {

    [SerializeField] private float chooseActionInSeconds;

    private YokaiObserver observer;

    private void Awake() {
        
        observer = FindObjectOfType<YokaiObserver>();
    }

    private void Update() {
        
        float floorIndex = observer.PlayerFloorIndex();
    }
}