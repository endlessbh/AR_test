#if XDREAMER_PROJECT_URP

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XCSJ.CommonUtils.PluginOutline
{
    /// <summary>
    /// 轮廓线特色
    /// </summary>
    public class OutlineFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class OutlineFeatureSettings
        {
            public string passTag = nameof(OutlineFeature);
            public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;
            public LayerMask layerMask = -1;
            public uint renderingLayerMask = 8;
            public Color color = Color.green;
            [Range(0, 10)]
            public float size = 2;
            [Range(0, 1)]
            public float softness = .5f;
            public bool drawOnTop = true;
            public bool downsample = false;
        }

        private OutlinePass.OutlineMaterials m_outlineMaterials = new OutlinePass.OutlineMaterials();
        private OutlinePass m_OutlinePass;
        public OutlineFeatureSettings settings = new OutlineFeatureSettings();

        public override void Create()
        {
            m_OutlinePass = new OutlinePass(settings.layerMask, settings.renderingLayerMask, m_outlineMaterials, name);
            m_OutlinePass.renderPassEvent = settings.Event;
        }
        public void OnEnable() => CreateMaterials(m_outlineMaterials);

        public void OnDisable() => DestroyMaterials(m_outlineMaterials);

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_OutlinePass.settings = new OutlinePass.OutlineSettings
            {
                color = settings.color,
                size = settings.size,
                softness = settings.softness,
                downsample = settings.downsample,
                drawOnTop = settings.drawOnTop,
            };

            RenderTargetIdentifier cameraTargetID = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
            //下面代码会报出警告：You can only call cameraDepthTarget inside the scope of a ScriptableRenderPass. Otherwise the pipeline camera target texture might have not been created or might have already been disposed. UnityEngine.Rendering.Universal.ScriptableRenderer:get_cameraDepthTarget ()，因此注释掉

            //m_OutlinePass.cameraDepth = renderer.cameraDepthTarget == cameraTargetID ? renderer.cameraColorTarget : renderer.cameraDepthTarget;
            m_OutlinePass.cameraDepth = renderer.cameraColorTarget;
            m_OutlinePass.cameraColorTarget = renderer.cameraColorTarget;

            renderer.EnqueuePass(m_OutlinePass);
        }
        private void CreateMaterials(OutlinePass.OutlineMaterials materials)
        {
            materials.UnlitMaterial = new Material(Shader.Find("XDreamer/Outline/UnlitColor"));
            materials.UnlitMaterial.hideFlags = HideFlags.HideAndDontSave;

            materials.BlurMaterial = new Material(Shader.Find("XDreamer/Outline/SeparableBlur"));
            materials.BlurMaterial.hideFlags = HideFlags.HideAndDontSave;

            materials.DilateMaterial = new Material(Shader.Find("XDreamer/Outline/Dilate"));
            materials.DilateMaterial.hideFlags = HideFlags.HideAndDontSave;

            materials.OutlineMaterial = new Material(Shader.Find("XDreamer/Outline"));
            materials.OutlineMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        private void DestroyMaterials(OutlinePass.OutlineMaterials materials)
        {
            DestroyImmediate(materials.UnlitMaterial);
            DestroyImmediate(materials.BlurMaterial);
            DestroyImmediate(materials.DilateMaterial);
            DestroyImmediate(materials.OutlineMaterial);
        }
    }
}

#endif