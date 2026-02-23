using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using InterdimensionalGroceries.UI;

namespace InterdimensionalGroceries.Scenes
{
    public class ScreenGlitchController : MonoBehaviour
    {
        [SerializeField] private Material glitchMaterial;
        [SerializeField] private UniversalRendererData rendererData;
        [SerializeField] private GlitchLogoFlasher logoFlasher;
        
        private Rendering.GlitchRendererFeature glitchFeature;
        private static readonly int GlitchIntensityProperty = Shader.PropertyToID("_GlitchIntensity");
        
        private void Awake()
        {
            FindGlitchFeature();
        }
        
        private void FindGlitchFeature()
        {
            if (rendererData != null)
            {
                foreach (var feature in rendererData.rendererFeatures)
                {
                    if (feature is Rendering.GlitchRendererFeature glitch)
                    {
                        glitchFeature = glitch;
                        break;
                    }
                }
            }
        }
        
        public void PlaySmallGlitch(float duration)
        {
            StartCoroutine(PlayGlitchCoroutine(0.3f, duration, true));
        }
        
        public void PlayLargeGlitch(float duration)
        {
            StartCoroutine(PlayGlitchCoroutine(1.0f, duration, true));
        }
        
        private IEnumerator PlayGlitchCoroutine(float targetIntensity, float duration, bool shouldFlashLogo = true)
        {
            if (glitchFeature == null)
            {
                FindGlitchFeature();
            }
            
            if (glitchFeature != null)
            {
                glitchFeature.SetActive(true);
            }
            
            if (shouldFlashLogo && logoFlasher != null)
            {
                logoFlasher.TriggerLogoFlash(duration);
            }
            
            float halfDuration = duration * 0.5f;
            float elapsed = 0f;
            
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                float easedT = EaseInOut(t);
                float intensity = Mathf.Lerp(0f, targetIntensity, easedT);
                
                SetIntensity(intensity);
                yield return null;
            }
            
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                float easedT = EaseInOut(t);
                float intensity = Mathf.Lerp(targetIntensity, 0f, easedT);
                
                SetIntensity(intensity);
                yield return null;
            }
            
            SetIntensity(0f);
            
            if (glitchFeature != null)
            {
                glitchFeature.SetActive(false);
            }
        }
        
        private void SetIntensity(float intensity)
        {
            if (glitchMaterial != null)
            {
                glitchMaterial.SetFloat(GlitchIntensityProperty, intensity);
            }
            
            if (glitchFeature != null)
            {
                glitchFeature.SetIntensity(intensity);
            }
        }
        
        private float EaseInOut(float t)
        {
            return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
        }
    }
}
