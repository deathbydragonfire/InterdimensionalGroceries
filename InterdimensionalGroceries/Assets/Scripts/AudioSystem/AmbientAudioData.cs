using UnityEngine;
using UnityEngine.Audio;

namespace InterdimensionalGroceries.AudioSystem
{
    [CreateAssetMenu(fileName = "SO_AmbientAudio", menuName = "Audio/Ambient Audio Data")]
    public class AmbientAudioData : ScriptableObject
    {
        [Header("Audio Clip")]
        [Tooltip("The ambient audio clip to play")]
        public AudioClip clip;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [Tooltip("Base volume for this ambient sound")]
        public float volume = 0.5f;

        [Header("Pitch Settings")]
        [Tooltip("Base pitch for this ambient sound")]
        [Range(0.1f, 3f)]
        public float pitch = 1f;

        [Tooltip("Random pitch variation range (±)")]
        [Range(0f, 0.5f)]
        public float pitchVariation = 0f;

        [Header("Spatial Audio")]
        [Tooltip("0 = 2D (no spatial), 1 = 3D (full spatial)")]
        [Range(0f, 1f)]
        public float spatialBlend = 0f;

        [Tooltip("Maximum distance for 3D sounds")]
        public float maxDistance = 50f;

        [Header("Playback Settings")]
        [Tooltip("Should this ambient sound loop?")]
        public bool loop = true;

        [Tooltip("Fade in duration when starting (seconds)")]
        public float fadeInDuration = 2f;

        [Tooltip("Fade out duration when stopping (seconds)")]
        public float fadeOutDuration = 2f;

        [Header("Audio Mixer")]
        [Tooltip("Optional audio mixer group")]
        public AudioMixerGroup mixerGroup;

        public float GetRandomizedPitch()
        {
            if (pitchVariation > 0f)
            {
                return pitch + Random.Range(-pitchVariation, pitchVariation);
            }
            return pitch;
        }
    }
}
