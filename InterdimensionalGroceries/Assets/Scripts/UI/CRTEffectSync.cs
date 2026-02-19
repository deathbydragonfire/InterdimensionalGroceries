using UnityEngine;

namespace InterdimensionalGroceries.UI
{
    public class CRTEffectSync : MonoBehaviour
    {
        [Header("CRT Materials")]
        [SerializeField] private Material screenMaterial;
        [SerializeField] private Material overlayMaterial;
        
        [Header("Effect Control")]
        [SerializeField] private float timeMultiplier = 1f;
        [SerializeField] private float glitchFrequency = 0.1f;
        
        [Header("Scanline Settings")]
        [SerializeField] [Range(0f, 1f)] private float scanlineDarkness = 0.3f;
        
        private float syncTime;
        private float nextGlitchTime;
        
        private static readonly int TimeSyncProperty = Shader.PropertyToID("_TimeSync");
        private static readonly int GlitchIntensityProperty = Shader.PropertyToID("_GlitchIntensity");
        private static readonly int ScanlineDarknessProperty = Shader.PropertyToID("_ScanlineDarkness");
        
        private void Update()
        {
            syncTime += Time.deltaTime * timeMultiplier;
            
            UpdateMaterialTime(screenMaterial);
            UpdateMaterialTime(overlayMaterial);
            
            UpdateScanlineDarkness();
            
            HandleRandomGlitches();
        }
        
        private void UpdateMaterialTime(Material mat)
        {
            if (mat != null)
            {
                mat.SetFloat(TimeSyncProperty, syncTime);
            }
        }
        
        private void UpdateScanlineDarkness()
        {
            if (overlayMaterial != null)
            {
                overlayMaterial.SetFloat(ScanlineDarknessProperty, scanlineDarkness);
            }
        }
        
        private void HandleRandomGlitches()
        {
            if (Time.time >= nextGlitchTime)
            {
                nextGlitchTime = Time.time + Random.Range(2f, 8f);
                StartCoroutine(TriggerGlitch());
            }
        }
        
        private System.Collections.IEnumerator TriggerGlitch()
        {
            float originalIntensity = screenMaterial != null ? screenMaterial.GetFloat(GlitchIntensityProperty) : 0.08f;
            float glitchIntensity = originalIntensity * Random.Range(2f, 5f);
            
            SetGlitchIntensity(glitchIntensity);
            
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            
            SetGlitchIntensity(originalIntensity);
        }
        
        public void SetGlitchIntensity(float intensity)
        {
            if (screenMaterial != null)
                screenMaterial.SetFloat(GlitchIntensityProperty, intensity);
                
            if (overlayMaterial != null)
                overlayMaterial.SetFloat(GlitchIntensityProperty, intensity);
        }
        
        public void SetScanlineDarkness(float darkness)
        {
            scanlineDarkness = Mathf.Clamp01(darkness);
            UpdateScanlineDarkness();
        }
        
        public void SetEffectIntensity(float scanlineIntensity, float distortionAmount)
        {
            if (screenMaterial != null)
            {
                screenMaterial.SetFloat("_ScanlineIntensity", scanlineIntensity);
                screenMaterial.SetFloat("_DistortionAmount", distortionAmount);
            }
            
            if (overlayMaterial != null)
            {
                overlayMaterial.SetFloat("_ScanlineDarkness", scanlineIntensity);
                overlayMaterial.SetFloat("_DistortionAmount", distortionAmount);
            }
        }
    }
}
