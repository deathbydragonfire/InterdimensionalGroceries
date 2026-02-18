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

    [CreateAssetMenu(fileName = "TutorialEvent", menuName = "Tutorial/Tutorial Event")]
    public class TutorialEvent : ScriptableObject
    {
        [Header("Audio")]
        public AudioClip audioClip;

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
