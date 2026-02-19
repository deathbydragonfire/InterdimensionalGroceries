using UnityEngine;
using TMPro;

namespace InterdimensionalGroceries.UI
{
    [RequireComponent(typeof(TextMeshPro))]
    public class TMProCRTEffect : MonoBehaviour
    {
        [Header("CRT Material")]
        [SerializeField] private Material crtMaterial;
        
        [Header("Sync Reference")]
        [SerializeField] private CRTEffectSync syncController;
        
        private TextMeshPro tmpText;
        private MeshRenderer meshRenderer;
        private MaterialPropertyBlock propertyBlock;
        
        private static readonly int TimeSyncProperty = Shader.PropertyToID("_TimeSync");
        private static readonly int ScanlineIntensityProperty = Shader.PropertyToID("_ScanlineIntensity");
        private static readonly int ScanlineCountProperty = Shader.PropertyToID("_ScanlineCount");
        private static readonly int PixelSizeProperty = Shader.PropertyToID("_PixelSize");
        private static readonly int GlitchIntensityProperty = Shader.PropertyToID("_GlitchIntensity");
        private static readonly int GlitchSpeedProperty = Shader.PropertyToID("_GlitchSpeed");
        private static readonly int DistortionAmountProperty = Shader.PropertyToID("_DistortionAmount");
        private static readonly int DistortionSpeedProperty = Shader.PropertyToID("_DistortionSpeed");
        
        private void Awake()
        {
            tmpText = GetComponent<TextMeshPro>();
            meshRenderer = GetComponent<MeshRenderer>();
            propertyBlock = new MaterialPropertyBlock();
            
            if (crtMaterial != null)
            {
                meshRenderer.sharedMaterial = crtMaterial;
            }
        }
        
        private void Update()
        {
            if (crtMaterial == null || meshRenderer == null) return;
            
            meshRenderer.GetPropertyBlock(propertyBlock);
            
            if (syncController != null)
            {
                propertyBlock.SetFloat(TimeSyncProperty, Time.time);
            }
            
            meshRenderer.SetPropertyBlock(propertyBlock);
        }
        
        public void SetCRTParameters(float scanlineIntensity, float pixelSize, float glitchIntensity, float distortionAmount)
        {
            if (meshRenderer == null) return;
            
            meshRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(ScanlineIntensityProperty, scanlineIntensity);
            propertyBlock.SetFloat(PixelSizeProperty, pixelSize);
            propertyBlock.SetFloat(GlitchIntensityProperty, glitchIntensity);
            propertyBlock.SetFloat(DistortionAmountProperty, distortionAmount);
            meshRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
