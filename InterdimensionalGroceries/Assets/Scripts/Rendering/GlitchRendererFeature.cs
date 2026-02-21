using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace InterdimensionalGroceries.Rendering
{
    public class GlitchRendererFeature : ScriptableRendererFeature
    {
        [SerializeField] private Material glitchMaterial;
        
        private GlitchRenderPass glitchPass;
        
        public override void Create()
        {
            if (glitchMaterial != null)
            {
                glitchPass = new GlitchRenderPass(glitchMaterial);
            }
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (glitchPass != null && isActive)
            {
                renderer.EnqueuePass(glitchPass);
            }
        }
        
        public void SetIntensity(float intensity)
        {
            if (glitchPass != null)
            {
                glitchPass.SetIntensity(intensity);
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            glitchPass = null;
        }
    }
}
