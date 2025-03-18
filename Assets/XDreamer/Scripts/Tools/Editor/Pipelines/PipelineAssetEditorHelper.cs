#if UNITY_2019_3_OR_NEWER

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.Languages;

#if XDREAMER_PROJECT_URP
using UnityEngine.Rendering.Universal;
#endif

#if XDREAMER_PROJECT_HDRP
using UnityEngine.Rendering.HighDefinition;
#endif

namespace XCSJ.EditorTools.Pipelines
{

    /// <summary>
    /// 渲染管线助手
    /// </summary>
#if (XDREAMER_PROJECT_URP || XDREAMER_PROJECT_HDRP)
    [LanguageFileOutput]
    public static class PipelineAssetEditorHelper
    {
        /// <summary>
        /// 当前渲染管线配置
        /// </summary>
        public static RenderPipelineAsset CurrentRenderPipelineAsset
        {
            get
            {
                return QualitySettings.renderPipeline ? QualitySettings.renderPipeline : GraphicsSettings.renderPipelineAsset;
            }
        }

        /// <summary>
        /// 获取激活渲染资产配置文件：来自工程质量配置对应的资产文件
        /// </summary>
        public static List<RenderPipelineAsset> ActiveAssets
        {
            get
            {
                var assetList = new List<RenderPipelineAsset>();

                if (GraphicsSettings.renderPipelineAsset)
                {
                    assetList.Add(GraphicsSettings.renderPipelineAsset);
                }

                var qualitySettingNames = QualitySettings.names;
                for (var index = 0; index < qualitySettingNames.Length; index++)
                {
                    var assset = QualitySettings.GetRenderPipelineAssetAt(index);
                    if (assset && !assetList.Contains(assset))
                    {
                        assetList.Add(assset);
                    }
                }
                return assetList;
            }
        }

        /// <summary>
        /// 是否为URP资产
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static bool IsURP(RenderPipelineAsset asset)
        {
            return asset && asset.GetType().Name.Equals("UniversalRenderPipelineAsset");
        }

        /// <summary>
        /// 是否为HDRP资产 
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>

        public static bool IsHDRP(RenderPipelineAsset asset)
        {
            return asset && asset.GetType().Name.Equals("HDRenderPipelineAsset");
        }

        /// <summary>
        /// 当前激活渲染配置数量
        /// </summary>
        public static int activeRenderersCount => ActiveAssets.Count;

#if XDREAMER_PROJECT_URP

#if UNITY_2021_2_OR_NEWER
        
        /// <summary>
        /// 创建资产配置文件
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static RenderPipelineAsset CreateAsset(UniversalRendererData data)
        {
            return UniversalRenderPipelineAsset.Create(data);
        }

        /// <summary>
        /// 创建渲染数据文件
        /// </summary>
        /// <returns></returns>
        public static UniversalRendererData CreateRenderData()
        {
            return ScriptableObject.CreateInstance<UniversalRendererData>();
        }
#else
        public static RenderPipelineAsset CreateAsset(ForwardRendererData data)
        {
            return UniversalRenderPipelineAsset.Create(data);
        }

        public static ForwardRendererData CreateRenderData()
        {
            return ScriptableObject.CreateInstance<ForwardRendererData>();
        }
#endif

        /// <summary>
        /// 变量所有活跃渲染配置配置文件，添加指定的渲染特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [LanguageTuple("Add [{0}] Features to [{1}] asset profile", "添加[{0}]特性到[{1}]资产配置文件中")]
        public static bool AddFeatureIfNotContain<T>() where T : ScriptableRendererFeature
        {
            bool add = false;
            foreach (var asset in ActiveAssets)
            {
                if (!IsAssetContainsSRPFeature<T>(asset))
                {
                    if (asset is UniversalRenderPipelineAsset urpAsset && urpAsset)
                    {
                        var data = GetRenderer(asset);
                        if (data)
                        {
                            var so = ScriptableObject.CreateInstance<T>();
                            so.name = typeof(T).Name;
                            data.rendererFeatures.Add(so);
                            AssetDatabase.AddObjectToAsset(so, data);
                            Debug.LogFormat(LanguageHelper.Tr("Add [{0}] Features to [{1}] Asset profile", typeof(PipelineAssetEditorHelper)), so.name, AssetDatabase.GetAssetPath(asset));
                            add = true;
                        }
                    }
                }
            }
            
            return add;
        }

        /// <summary>
        /// 是否包含指定类型的SRP特征资产
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static bool IsAssetContainsSRPFeature<T>(RenderPipelineAsset asset) where T : ScriptableRendererFeature
        {
            if (asset is UniversalRenderPipelineAsset urpAsset && urpAsset)
            {
                var data = GetRenderer(asset);
                return data ? data.rendererFeatures.Exists(x => x is T) : false;
            }

            return false;
        }

        /// <summary>
        /// 获取渲染数据配置信息
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        private static ScriptableRendererData GetRenderer(RenderPipelineAsset asset)
        {
            using (var so = new SerializedObject(asset))
            {
                so.Update();

                var rendererDataList = so.FindProperty("m_RendererDataList");
                var assetIndex = so.FindProperty("m_DefaultRendererIndex");
                var item = rendererDataList.GetArrayElementAtIndex(assetIndex.intValue);

                return item.objectReferenceValue as ScriptableRendererData;
            }
        }
#endif

#if XDREAMER_PROJECT_HDRP
        public static RenderPipelineAsset CreateHDRPAsset()
        {
            return ScriptableObject.CreateInstance<HDRenderPipelineAsset>();
        }
#endif

    }
#endif
}

#endif