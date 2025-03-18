using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXAR.Foundation.Images.Tools
{
    /// <summary>
    /// 图像跟踪器可视化工具
    /// </summary>
    [Name("图像跟踪器可视化工具")]
    [Tool(XARFoundationHelper.Title, nameof(ImageTracker))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.RawImage)]
    public class ImageTrackerVisualizer : BaseTrackerVisualizer
    {
        /// <summary>
        /// 图像跟踪器
        /// </summary>
        [Name("图像跟踪器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ImageTracker _imageTracker;

        /// <summary>
        /// 图像跟踪器
        /// </summary>
        public ImageTracker imageTracker => this.XGetComponentInParent(ref _imageTracker);

        /// <summary>
        /// 图像渲染器
        /// </summary>
        [Name("图像渲染器")]
        public Renderer _imageRenderer;

        /// <summary>
        /// 默认纹理
        /// </summary>
        [Name("默认纹理")]
        public Texture2D _defaultTexture;

        /// <summary>
        /// 缩放规则:图像渲染器对应变换的缩放规则
        /// </summary>
        [Name("缩放规则")]
        [Tip("图像渲染器对应变换的缩放规则", "Scaling rules for image renderer corresponding transformations")]
        [EnumPopup]
        public EScaleRule _scaleRule = EScaleRule.XY__LocalXZ;

        /// <summary>
        /// 图像变换
        /// </summary>
        [Name("图像变换")]
        public Transform _imageTransform;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            BaseTracker.onTrackerChanged += OnTrackerChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            BaseTracker.onTrackerChanged += OnTrackerChanged;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (!imageTracker) { }
        }

        private void OnTrackerChanged(BaseTracker tracker, ETrackEvent trackEvent)
        {
            if (tracker != this.imageTracker) return;
            switch (trackEvent)
            {
                case ETrackEvent.OnAdded:
                    {
                        UpdateInfo();
                        break;
                    }
                case ETrackEvent.OnUpdated:
                    {
                        UpdateInfo();
                        break;
                    }
            }
        }

        private void UpdateInfo()
        {
            var trackerData = imageTracker.trackData;

            if (_imageRenderer)
            {
                _imageRenderer.material.mainTexture = trackerData.texture ? trackerData.texture : _defaultTexture;
            }

            if(_imageTransform)
            {
                Scale(_scaleRule, _imageTransform, trackerData.imageSize);
            }
        }

        private void Scale(EScaleRule scaleRule, Transform imageTransform, Vector2 size)
        {
            switch (scaleRule)
            {
                case EScaleRule.XY__LocalXZ:
                    {
                        imageTransform.localScale = new Vector3(size.x, 1, size.y);
                        break;
                    }
                case EScaleRule.XY__LocalXY:
                    {
                        imageTransform.localScale = new Vector3(size.x, size.y, 1);
                        break;
                    }
                case EScaleRule.XY__LocalYZ:
                    {
                        imageTransform.localScale = new Vector3(1, size.x, size.y);
                        break;
                    }
            }
        }

        /// <summary>
        /// 缩放规则
        /// </summary>
        [Name("缩放规则")]
        public enum EScaleRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// XY到本地XZ
            /// </summary>
            [Name("XY到本地XZ")]
            XY__LocalXZ,

            /// <summary>
            /// XY到本地XY
            /// </summary>
            [Name("XY到本地XY")]
            XY__LocalXY,

            /// <summary>
            /// XY到本地YZ
            /// </summary>
            [Name("XY到本地YZ")]
            XY__LocalYZ,
        }
    }
}
