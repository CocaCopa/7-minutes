using UnityEngine;

public class PlayerObserver : MonoBehaviour {

    [SerializeField] private Transform[] jumpscareDoorsTransform;
    [SerializeField] private Transform runTowards;

    private YokaiBehaviour behaviour;
    private PlayerMovement playerMovement;

    private void Awake() {

        behaviour = FindObjectOfType<YokaiBehaviour>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    private void Start() {

        foreach (var door in jumpscareDoorsTransform) {

            door.GetComponent<Door>().OnDoorOpen += PlayerObserver_OnDoorOpen;
        }
    }

    private void PlayerObserver_OnDoorOpen(object sender, Door.OnDoorOpenEventArgs e) {

        if (e.isOpen) {

            behaviour.SpawnAtPosition(e.jumpscareTransform);
            behaviour.RunTowardsPosition(e.jumpscareTransform.position);
            Invoke(nameof(StartRunning), 4f);
        }
    }

    private void StartRunning() {

        behaviour.RunTowardsPosition(runTowards.position);
    }
}
