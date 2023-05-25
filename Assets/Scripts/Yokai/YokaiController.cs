using UnityEngine;

public class YokaiController : MonoBehaviour {

    [SerializeField] private float chooseActionInSeconds;
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;

    private YokaiBehaviour behaviour;
    private float chooseActionTimer;

    private void Awake() {

        behaviour = FindObjectOfType<YokaiBehaviour>();
    }

    private void Update() {

        if (ChooseNextAction()) {

            behaviour.RandomBehaviour();
        }
    }

    private bool ChooseNextAction() {

        chooseActionTimer += Time.deltaTime;

        return chooseActionTimer >= chooseActionInSeconds;
    }
}
