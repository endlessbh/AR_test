using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using XCSJ.Extension.Base.Extensions;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Faces.Tools
{
    /// <summary>
    /// 面部跟踪器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Authentication)]
    public class FaceTracker
#if XDREAMER_AR_FOUNDATION
        : BaseMainTracker<XRFace, ARFace, FaceData>
#else
        : BaseMainTracker<FaceData>
#endif
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "面部跟踪器";

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// AR面部管理器
        /// </summary>
        [Name("AR面部管理器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ARFaceManager _faceManager = null;

        /// <summary>
        /// AR面部管理器
        /// </summary>
        public ARFaceManager faceManager => this.XGetComponentInParentOrGlobal(ref _faceManager);

        /// <summary>
        /// 可跟踪对象集
        /// </summary>
        public override TrackableCollection<ARFace> trackables => faceManager ? faceManager.trackables : default;

        private void OnTrackedFacesChanged(ARFacesChangedEventArgs eventArgs)
        {
            OnTrackablesChanged(eventArgs.added, eventArgs.updated, eventArgs.removed);
        }

#endif

        /// <summary>
        /// 跟踪数据
        /// </summary>
        public override FaceData trackData => _faceData;

        /// <summary>
        /// 面部数据
        /// </summary>
        [Name("面部数据")]
        [OnlyMemberElements]
        public FaceData _faceData = new FaceData();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
#if XDREAMER_AR_FOUNDATION

            var manager = this.faceManager;
            if (manager)
            {
                manager.facesChanged += OnTrackedFacesChanged;
            }
#endif
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
#if XDREAMER_AR_FOUNDATION

            var manager = this.faceManager;
            if (manager)
            {
                manager.facesChanged -= OnTrackedFacesChanged;
            }

            trackData.ClearTrackingState();
#endif
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
#if XDREAMER_AR_FOUNDATION
            if (!faceManager) { }
#endif
        }
    }
}
