using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.CommonUtils.PluginHighlightingSystem
{
    /// <summary>
    /// 轮廓线数据加载器
    /// </summary>
    [DisallowMultipleComponent]
    [Name("轮廓线数据加载器")]
    [ExecuteInEditMode]
    [RequireManager(typeof(ToolsManager))]
    public class OutlineDataLoader : InteractProvider
    {
        /// <summary>
        /// 轮廓线必需Shader
        /// </summary>
        [Name("轮廓线必需Shader")]
        [Tip("使用右键菜单Reset可重置为默认Shader", "Use the right-click menu reset to reset to the default shader")]
        [Readonly]
        public Shader[] shaders;

        public static event Action<OutlineDataLoader, bool> enableChanged;

        private static bool shadersLoaded = false;

#if XDREAMER_PROJECT_URP

#else
        [Name("为所有相机添加")]
        [Tip("为所有相机添加轮廓线渲染器", "Add a contour renderer for all cameras")]
        public bool addForAllCamera = true;

        [Name("包含非激活")]
        public bool includeInactive = true;

        [Name("添加相机列表")]
        [HideInSuperInspector(nameof(addForAllCamera), EValidityCheckType.True)]
        public List<Camera> addCameras = new List<Camera>();

        [Name("忽略相机列表")]
        public List<Camera> ingoreCameras = new List<Camera>();
#endif

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            LoadShaders();
        }

        public void LoadShaders()
        {
#if XDREAMER_PROJECT_URP
            shaders = new Shader[4];
            shaders[0] = Shader.Find("XDreamer/Outline/UnlitColor");
            shaders[1] = Shader.Find("XDreamer/Outline/SeparableBlur");
            shaders[2] = Shader.Find("XDreamer/Outline/Dilate");
            shaders[3] = Shader.Find("XDreamer/Outline");
#else
            shaders = new Shader[5];
            shaders[0] = Shader.Find("XDreamer/Highlighted/Blur");
            shaders[1] = Shader.Find("XDreamer/Highlighted/Composite");
            shaders[2] = Shader.Find("XDreamer/Highlighted/Cut");
            shaders[3] = Shader.Find("XDreamer/Highlighted/Opaque");
            shaders[4] = Shader.Find("XDreamer/Highlighted/Transparent");
            
#endif
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!shadersLoaded)
            {
                shadersLoaded = true;
                LoadShaders();
            }

            enableChanged?.Invoke(this, true);

#if XDREAMER_PROJECT_URP

#else
            outlineDataLoaderMap.Add(this);

            if (Application.isPlaying)
            {
                InitCamera();

                SetHighlightingRenderer(true);
            }
#endif
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            enableChanged?.Invoke(this, false);

#if XDREAMER_PROJECT_URP

#else
            if (Application.isPlaying)
            {
                if(!outlineDataLoaderMap.Exists(obj => obj && obj.enabled))
                {
                    SetHighlightingRenderer(false);
                }
            }
#endif
        }

#if XDREAMER_PROJECT_URP

#else
        private void InitCamera()
        {
            if (addForAllCamera)
            {
                CommonFun.GetComponentsInChildren<Camera>(includeInactive).Foreach(AddHighlightingRendererToCamera);
            }
            else
            {
                addCameras.ForEach(AddHighlightingRendererToCamera);
            }
        }

        private void SetHighlightingRenderer(bool enabled)
        {
            foreach (var h in highlightingRenderers)
            {
                if (h) h.enabled = enabled; 
            }
        }

        private static List<OutlineDataLoader> outlineDataLoaderMap = new List<OutlineDataLoader>();

        private static HashSet<HighlightingRenderer> highlightingRenderers { get; set; } = new HashSet<HighlightingRenderer>();

        private void AddHighlightingRendererToCamera(Camera camera)
        {
            if (!camera || ingoreCameras.Contains(camera) || camera.GetComponent<HighlightingRenderer>()) return;

            if (!includeInactive && !camera.gameObject.activeSelf) return;

            var hr = camera.gameObject.AddComponent<HighlightingRenderer>();
            if (hr)
            {
                // 默认加载锐利的轮廓线
                hr.LoadPreset("2pix");

                highlightingRenderers.Add(hr);
            }
        }

#endif
    }
}
