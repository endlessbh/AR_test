using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginHighlightingSystem;
using XCSJ.CommonUtils.PluginOutline;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.Languages;

#if XDREAMER_PROJECT_URP
using XCSJ.EditorTools.Pipelines;
#endif

namespace XCSJ.CommonUtils.EditorHighlightingSystem
{
    /// <summary>
    /// 轮廓线数据加载器检查器
    /// </summary>
    [Name("轮廓线数据加载器检查器")]
    [CustomEditor(typeof(OutlineDataLoader))]
    public class OutlineDataLoaderInspector : MBInspector<OutlineDataLoader>
    {
        /// <summary>
        /// 启用回调函数
        /// </summary>
        [InitializeOnLoadMethod]
        private static void AddEnableEventListener()
        {
            OutlineDataLoader.enableChanged -= OnSelectionHighlightingRendererEnableChanged;
            OutlineDataLoader.enableChanged += OnSelectionHighlightingRendererEnableChanged;
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Add [outline feature] to [render pipeline asset profile]", "添加【轮廓线特性】至【渲染管线资产配置文件】")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

#if XDREAMER_PROJECT_URP
            if (GUILayout.Button(Tr("Add [outline feature] to [render pipeline asset profile]")))
            {
                AddOutlineFeatureIfPipelineNotContain();
            }
#endif
        }

        private static void OnSelectionHighlightingRendererEnableChanged(OutlineDataLoader selectionHighlightingRenderer, bool enable)
        {
            if (enable)
            {
                AddOutlineFeatureIfPipelineNotContain();
            }
        }

        /// <summary>
        /// 检查轮廓线特性是否添加至渲染管线配置文件中
        /// </summary>
        [LanguageTuple("Create URP [render pipeline asset profile]", "创建URP【渲染管线资产配置文件】！")]
        private static void AddOutlineFeatureIfPipelineNotContain()
        {
#if XDREAMER_PROJECT_URP

            // 创建通用渲染管线资产
            if (PipelineAssetEditorHelper.activeRenderersCount == 0)
            {
                var rendererAsset = PipelineAssetEditorHelper.CreateRenderData();
                var asset = PipelineAssetEditorHelper.CreateAsset(rendererAsset);
                GraphicsSettings.renderPipelineAsset = asset;
                AssetDatabase.CreateAsset(rendererAsset, Product.Name + "_UniversalRenderPipelineAsset.asset");
                AssetDatabase.CreateAsset(asset, "");

                Debug.Log(LanguageHelper.Tr("Create URP [render pipeline asset profile]", typeof(OutlineDataLoaderInspector)));
            }

            // 给通用渲染管线资产添加特性
            if (PipelineAssetEditorHelper.AddFeatureIfNotContain<OutlineFeature>())
            {
                Debug.Log(LanguageHelper.Tr("Add [outline feature] to [render pipeline asset profile]", typeof(OutlineDataLoaderInspector)));
            }
#endif
        }
    }
}
