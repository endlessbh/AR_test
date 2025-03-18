using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using XCSJ.Extension.Base.Extensions;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Planes.Tools
{
    /// <summary>
    /// 平面跟踪器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.WireFrame)]
    public class PlaneTracker
#if XDREAMER_AR_FOUNDATION
        : BaseMainTracker<BoundedPlane, ARPlane, PlaneData>
#else
        : BaseMainTracker<PlaneData>
#endif
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "平面跟踪器";

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// AR平面管理器
        /// </summary>
        [Name("AR平面管理器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ARPlaneManager _planeManager = null;

        /// <summary>
        /// AR平面管理器
        /// </summary>
        public ARPlaneManager planeManager => this.XGetComponentInParentOrGlobal(ref _planeManager);

        /// <summary>
        /// 可跟踪对象集
        /// </summary>
        public override TrackableCollection<ARPlane> trackables => planeManager ? planeManager.trackables : default;

        private void OnTrackedPlanesChanged(ARPlanesChangedEventArgs eventArgs)
        {
            OnTrackablesChanged(eventArgs.added, eventArgs.updated, eventArgs.removed);
        }

#endif

        /// <summary>
        /// 跟踪数据
        /// </summary>
        public override PlaneData trackData => _planeData;

        /// <summary>
        /// 平面数据
        /// </summary>
        [Name("平面数据")]
        [OnlyMemberElements]
        public PlaneData _planeData = new PlaneData();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
#if XDREAMER_AR_FOUNDATION

            var manager = this.planeManager;
            if (manager)
            {
                manager.planesChanged += OnTrackedPlanesChanged;
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

            var manager = this.planeManager;
            if (manager)
            {
                manager.planesChanged -= OnTrackedPlanesChanged;
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
            if (!planeManager) { }
#endif
        }
    }
}
