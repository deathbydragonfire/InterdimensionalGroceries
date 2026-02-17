using UnityEngine;

namespace InterdimensionalGroceries.AudioSystem
{
    [CreateAssetMenu(fileName = "SO_SurfaceAudioData", menuName = "Audio/Surface Audio Data")]
    public class SurfaceAudioData : ScriptableObject
    {
        [System.Serializable]
        public class SurfaceAudioMapping
        {
            public SurfaceType surfaceType;
            public AudioClipData audioClipData;
        }

        [SerializeField] private SurfaceAudioMapping[] surfaceMappings;

        public AudioClipData GetAudioForSurface(SurfaceType surfaceType)
        {
            if (surfaceMappings == null) return null;

            foreach (var mapping in surfaceMappings)
            {
                if (mapping.surfaceType == surfaceType)
                {
                    return mapping.audioClipData;
                }
            }

            return null;
        }
    }
}
