using UnityEngine;

namespace TutorialSystem
{
    public enum TutorialEventTrigger
    {
        SceneLoad,
        AudioClipFinished,
        LocomotionComplete,
        ItemPickedUp,
        ItemScanned,
        TerminalOpened,
        ShelfPlaced,
        OrientationComplete
    }

    [System.Serializable]
    public class TimedAudioClip
    {
        [Tooltip("The audio clip to play")]
        public AudioClip clip;
        
        [Tooltip("Time (in seconds) after the main audio starts when this clip should play")]
        [Min(0f)]
        public float playAtTime;
    }

    [CreateAssetMenu(fileName = "TutorialEvent", menuName = "Tutorial/Tutorial Event")]
    public class TutorialEvent : ScriptableObject
    {
        [Header("Audio")]
        public AudioClip audioClip;

        [Header("Additional Timed Audio")]
        [Tooltip("Additional audio clips that play at specific times during the main audio")]
        public TimedAudioClip[] timedAudioClips;

        [Header("Trigger Settings")]
        public TutorialEventTrigger triggerType = TutorialEventTrigger.SceneLoad;
        public float delayBeforeTrigger = 0.5f;

        [Header("UI Settings")]
        public bool showLocomotionUI;
        public bool hideLocomotionUI;

        [Header("GameObject Activation")]
        public string[] gameObjectNamesToActivate;
    }
}
