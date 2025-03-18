#if XDREAMER_PROJECT_URP

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XCSJ.CommonUtils.PluginOutline
{
    public class OutlinePass : ScriptableRenderPass
    {
        public struct OutlineSettings
        {
            public Color color;
            public float size;
            public float softness;
            public bool drawOnTop;
            public bool downsample;
        }
        public class OutlineMaterials
        {
            public Material UnlitMaterial { get; set; }
            public Material BlurMaterial { get; set; }
            public Material DilateMaterial { get; set; }
            public Material OutlineMaterial { get; set; }
        }

        private readonly string m_ProfilerTag;
        private readonly ProfilingSampler m_ProfilingSampler;
        private readonly List<ShaderTagId> m_ShaderTagIdList;
        private readonly OutlineMaterials m_materials;
        private RenderStateBlock m_RenderStateZTestOff;
        private RenderStateBlock m_RenderStateZTestOn;
        private FilteringSettings m_FilteringSettings;
        private RenderTargetHandle objectsID;
        private RenderTargetHandle blurredID;
        private RenderTargetHandle screenCopyID;
        private RenderTargetHandle tempID;

        public OutlineSettings settings;
        public RenderTargetIdentifier cameraColorTarget;
        public RenderTargetIdentifier cameraDepth;
        public OutlinePass(LayerMask layerMask, uint renderingLayerMask, OutlineMaterials materials, string profilerTag)
        {
            m_ProfilerTag = profilerTag;
            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            m_materials = materials;

            m_FilteringSettings = new FilteringSettings(RenderQueueRange.all, layerMask, renderingLayerMask);

            m_ShaderTagIdList = new List<ShaderTagId>
        {
            new ShaderTagId("SRPDefaultUnlit"),
            new ShaderTagId("UniversalForward"),
            new ShaderTagId("LightweightForward")
        };

            m_RenderStateZTestOff = new RenderStateBlock(RenderStateMask.Nothing);
            m_RenderStateZTestOn = new RenderStateBlock(RenderStateMask.Depth)
            {
                depthState = new DepthState { writeEnabled = false, compareFunction = CompareFunction.LessEqual }
            };
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            objectsID.Init("_ObjectsTex");
            blurredID.Init("_BlurredTex");
            screenCopyID.Init("_ScreenCopyTex");
            tempID.Init("_Temp");
            cmd.SetGlobalColor("_OutlineColor", settings.color);

            var descriptor = cameraTextureDescriptor;
            descriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(objectsID.id, descriptor);
            if (settings.downsample)
            {
                descriptor.width /= 2;
                descriptor.height /= 2;
            }
            cmd.GetTemporaryRT(blurredID.id, descriptor, FilterMode.Bilinear);
            cmd.GetTemporaryRT(tempID.id, descriptor, FilterMode.Bilinear);
            cmd.GetTemporaryRT(screenCopyID.id, cameraTextureDescriptor);

            ConfigureTarget(objectsID.id, cameraDepth);
            ConfigureClear(ClearFlag.Color, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, SortingCriteria.CommonOpaque);
            drawingSettings.overrideMaterial = m_materials.UnlitMaterial;
            drawingSettings.overrideMaterialPassIndex = 0;
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            cmd.Clear();

            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
                var blurSize = settings.softness * settings.size;
                var dilateSize = settings.size - blurSize;
                var renderStateBlock = settings.drawOnTop ? m_RenderStateZTestOff : m_RenderStateZTestOn;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings, ref renderStateBlock);

                Blit(cmd, colorAttachment, blurredID.id);
                // horizontal dilate
                cmd.SetGlobalVector("_OutlineOffsets", new Vector4(dilateSize / desc.width, 0, 0, 0));
                Blit(cmd, blurredID.id, tempID.id, m_materials.DilateMaterial);
                // vertical dilate            
                cmd.SetGlobalVector("_OutlineOffsets", new Vector4(0, dilateSize / desc.height, 0, 0));
                Blit(cmd, tempID.id, blurredID.id, m_materials.DilateMaterial);
                // horizontal blur
                cmd.SetGlobalVector("_OutlineOffsets", new Vector4(blurSize / desc.width, 0, 0, 0));
                Blit(cmd, blurredID.id, tempID.id, m_materials.BlurMaterial);
                // vertical blur
                cmd.SetGlobalVector("_OutlineOffsets", new Vector4(0, blurSize / desc.height, 0, 0));
                Blit(cmd, tempID.id, blurredID.id, m_materials.BlurMaterial);
                Blit(cmd, cameraColorTarget, screenCopyID.id, m_materials.OutlineMaterial);
                Blit(cmd, screenCopyID.id, cameraColorTarget);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(objectsID.id);
            cmd.ReleaseTemporaryRT(blurredID.id);
            cmd.ReleaseTemporaryRT(tempID.id);
            cmd.ReleaseTemporaryRT(screenCopyID.id);
        }
    }
}

#endif