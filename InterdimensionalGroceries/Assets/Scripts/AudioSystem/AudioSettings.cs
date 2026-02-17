using UnityEngine;

namespace InterdimensionalGroceries.AudioSystem
{
    [CreateAssetMenu(fileName = "SO_AudioSettings", menuName = "Audio/Audio Settings")]
    public class AudioSettings : ScriptableObject
    {
        [Header("Master Volume")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

        [Header("Category Volumes")]
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float uiVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float ambientVolume = 1f;

        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 10;

        [Header("Default Pitch Variation")]
        [SerializeField] private float defaultMinPitch = 0.95f;
        [SerializeField] private float defaultMaxPitch = 1.05f;

        public float MasterVolume => masterVolume;
        public float SfxVolume => sfxVolume;
        public float UiVolume => uiVolume;
        public float AmbientVolume => ambientVolume;
        public int PoolSize => poolSize;
        public float DefaultMinPitch => defaultMinPitch;
        public float DefaultMaxPitch => defaultMaxPitch;
    }
}
