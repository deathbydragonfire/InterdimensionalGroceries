using UnityEngine;
using UnityEngine.Audio;
using InterdimensionalGroceries.UI;

namespace InterdimensionalGroceries.AudioSystem
{
    [CreateAssetMenu(fileName = "SO_AudioClipData", menuName = "Audio/Audio Clip Data")]
    public class AudioClipData : ScriptableObject
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] clips;

        [Header("Subtitles")]
        [Tooltip("Optional subtitles matching each audio clip. Array should match clips array length.")]
        [SerializeField] private SubtitleData[] subtitles;

        [Header("Volume Settings")]
        [SerializeField] private float minVolume = 0.8f;
        [SerializeField] private float maxVolume = 1f;

        [Header("Pitch Settings")]
        [SerializeField] private float minPitch = 0.95f;
        [SerializeField] private float maxPitch = 1.05f;

        [Header("Spatial Audio")]
        [SerializeField, Range(0f, 1f)] private float spatialBlend = 1f;
        [SerializeField] private float maxDistance = 20f;

        [Header("Mixer")]
        [SerializeField] private AudioMixerGroup mixerGroup;

        public AudioClip[] Clips => clips;
        public SubtitleData[] Subtitles => subtitles;
        public float SpatialBlend => spatialBlend;
        public float MaxDistance => maxDistance;
        public AudioMixerGroup MixerGroup => mixerGroup;

        public AudioClip GetRandomClip()
        {
            if (clips == null || clips.Length == 0)
                return null;

            return clips[Random.Range(0, clips.Length)];
        }

        public SubtitleData GetRandomSubtitle()
        {
            if (subtitles == null || subtitles.Length == 0)
                return null;

            return subtitles[Random.Range(0, subtitles.Length)];
        }

        public SubtitleData GetSubtitleForClip(int clipIndex)
        {
            if (subtitles == null || subtitles.Length == 0)
                return null;

            if (clipIndex < 0 || clipIndex >= subtitles.Length)
                return null;

            return subtitles[clipIndex];
        }

        public float GetRandomVolume()
        {
            return Random.Range(minVolume, maxVolume);
        }

        public float GetRandomPitch()
        {
            return Random.Range(minPitch, maxPitch);
        }
    }
}
