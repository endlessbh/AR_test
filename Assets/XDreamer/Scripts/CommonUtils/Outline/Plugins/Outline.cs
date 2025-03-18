using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Effects;
using System.Collections.Generic;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Collections;

namespace XCSJ.CommonUtils.PluginOutline
{
    /// <summary>
    /// 轮廓线
    /// </summary>
    [Name("轮廓线")]
    [RequireManager(typeof(ToolsManager))]
    public class Outline : InteractProvider, IOutline
    {
        [Name("使用自定义参数")]
        public bool _useCustom = false;

        [Name("颜色")]
        public Color _color = Color.green;

        [Name("宽度")]
        public float _width = 2;

        [Name("无遮挡")]
        public bool _overlay = true;

#if XDREAMER_PROJECT_URP

        private Color orgColor;

        private float orgWidth;

        private bool orgDrawOnTop;

        private static OutlineFeature outlineFeature
        {
            get
            {
                if (!_outlineFeature)
                {
                    _outlineFeature = Resources.FindObjectsOfTypeAll<OutlineFeature>().FirstOrDefault();

                    if (!outlineFeature)
                    {
                        Debug.LogError("轮廓线特色未找到!");
                    }
                }
                return _outlineFeature;
            }
        }
        private static OutlineFeature _outlineFeature = null;

        private Renderer[] renderers = null;

        private List<RendererRenderingLayerMaskRecorder> rendererRenderingLayerMaskRecorders = new List<RendererRenderingLayerMaskRecorder>();

        protected override void OnEnable()
        {
            base.OnEnable();

            SetOutline();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            RecoverOutline();
        }

        private void OnTransformChildrenChanged()
        {
            RecoverOutline();
            ResetData();
            SetOutline();
        }

        private void SetOutline()
        {
            // 设置特性颜色
            orgColor = outlineFeature.settings.color;
            orgWidth = outlineFeature.settings.size;
            orgDrawOnTop = outlineFeature.settings.drawOnTop;

            if (_useCustom || outliner == null)
            {
                outlineFeature.settings.color = _color;
                outlineFeature.settings.size = _width;
                outlineFeature.settings.drawOnTop = _overlay;
            }
            else
            {
                outlineFeature.settings.color = outliner.color;
                outlineFeature.settings.size = outliner.width;
                outlineFeature.settings.drawOnTop = outliner.overlay;
            }

            // 设置渲染层级
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }

            foreach (var r in renderers)
            {
                var recorder = new RendererRenderingLayerMaskRecorder();
                recorder.Record(r);
                rendererRenderingLayerMaskRecorders.Add(recorder);
                r.renderingLayerMask = outlineFeature.settings.renderingLayerMask;
            }
        }

        private void RecoverOutline()
        {
            // 还原渲染层级
            foreach (var r in rendererRenderingLayerMaskRecorders)
            {
                r.Recover();
            }

            // 还原特性颜色
            outlineFeature.settings.color = orgColor;
            outlineFeature.settings.size = orgWidth;
            outlineFeature.settings.drawOnTop = orgDrawOnTop;
        }

        private void ResetData()
        {
            renderers = null;
            rendererRenderingLayerMaskRecorders.Clear();
        }

#endif

        #region IOutline

        public bool canDisplay { get => enabled; set => enabled = value; }

        public bool isDisplay { get => enabled; set { } }

        private IOutlineData outliner;

        public void StartDisplay(IOutlineData outliner)
        {
            this.outliner = outliner;
            if (enabled)// 如果已经启用，则先禁用再启用
            {
                enabled = false;
            }
            enabled = true;
        }

        public void StopDisplay()
        {
            enabled = false;
        }

        #endregion
    }
}
