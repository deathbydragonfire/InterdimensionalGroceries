using UnityEngine;

namespace InterdimensionalGroceries.Rendering
{
    public class VHSEffectManager : MonoBehaviour
    {
        public static VHSEffectManager Instance { get; private set; }
        
        [Header("References")]
        [SerializeField] private VHSEffectController mainCameraVHS;
        
        [Header("Event Glitch Settings")]
        [SerializeField] private float customerRejectionGlitchDuration = 0.5f;
        [SerializeField] private float phaseTransitionGlitchDuration = 0.75f;
        [SerializeField] private float genericEventGlitchDuration = 0.3f;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (mainCameraVHS == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                {
                    mainCameraVHS = mainCam.GetComponent<VHSEffectController>();
                    if (mainCameraVHS == null)
                    {
                        Debug.LogWarning("VHSEffectManager: Main camera does not have VHSEffectController component!");
                    }
                }
            }
        }
        
        public void TriggerCustomerRejectionGlitch()
        {
            if (mainCameraVHS != null && mainCameraVHS.IsEnabled())
            {
                mainCameraVHS.GlitchOut(customerRejectionGlitchDuration);
            }
        }
        
        public void TriggerPhaseTransitionGlitch()
        {
            if (mainCameraVHS != null && mainCameraVHS.IsEnabled())
            {
                mainCameraVHS.GlitchOut(phaseTransitionGlitchDuration);
            }
        }
        
        public void TriggerGenericGlitch()
        {
            if (mainCameraVHS != null && mainCameraVHS.IsEnabled())
            {
                mainCameraVHS.GlitchOut(genericEventGlitchDuration);
            }
        }
        
        public void TriggerCustomGlitch(float duration)
        {
            if (mainCameraVHS != null && mainCameraVHS.IsEnabled())
            {
                mainCameraVHS.GlitchOut(duration);
            }
        }
        
        public void SetEnabled(bool enabled)
        {
            if (mainCameraVHS != null)
            {
                mainCameraVHS.SetEnabled(enabled);
            }
        }
        
        public bool IsEnabled()
        {
            if (mainCameraVHS != null)
            {
                return mainCameraVHS.IsEnabled();
            }
            return false;
        }
        
        public void SetWarpEnabled(bool enabled)
        {
            if (mainCameraVHS != null) mainCameraVHS.SetWarpEnabled(enabled);
        }
        
        public void SetTrackingBarsEnabled(bool enabled)
        {
            if (mainCameraVHS != null) mainCameraVHS.SetTrackingBarsEnabled(enabled);
        }
        
        public void SetColorBleedEnabled(bool enabled)
        {
            if (mainCameraVHS != null) mainCameraVHS.SetColorBleedEnabled(enabled);
        }
        
        public void SetNoiseEnabled(bool enabled)
        {
            if (mainCameraVHS != null) mainCameraVHS.SetNoiseEnabled(enabled);
        }
        
        public void SetScanlinesEnabled(bool enabled)
        {
            if (mainCameraVHS != null) mainCameraVHS.SetScanlinesEnabled(enabled);
        }
        
        public void SetVignetteEnabled(bool enabled)
        {
            if (mainCameraVHS != null) mainCameraVHS.SetVignetteEnabled(enabled);
        }
        
        public void SetColorShiftEnabled(bool enabled)
        {
            if (mainCameraVHS != null) mainCameraVHS.SetColorShiftEnabled(enabled);
        }
        
        public void SetDesaturationEnabled(bool enabled)
        {
            if (mainCameraVHS != null) mainCameraVHS.SetDesaturationEnabled(enabled);
        }
        
        public void SetAllEffects(bool enabled)
        {
            if (mainCameraVHS != null) mainCameraVHS.SetAllEffects(enabled);
        }
        
        public void SetIntensity(float intensity)
        {
            if (mainCameraVHS != null)
            {
                mainCameraVHS.SetGlobalIntensity(intensity);
            }
        }
        
        public void ApplyPreset(VHSPreset preset)
        {
            if (mainCameraVHS != null)
            {
                mainCameraVHS.SetPreset(preset);
            }
        }
        
        public void FadeIntensity(float targetIntensity, float duration)
        {
            if (mainCameraVHS != null)
            {
                StartCoroutine(FadeIntensityCoroutine(targetIntensity, duration));
            }
        }
        
        private System.Collections.IEnumerator FadeIntensityCoroutine(float targetIntensity, float duration)
        {
            float startIntensity = 1f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, t);
                mainCameraVHS.SetGlobalIntensity(currentIntensity);
                yield return null;
            }
            
            mainCameraVHS.SetGlobalIntensity(targetIntensity);
        }
    }
}
