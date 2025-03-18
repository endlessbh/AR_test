using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXAR.Foundation.Images.Tools;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Faces.Tools
{
    /// <summary>
    /// 面部区域跟踪器
    /// </summary>
    [Name(Title)]
    [Tip("用于跟踪面部的局部区域", "Used to track local areas of the face")]
    [XCSJ.Attributes.Icon(EIcon.Authentication)]
    [Tool(XARFoundationHelper.Title, nameof(FaceTracker))]
    public class FaceRegionTracker
#if XDREAMER_AR_FOUNDATION
        : BaseSubTracker<FaceTracker, XRFace, ARFace, FaceData>
#else
        : BaseSubTracker<FaceTracker, FaceData>
#endif
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "面部区域跟踪器";

        /// <summary>
        /// 不支持时的处理规则
        /// </summary>
        [Name("不支持时的处理规则")]
        public enum ETrackRuleOnUnsupported
        {
            /// <summary>
            /// 无：即不做任何处理
            /// </summary>
            [Name("无")]
            [Tip("即不做任何处理", "That is, do not do any processing")]
            None,

            /// <summary>
            /// 错误日志：输出错误日志以提示用户当前功能不支持
            /// </summary>
            [Name("错误日志")]
            [Tip("输出错误日志以提示用户当前功能不支持", "Output the error log to remind the user that the current function is not supported")]
            ErrorLog,
        }

        /// <summary>
        /// 不支持时的处理规则:当对应主跟踪器中不支持当前期望功能时的处理规则
        /// </summary>
        [Name("不支持时的处理规则")]
        [Tip("当对应主跟踪器中不支持当前期望功能时的处理规则", "Processing rules when the current expected function is not supported in the corresponding main tracker")]
        [EnumPopup]
        public ETrackRuleOnUnsupported _trackRuleOnUnsupported = ETrackRuleOnUnsupported.ErrorLog;

        /// <summary>
        /// 面部跟踪器
        /// </summary>
        [Name("面部跟踪器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public FaceTracker _faceTracker = null;

        /// <summary>
        /// 主跟踪器
        /// </summary>
        public override FaceTracker mainTracker => this.XGetComponentInParent(ref _faceTracker);

        /// <summary>
        /// 跟踪数据
        /// </summary>
        public override FaceData trackData => mainTracker ? mainTracker.trackData : default;

        /// <summary>
        /// 面部区域
        /// </summary>
        [Name("面部区域")]
        public enum EFaceRegion
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 左眼
            /// </summary>
            [Name("左眼")]
            LeftEye,

            /// <summary>
            /// 右眼
            /// </summary>
            [Name("右眼")]
            RightEye,

            /// <summary>
            /// 注视点
            /// </summary>
            [Name("注视点")]
            FixationPoint,
        }

        /// <summary>
        /// 面部区域
        /// </summary>
        [Name("面部区域")]
        [EnumPopup]
        public EFaceRegion _faceRegion = EFaceRegion.FixationPoint;

        private EFaceRegion faceRegionCache = EFaceRegion.None;

        private void Link()
        {
#if XDREAMER_AR_FOUNDATION
            if (!mainTracker.faceManager.subsystem.subsystemDescriptor.supportsEyeTracking)//不支持
            {
                switch (_trackRuleOnUnsupported)
                {
                    case ETrackRuleOnUnsupported.ErrorLog:
                        {
                            Debug.LogErrorFormat("不支持游戏对象[{0}]上组件[{1}]的[{2}]功能！",
                                CommonFun.GameObjectToString(gameObject),
                                Title,
                                CommonFun.Name(_faceRegion));
                            break;
                        }
                    default: return;
                }
            }
            faceRegionCache = _faceRegion;
            switch (faceRegionCache)
            {
                case EFaceRegion.LeftEye: RecordTransform(trackable.leftEye); break;
                case EFaceRegion.RightEye: RecordTransform(trackable.rightEye); break;
                case EFaceRegion.FixationPoint: RecordTransform(trackable.fixationPoint); break;
            }
#endif
        }

        private void Unlink()
        {
#if XDREAMER_AR_FOUNDATION
            faceRegionCache = EFaceRegion.None;
            RecoverTransform();
#endif
        }

        private void OnTrackerChanged(BaseTracker tracker, ETrackEvent trackEvent)
        {
            if (tracker != mainTracker) return;
            switch (trackEvent)
            {
                case ETrackEvent.OnAdded:
                    {
                        Link();
                        break;
                    }
                case ETrackEvent.OnRemoved:
                    {
                        Unlink();
                        break;
                    }
                default:
                    {
                        if (faceRegionCache != _faceRegion && faceRegionCache == EFaceRegion.None)
                        {
                            Link();
                        }
                        break;
                    }
            }
            OnTrackEvent(trackEvent);
        }

        /// <summary>
        /// 当更新时
        /// </summary>
        protected override void OnUpdate()
        {
#if XDREAMER_AR_FOUNDATION
            //base.OnUpdate();
            switch (faceRegionCache)
            {
                case EFaceRegion.LeftEye: SyncTransform(trackable.leftEye); break;
                case EFaceRegion.RightEye: SyncTransform(trackable.rightEye); break;
                case EFaceRegion.FixationPoint: SyncTransform(trackable.fixationPoint); break;
            }
#endif
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            onTrackerChanged += OnTrackerChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            onTrackerChanged -= OnTrackerChanged;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (!mainTracker) { }
        }
    }
}
