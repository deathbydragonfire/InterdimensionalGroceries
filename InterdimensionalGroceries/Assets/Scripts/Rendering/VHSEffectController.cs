using UnityEngine;

namespace InterdimensionalGroceries.Rendering
{
    [RequireComponent(typeof(Camera))]
    public class VHSEffectController : MonoBehaviour
    {
        [Header("VHS Material")]
        [SerializeField] private Material vhsMaterial;
        
        [Header("Effect Control")]
        [SerializeField] private bool effectEnabled = true;
        
        [Header("Effect Toggles")]
        [SerializeField] private bool enableWarp = true;
        [SerializeField] private bool enableTrackingBars = true;
        [SerializeField] private bool enableColorBleed = true;
        [SerializeField] private bool enableNoise = true;
        [SerializeField] private bool enableScanlines = true;
        [SerializeField] private bool enableVignette = true;
        [SerializeField] private bool enableColorShift = true;
        [SerializeField] private bool enableDesaturation = true;
        
        [Header("Tape Warping")]
        [Range(0f, 0.1f)]
        [SerializeField] private float warpIntensity = 0.02f;
        [Range(0f, 5f)]
        [SerializeField] private float warpSpeed = 1f;
        
        [Header("Tracking Bars")]
        [Range(0f, 1f)]
        [SerializeField] private float trackingBarIntensity = 0.3f;
        [Range(0f, 2f)]
        [SerializeField] private float trackingBarSpeed = 0.5f;
        [Range(0f, 0.2f)]
        [SerializeField] private float trackingBarThickness = 0.05f;
        
        [Header("Color Bleeding")]
        [Range(0f, 0.05f)]
        [SerializeField] private float colorBleed = 0.015f;
        
        [Header("Film Grain/Noise")]
        [Range(0f, 1f)]
        [SerializeField] private float noiseIntensity = 0.2f;
        [Range(0f, 5f)]
        [SerializeField] private float noiseSpeed = 2f;
        
        [Header("Scanlines")]
        [Range(0f, 1f)]
        [SerializeField] private float scanlineIntensity = 0.3f;
        [Range(100f, 1000f)]
        [SerializeField] private float scanlineCount = 500f;
        [Range(0f, 0.5f)]
        [SerializeField] private float scanlineFlicker = 0.1f;
        
        [Header("Vignette & Color")]
        [Range(0f, 1f)]
        [SerializeField] private float vignetteStrength = 0.3f;
        [Range(0f, 1f)]
        [SerializeField] private float colorShift = 0.2f;
        [Range(0f, 1f)]
        [SerializeField] private float desaturation = 0.3f;
        
        [Header("Runtime Control")]
        [Range(0f, 2f)]
        [SerializeField] private float globalIntensity = 1f;
        
        private Texture2D noiseTexture;
        private Material runtimeMaterial;
        
        public Material RuntimeMaterial => runtimeMaterial;
        
        private void Awake()
        {
            if (vhsMaterial == null)
            {
                Debug.LogError("VHSEffectController: VHS Material is not assigned!");
                enabled = false;
                return;
            }
            
            runtimeMaterial = new Material(vhsMaterial);
            
            if (runtimeMaterial.GetTexture("_NoiseTex") == null)
            {
                noiseTexture = VHSNoiseTextureGenerator.GenerateNoiseTexture();
                runtimeMaterial.SetTexture("_NoiseTex", noiseTexture);
            }
        }
        
        private void Update()
        {
            UpdateMaterialProperties();
        }
        
        public void UpdateMaterialPropertiesPublic()
        {
            UpdateMaterialProperties();
        }
        
        private void UpdateMaterialProperties()
        {
            if (runtimeMaterial == null) return;
            
            float intensity = effectEnabled ? globalIntensity : 0f;
            
            runtimeMaterial.SetFloat("_WarpIntensity", (enableWarp ? warpIntensity : 0f) * intensity);
            runtimeMaterial.SetFloat("_WarpSpeed", warpSpeed);
            
            runtimeMaterial.SetFloat("_TrackingBarIntensity", (enableTrackingBars ? trackingBarIntensity : 0f) * intensity);
            runtimeMaterial.SetFloat("_TrackingBarSpeed", trackingBarSpeed);
            runtimeMaterial.SetFloat("_TrackingBarThickness", trackingBarThickness);
            
            runtimeMaterial.SetFloat("_ColorBleed", (enableColorBleed ? colorBleed : 0f) * intensity);
            runtimeMaterial.SetFloat("_NoiseIntensity", (enableNoise ? noiseIntensity : 0f) * intensity);
            runtimeMaterial.SetFloat("_NoiseSpeed", noiseSpeed);
            
            runtimeMaterial.SetFloat("_ScanlineIntensity", (enableScanlines ? scanlineIntensity : 0f) * intensity);
            runtimeMaterial.SetFloat("_ScanlineCount", scanlineCount);
            runtimeMaterial.SetFloat("_ScanlineFlicker", scanlineFlicker);
            
            runtimeMaterial.SetFloat("_VignetteStrength", (enableVignette ? vignetteStrength : 0f) * intensity);
            runtimeMaterial.SetFloat("_ColorShift", (enableColorShift ? colorShift : 0f) * intensity);
            runtimeMaterial.SetFloat("_Desaturation", (enableDesaturation ? desaturation : 0f) * intensity);
        }
        
        public void SetEnabled(bool enabled)
        {
            effectEnabled = enabled;
        }
        
        public bool IsEnabled()
        {
            return effectEnabled;
        }
        
        public void SetWarpEnabled(bool enabled) => enableWarp = enabled;
        public void SetTrackingBarsEnabled(bool enabled) => enableTrackingBars = enabled;
        public void SetColorBleedEnabled(bool enabled) => enableColorBleed = enabled;
        public void SetNoiseEnabled(bool enabled) => enableNoise = enabled;
        public void SetScanlinesEnabled(bool enabled) => enableScanlines = enabled;
        public void SetVignetteEnabled(bool enabled) => enableVignette = enabled;
        public void SetColorShiftEnabled(bool enabled) => enableColorShift = enabled;
        public void SetDesaturationEnabled(bool enabled) => enableDesaturation = enabled;
        
        public bool IsWarpEnabled() => enableWarp;
        public bool IsTrackingBarsEnabled() => enableTrackingBars;
        public bool IsColorBleedEnabled() => enableColorBleed;
        public bool IsNoiseEnabled() => enableNoise;
        public bool IsScanlinesEnabled() => enableScanlines;
        public bool IsVignetteEnabled() => enableVignette;
        public bool IsColorShiftEnabled() => enableColorShift;
        public bool IsDesaturationEnabled() => enableDesaturation;
        
        public void SetAllEffects(bool enabled)
        {
            enableWarp = enabled;
            enableTrackingBars = enabled;
            enableColorBleed = enabled;
            enableNoise = enabled;
            enableScanlines = enabled;
            enableVignette = enabled;
            enableColorShift = enabled;
            enableDesaturation = enabled;
        }
        
        public void SetGlobalIntensity(float intensity)
        {
            globalIntensity = Mathf.Clamp(intensity, 0f, 2f);
        }
        
        public void GlitchOut(float duration = 0.5f)
        {
            StartCoroutine(GlitchOutCoroutine(duration));
        }
        
        private System.Collections.IEnumerator GlitchOutCoroutine(float duration)
        {
            float originalIntensity = globalIntensity;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                globalIntensity = Mathf.Lerp(originalIntensity, 2f, Mathf.Sin(t * Mathf.PI * 4f));
                
                trackingBarIntensity = Random.Range(0.5f, 1f);
                warpIntensity = Random.Range(0.05f, 0.1f);
                
                yield return null;
            }
            
            globalIntensity = originalIntensity;
        }
        
        public void SetPreset(VHSPreset preset)
        {
            switch (preset)
            {
                case VHSPreset.Subtle:
                    warpIntensity = 0.01f;
                    trackingBarIntensity = 0.1f;
                    noiseIntensity = 0.1f;
                    scanlineIntensity = 0.15f;
                    vignetteStrength = 0.2f;
                    colorShift = 0.1f;
                    desaturation = 0.2f;
                    break;
                    
                case VHSPreset.Classic:
                    warpIntensity = 0.02f;
                    trackingBarIntensity = 0.3f;
                    noiseIntensity = 0.2f;
                    scanlineIntensity = 0.3f;
                    vignetteStrength = 0.3f;
                    colorShift = 0.2f;
                    desaturation = 0.3f;
                    break;
                    
                case VHSPreset.Heavy:
                    warpIntensity = 0.05f;
                    trackingBarIntensity = 0.5f;
                    noiseIntensity = 0.4f;
                    scanlineIntensity = 0.5f;
                    vignetteStrength = 0.5f;
                    colorShift = 0.4f;
                    desaturation = 0.5f;
                    break;
                    
                case VHSPreset.ExtremeGlitch:
                    warpIntensity = 0.1f;
                    trackingBarIntensity = 1f;
                    noiseIntensity = 0.6f;
                    scanlineIntensity = 0.8f;
                    vignetteStrength = 0.6f;
                    colorShift = 0.6f;
                    desaturation = 0.4f;
                    break;
            }
        }
        
        private void OnDestroy()
        {
            if (runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }
            
            if (noiseTexture != null)
            {
                Destroy(noiseTexture);
            }
        }
    }
    
    public enum VHSPreset
    {
        Subtle,
        Classic,
        Heavy,
        ExtremeGlitch
    }
}
