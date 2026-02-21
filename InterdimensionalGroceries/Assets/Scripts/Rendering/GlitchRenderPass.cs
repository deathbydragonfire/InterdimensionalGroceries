using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

namespace InterdimensionalGroceries.Rendering
{
    public class GlitchRenderPass : ScriptableRenderPass
    {
        private Material glitchMaterial;
        private static readonly int GlitchIntensityProperty = Shader.PropertyToID("_GlitchIntensity");
        
        private class PassData
        {
            internal Material material;
            internal TextureHandle source;
        }
        
        public GlitchRenderPass(Material material)
        {
            glitchMaterial = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }
        
        public void SetIntensity(float intensity)
        {
            if (glitchMaterial != null)
            {
                glitchMaterial.SetFloat(GlitchIntensityProperty, intensity);
            }
        }
        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (glitchMaterial == null)
                return;
            
            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();
            
            if (resourceData.isActiveTargetBackBuffer)
                return;
            
            var source = resourceData.activeColorTexture;
            var destinationDesc = cameraData.cameraTargetDescriptor;
            destinationDesc.depthBufferBits = 0;
            
            var destination = UniversalRenderer.CreateRenderGraphTexture(renderGraph, destinationDesc, "_GlitchTemp", false);
            
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Screen Glitch Effect", out var passData))
            {
                passData.material = glitchMaterial;
                passData.source = source;
                
                builder.UseTexture(source, AccessFlags.Read);
                builder.SetRenderAttachment(destination, 0, AccessFlags.Write);
                
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }
            
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Copy Back to Camera", out var passData))
            {
                passData.source = destination;
                
                builder.UseTexture(destination, AccessFlags.Read);
                builder.SetRenderAttachment(source, 0, AccessFlags.Write);
                
                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0, false);
                });
            }
        }
    }
}
