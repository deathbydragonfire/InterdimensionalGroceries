using UnityEngine;

namespace InterdimensionalGroceries.AudioSystem
{
    [CreateAssetMenu(fileName = "PhaseAudioCollection", menuName = "Audio/Phase Audio Clip Collection")]
    public class PhaseAudioClipCollection : ScriptableObject
    {
        [Header("Inventory Phase Audio")]
        [Tooltip("Audio clips to play during Inventory Phase")]
        public AudioClip[] inventoryPhaseClips;

        [Header("Delivery Phase Audio")]
        [Tooltip("Audio clips to play during Delivery Phase")]
        public AudioClip[] deliveryPhaseClips;

        [Header("Idle Audio")]
        [Tooltip("Audio clips to play when player is idle (standing still)")]
        public AudioClip[] idleClips;

        [Header("Playback Settings")]
        [Range(0f, 1f)]
        [Tooltip("Base volume for all clips")]
        public float volume = 1f;

        [Range(0.1f, 3f)]
        [Tooltip("Minimum pitch variation")]
        public float minPitch = 0.95f;

        [Range(0.1f, 3f)]
        [Tooltip("Maximum pitch variation")]
        public float maxPitch = 1.05f;

        [Header("Timing")]
        [Tooltip("Minimum delay between clips (seconds)")]
        public float minDelay = 5f;

        [Tooltip("Maximum delay between clips (seconds)")]
        public float maxDelay = 15f;

        [Header("Spatial Audio")]
        [Range(0f, 1f)]
        [Tooltip("0 = 2D, 1 = 3D spatial")]
        public float spatialBlend = 1f;

        [Tooltip("Distance at which audio starts to fade")]
        public float minDistance = 1f;

        [Tooltip("Distance at which audio is inaudible")]
        public float maxDistance = 50f;

        [Tooltip("How the audio fades with distance")]
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

        [Range(0f, 5f)]
        [Tooltip("Doppler effect strength")]
        public float dopplerLevel = 1f;

        public AudioClip GetRandomClip(PhaseManagement.GamePhase phase)
        {
            AudioClip[] clips = phase == PhaseManagement.GamePhase.InventoryPhase 
                ? inventoryPhaseClips 
                : deliveryPhaseClips;

            if (clips == null || clips.Length == 0)
                return null;

            return clips[Random.Range(0, clips.Length)];
        }

        public AudioClip GetRandomIdleClip()
        {
            if (idleClips == null || idleClips.Length == 0)
                return null;

            return idleClips[Random.Range(0, idleClips.Length)];
        }

        public float GetRandomPitch()
        {
            return Random.Range(minPitch, maxPitch);
        }

        public float GetRandomDelay()
        {
            return Random.Range(minDelay, maxDelay);
        }
    }
}
