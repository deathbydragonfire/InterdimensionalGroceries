using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InterdimensionalGroceries.Rendering
{
    public class VHSTestingUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VHSEffectController vhsController;
        
        [Header("UI Elements - Chromatic Aberration")]
        [SerializeField] private Slider chromaticAberrationSlider;
        [SerializeField] private TextMeshProUGUI chromaticAberrationText;
        
        [Header("UI Elements - Warp")]
        [SerializeField] private Slider warpIntensitySlider;
        [SerializeField] private TextMeshProUGUI warpIntensityText;
        [SerializeField] private Slider warpSpeedSlider;
        [SerializeField] private TextMeshProUGUI warpSpeedText;
        
        [Header("UI Elements - Tracking")]
        [SerializeField] private Slider trackingIntensitySlider;
        [SerializeField] private TextMeshProUGUI trackingIntensityText;
        
        [Header("UI Elements - Noise")]
        [SerializeField] private Slider noiseIntensitySlider;
        [SerializeField] private TextMeshProUGUI noiseIntensityText;
        
        [Header("UI Elements - Scanlines")]
        [SerializeField] private Slider scanlineIntensitySlider;
        [SerializeField] private TextMeshProUGUI scanlineIntensityText;
        
        [Header("UI Elements - Global")]
        [SerializeField] private Slider globalIntensitySlider;
        [SerializeField] private TextMeshProUGUI globalIntensityText;
        
        [Header("Preset Buttons")]
        [SerializeField] private Button subtleButton;
        [SerializeField] private Button classicButton;
        [SerializeField] private Button heavyButton;
        [SerializeField] private Button glitchButton;
        [SerializeField] private Button triggerGlitchButton;
        
        private void Start()
        {
            if (vhsController == null)
            {
                vhsController = Camera.main?.GetComponent<VHSEffectController>();
            }
            
            if (vhsController == null)
            {
                Debug.LogError("VHSTestingUI: No VHSEffectController found!");
                enabled = false;
                return;
            }
            
            SetupSliders();
            SetupButtons();
        }
        
        private void SetupSliders()
        {
            if (chromaticAberrationSlider != null)
            {
                chromaticAberrationSlider.minValue = 0f;
                chromaticAberrationSlider.maxValue = 0.05f;
                chromaticAberrationSlider.onValueChanged.AddListener(OnChromaticAberrationChanged);
            }
            
            if (warpIntensitySlider != null)
            {
                warpIntensitySlider.minValue = 0f;
                warpIntensitySlider.maxValue = 0.1f;
                warpIntensitySlider.onValueChanged.AddListener(OnWarpIntensityChanged);
            }
            
            if (warpSpeedSlider != null)
            {
                warpSpeedSlider.minValue = 0f;
                warpSpeedSlider.maxValue = 5f;
                warpSpeedSlider.onValueChanged.AddListener(OnWarpSpeedChanged);
            }
            
            if (trackingIntensitySlider != null)
            {
                trackingIntensitySlider.minValue = 0f;
                trackingIntensitySlider.maxValue = 1f;
                trackingIntensitySlider.onValueChanged.AddListener(OnTrackingIntensityChanged);
            }
            
            if (noiseIntensitySlider != null)
            {
                noiseIntensitySlider.minValue = 0f;
                noiseIntensitySlider.maxValue = 1f;
                noiseIntensitySlider.onValueChanged.AddListener(OnNoiseIntensityChanged);
            }
            
            if (scanlineIntensitySlider != null)
            {
                scanlineIntensitySlider.minValue = 0f;
                scanlineIntensitySlider.maxValue = 1f;
                scanlineIntensitySlider.onValueChanged.AddListener(OnScanlineIntensityChanged);
            }
            
            if (globalIntensitySlider != null)
            {
                globalIntensitySlider.minValue = 0f;
                globalIntensitySlider.maxValue = 2f;
                globalIntensitySlider.value = 1f;
                globalIntensitySlider.onValueChanged.AddListener(OnGlobalIntensityChanged);
            }
        }
        
        private void SetupButtons()
        {
            if (subtleButton != null)
                subtleButton.onClick.AddListener(() => ApplyPreset(VHSPreset.Subtle));
            
            if (classicButton != null)
                classicButton.onClick.AddListener(() => ApplyPreset(VHSPreset.Classic));
            
            if (heavyButton != null)
                heavyButton.onClick.AddListener(() => ApplyPreset(VHSPreset.Heavy));
            
            if (glitchButton != null)
                glitchButton.onClick.AddListener(() => ApplyPreset(VHSPreset.ExtremeGlitch));
            
            if (triggerGlitchButton != null)
                triggerGlitchButton.onClick.AddListener(() => vhsController.GlitchOut(1f));
        }
        
        private void OnChromaticAberrationChanged(float value)
        {
            SetFieldValue("chromaticAberration", value);
            if (chromaticAberrationText != null)
                chromaticAberrationText.text = $"Chromatic Aberration: {value:F3}";
        }
        
        private void OnWarpIntensityChanged(float value)
        {
            SetFieldValue("warpIntensity", value);
            if (warpIntensityText != null)
                warpIntensityText.text = $"Warp Intensity: {value:F3}";
        }
        
        private void OnWarpSpeedChanged(float value)
        {
            SetFieldValue("warpSpeed", value);
            if (warpSpeedText != null)
                warpSpeedText.text = $"Warp Speed: {value:F2}";
        }
        
        private void OnTrackingIntensityChanged(float value)
        {
            SetFieldValue("trackingBarIntensity", value);
            if (trackingIntensityText != null)
                trackingIntensityText.text = $"Tracking Bars: {value:F2}";
        }
        
        private void OnNoiseIntensityChanged(float value)
        {
            SetFieldValue("noiseIntensity", value);
            if (noiseIntensityText != null)
                noiseIntensityText.text = $"Noise: {value:F2}";
        }
        
        private void OnScanlineIntensityChanged(float value)
        {
            SetFieldValue("scanlineIntensity", value);
            if (scanlineIntensityText != null)
                scanlineIntensityText.text = $"Scanlines: {value:F2}";
        }
        
        private void OnGlobalIntensityChanged(float value)
        {
            vhsController.SetGlobalIntensity(value);
            if (globalIntensityText != null)
                globalIntensityText.text = $"Global Intensity: {value:F2}";
        }
        
        private void ApplyPreset(VHSPreset preset)
        {
            vhsController.SetPreset(preset);
            Debug.Log($"VHS Testing: Applied {preset} preset");
        }
        
        private void SetFieldValue(string fieldName, float value)
        {
            var field = vhsController.GetType().GetField(fieldName, 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(vhsController, value);
        }
    }
}
