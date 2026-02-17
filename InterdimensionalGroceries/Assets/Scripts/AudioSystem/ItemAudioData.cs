using UnityEngine;

namespace InterdimensionalGroceries.AudioSystem
{
    [CreateAssetMenu(fileName = "SO_ItemAudioData", menuName = "Audio/Item Audio Data")]
    public class ItemAudioData : ScriptableObject
    {
        [Header("Custom Impact Sound")]
        [SerializeField] private AudioClipData customImpactSound;
        
        [Header("Fallback Surface")]
        [SerializeField] private SurfaceType defaultSurfaceType = SurfaceType.Default;

        public AudioClipData CustomImpactSound => customImpactSound;
        public SurfaceType DefaultSurfaceType => defaultSurfaceType;

        public bool HasCustomImpactSound => customImpactSound != null && customImpactSound.Clips != null && customImpactSound.Clips.Length > 0;
    }
}
