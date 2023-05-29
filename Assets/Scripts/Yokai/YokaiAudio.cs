using UnityEngine;

public class YokaiAudio : MonoBehaviour {

    [Header("--- Door Event ---")]
    [SerializeField] private AudioClip doorJumpscare;

    [Header("--- Basement Event ---")]
    [SerializeField] private AudioClip getTheKeySFX;
    [SerializeField] private float delayTime;
    [SerializeField] private AudioClip laughterSFX;

    [Header("--- Run event ---")]
    [SerializeField] private AudioClip[] runWarning;
    [SerializeField] private AudioClip[] chaceSFX;
    [SerializeField, Range(0, 100)] private float chanceToTriggerWarning;

    [Header("--- Upstairs Hall JumpScare ---")]
    [SerializeField] private AudioClip footsteps;
    [SerializeField] private AudioClip jumpscareEffect;

    [Header("--- Kill Player ---")]
    [SerializeField] private AudioClip killScreamSFX;
    [SerializeField] private AudioClip hitEffectJumpScare;

    private AudioSource audioSource;
    private YokaiBehaviour yokaiBehaviour;

    private void Awake() {

        audioSource = GetComponent<AudioSource>();
        yokaiBehaviour = FindObjectOfType<YokaiBehaviour>();
    }

    private void Start() {

        YokaiObserver.Instance.OnDoorOpenJumpscare += Observer_OnDoorOpenJumpscare;
        YokaiObserver.Instance.OnBasementEventJumpscare += Observer_OnBasementEventJumpscare;
        YokaiObserver.Instance.OnBasementEventComplete += Observer_OnBasementEventComplete;
        YokaiObserver.Instance.OnRunEventChase += Observer_OnRunEventChase;
        YokaiObserver.Instance.OnRunEventWarning += Observer_OnRunEventWarning;
        YokaiObserver.Instance.OnUpstairsHallJumpscare += Observer_OnUpstairsHallJumpscare;
        yokaiBehaviour.OnKillPlayer += YokaiBehaviour_OnKillPlayer;
    }

    private void YokaiBehaviour_OnKillPlayer(object sender, System.EventArgs e) {

        audioSource.PlayOneShot(killScreamSFX);
        audioSource.PlayOneShot(hitEffectJumpScare);
    }

    private void Observer_OnUpstairsHallJumpscare(object sender, YokaiObserver.OnUpstairsHallJumpscareEventArgs e) {

        audioSource.PlayOneShot(footsteps);
    }

    private void Observer_OnRunEventWarning(object sender, System.EventArgs e) {

        int chance = Random.Range(0, 101);
        if (chance > 0 && chance <= chanceToTriggerWarning) {

            int randomWarningSFX = Random.Range(0, runWarning.Length);
            audioSource.PlayOneShot(runWarning[randomWarningSFX], 2.5f);
        }
    }

    private void Observer_OnRunEventChase(object sender, System.EventArgs e) {

        int randomChaseAudio = Random.Range(0, chaceSFX.Length);
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(chaceSFX[randomChaseAudio], 0.8f);
    }

    private void Observer_OnBasementEventComplete(object sender, System.EventArgs e) {

        audioSource.PlayOneShot(laughterSFX, 1.35f);
    }

    private void Observer_OnBasementEventJumpscare(object sender, System.EventArgs e) {

        Invoke(nameof(DelayAudioClipGetTheKey), delayTime);
    }

    private void DelayAudioClipGetTheKey() {

        audioSource.PlayOneShot(getTheKeySFX, 2f);
    }

    private void Observer_OnDoorOpenJumpscare(object sender, System.EventArgs e) {
        
        audioSource.PlayOneShot(doorJumpscare);
    }


}
