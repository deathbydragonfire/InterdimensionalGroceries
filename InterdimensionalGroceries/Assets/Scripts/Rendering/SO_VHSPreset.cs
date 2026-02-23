using UnityEngine;

namespace InterdimensionalGroceries.Rendering
{
    [CreateAssetMenu(fileName = "VHSPreset", menuName = "InterdimensionalGroceries/Rendering/VHS Preset", order = 1)]
    public class SO_VHSPreset : ScriptableObject
    {
        [Header("Preset Information")]
        public string presetName = "Custom Preset";
        [TextArea(3, 5)]
        public string description = "Custom VHS effect configuration";
        
        [Header("Tape Warping")]
        [Range(0f, 0.1f)]
        public float warpIntensity = 0.02f;
        [Range(0f, 5f)]
        public float warpSpeed = 1f;
        
        [Header("Tracking Bars")]
        [Range(0f, 1f)]
        public float trackingBarIntensity = 0.3f;
        [Range(0f, 2f)]
        public float trackingBarSpeed = 0.5f;
        [Range(0f, 0.2f)]
        public float trackingBarThickness = 0.05f;
        
        [Header("Color Bleeding")]
        [Range(0f, 0.05f)]
        public float colorBleed = 0.015f;
        
        [Header("Film Grain/Noise")]
        [Range(0f, 1f)]
        public float noiseIntensity = 0.2f;
        [Range(0f, 5f)]
        public float noiseSpeed = 2f;
        
        [Header("Scanlines")]
        [Range(0f, 1f)]
        public float scanlineIntensity = 0.3f;
        [Range(100f, 1000f)]
        public float scanlineCount = 500f;
        [Range(0f, 0.5f)]
        public float scanlineFlicker = 0.1f;
        
        [Header("Vignette & Color")]
        [Range(0f, 1f)]
        public float vignetteStrength = 0.3f;
        [Range(0f, 1f)]
        public float colorShift = 0.2f;
        [Range(0f, 1f)]
        public float desaturation = 0.3f;
        
        public void ApplyToController(VHSEffectController controller)
        {
            if (controller == null)
            {
                Debug.LogWarning("VHSPreset: Cannot apply preset to null controller");
                return;
            }
            
            controller.GetType().GetField("warpIntensity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, warpIntensity);
            controller.GetType().GetField("warpSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, warpSpeed);
            controller.GetType().GetField("trackingBarIntensity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, trackingBarIntensity);
            controller.GetType().GetField("trackingBarSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, trackingBarSpeed);
            controller.GetType().GetField("trackingBarThickness", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, trackingBarThickness);
            controller.GetType().GetField("colorBleed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, colorBleed);
            controller.GetType().GetField("noiseIntensity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, noiseIntensity);
            controller.GetType().GetField("noiseSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, noiseSpeed);
            controller.GetType().GetField("scanlineIntensity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, scanlineIntensity);
            controller.GetType().GetField("scanlineCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, scanlineCount);
            controller.GetType().GetField("scanlineFlicker", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, scanlineFlicker);
            controller.GetType().GetField("vignetteStrength", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, vignetteStrength);
            controller.GetType().GetField("colorShift", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, colorShift);
            controller.GetType().GetField("desaturation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(controller, desaturation);
        }
    }
}
