using UnityEngine;

namespace InterdimensionalGroceries.UI
{
    public enum SubtitleDisplayMode
    {
        Typewriter,
        SuperGlitchy,
        ShakingGlitchy
    }

    [CreateAssetMenu(fileName = "SO_Subtitle", menuName = "UI/Subtitle Data")]
    public class SubtitleData : ScriptableObject
    {
        [Header("Subtitle Content")]
        [TextArea(2, 4)]
        [SerializeField] private string subtitleText;

        [Header("Display Settings")]
        [SerializeField] private SubtitleDisplayMode displayMode = SubtitleDisplayMode.Typewriter;
        
        [Tooltip("Optional override for display duration. If 0, uses audio clip length.")]
        [SerializeField] private float displayDuration = 0f;

        [Header("Typewriter Effect")]
        [Tooltip("Delay between each character for typewriter effect")]
        [SerializeField] private float characterDelay = 0.05f;

        [Header("Glitch Effect")]
        [Tooltip("How intense the glitch effect should be (0-1)")]
        [SerializeField, Range(0f, 1f)] private float glitchIntensity = 0.7f;

        public string SubtitleText => subtitleText;
        public SubtitleDisplayMode DisplayMode => displayMode;
        public float DisplayDuration => displayDuration;
        public float CharacterDelay => characterDelay;
        public float GlitchIntensity => glitchIntensity;
    }
}
