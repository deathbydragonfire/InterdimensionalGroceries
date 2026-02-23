using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Experimental.Rendering;

namespace InterdimensionalGroceries.Rendering
{
    public class VHSRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class VHSSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            public Material vhsMaterial = null;
        }
        
        public VHSSettings settings = new VHSSettings();
        private VHSRenderPass vhsPass;
        
        public override void Create()
        {
            vhsPass = new VHSRenderPass(settings);
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (settings.vhsMaterial == null)
            {
                return;
            }
            
            vhsPass.ConfigureInput(ScriptableRenderPassInput.Color);
            renderer.EnqueuePass(vhsPass);
        }
        
        protected override void Dispose(bool disposing)
        {
            vhsPass?.Dispose();
        }
        
        class VHSRenderPass : ScriptableRenderPass
        {
            private VHSSettings settings;
            private VHSEffectController vhsController;
            private Material materialToUse;
            
            private class PassData
            {
                internal TextureHandle source;
                internal TextureHandle destination;
                internal Material material;
            }
            
            public VHSRenderPass(VHSSettings settings)
            {
                this.settings = settings;
                this.renderPassEvent = settings.renderPassEvent;
                this.profilingSampler = new ProfilingSampler("VHS Effect");
            }
            
            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                if (settings.vhsMaterial == null)
                    return;
                
                var resourceData = frameData.Get<UniversalResourceData>();
                var cameraData = frameData.Get<UniversalCameraData>();
                
                if (vhsController == null)
                {
                    vhsController = cameraData.camera.GetComponent<VHSEffectController>();
                }
                
                materialToUse = settings.vhsMaterial;
                
                if (vhsController != null && vhsController.enabled)
                {
                    vhsController.UpdateMaterialPropertiesPublic();
                    if (vhsController.RuntimeMaterial != null)
                    {
                        materialToUse = vhsController.RuntimeMaterial;
                    }
                }
                
                if (materialToUse == null)
                    return;
                
                TextureHandle source = resourceData.activeColorTexture;
                TextureHandle destination = resourceData.activeColorTexture;
                
                RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
                desc.depthBufferBits = 0;
                desc.msaaSamples = 1;
                
                TextureHandle tempTexture = UniversalRenderer.CreateRenderGraphTexture(
                    renderGraph, 
                    desc, 
                    "_VHSTempTexture", 
                    false
                );
                
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("VHS Effect Blit", out var passData, profilingSampler))
                {
                    passData.source = source;
                    passData.destination = tempTexture;
                    passData.material = materialToUse;
                    
                    builder.UseTexture(source, AccessFlags.Read);
                    builder.SetRenderAttachment(tempTexture, 0, AccessFlags.Write);
                    
                    builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                    {
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                    });
                }
                
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("VHS Copy to Source", out var passData2, profilingSampler))
                {
                    passData2.source = tempTexture;
                    passData2.destination = destination;
                    passData2.material = null;
                    
                    builder.UseTexture(tempTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(destination, 0, AccessFlags.Write);
                    
                    builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                    {
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0, false);
                    });
                }
            }
            
            public void Dispose()
            {
            }
        }
    }
}
