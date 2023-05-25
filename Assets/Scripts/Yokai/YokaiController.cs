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

    private void Start() {

        Door[] doors = FindObjectsOfType<Door>();
        foreach (var door in doors) {

            if (door.GetFireEvent()) {

                door.OnDoorOpen += Door_OnDoorOpen;
            }
        }
    }

    private void Door_OnDoorOpen(object sender, Door.OnDoorOpenEventArgs e) {

        if (e.isOpen) {

            behaviour.SpawnAtPosition(e.jumpscareTransform);
            behaviour.RunTowardsPosition(e.jumpscareTransform.position);
        }
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
