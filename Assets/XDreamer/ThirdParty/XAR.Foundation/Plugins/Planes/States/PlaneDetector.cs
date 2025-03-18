using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginXAR.Foundation.Base.States;
using XCSJ.PluginXAR.Foundation.Planes.Tools;
using XCSJ.Extension.Base.Extensions;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Planes.States
{
    /// <summary>
    /// 平面检测器
    /// </summary>
    [Name(Title, nameof(PlaneDetector))]
    [XCSJ.Attributes.Icon(EIcon.WireFrame)]
    public class PlaneDetector
#if XDREAMER_AR_FOUNDATION
        : BaseDetector<PlaneDetector, BoundedPlane, ARPlane, PlaneData>
#else
        : BaseDetector<PlaneDetector, PlaneData>
#endif
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "平面检测器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(PlaneDetector))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(XARFoundationHelper.Title, typeof(XARFoundationManager))]
        [StateComponentMenu(XARFoundationHelper.Title + "/" + Title, typeof(XARFoundationManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// AR平面管理器
        /// </summary>
        [Name("AR平面管理器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ARPlaneManager _planeManager;

        /// <summary>
        /// AR平面管理器
        /// </summary>
        public ARPlaneManager planeManager => this.XGetRootComponentInGlobal(ref _planeManager);

        /// <summary>
        /// 可跟踪对象集
        /// </summary>
        public override TrackableCollection<ARPlane> trackables => planeManager ? planeManager.trackables : default;

        private void OnTrackedImagesChanged(ARPlanesChangedEventArgs eventArgs)
        {
            OnTrackablesChanged(eventArgs.added, eventArgs.updated, eventArgs.removed);
        }


        /// <summary>
        /// 判断数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && _planeManager;

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
        /// 进入时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

#if XDREAMER_AR_FOUNDATION

            var manager = this.planeManager;
            if (manager)
            {
                manager.planesChanged += OnTrackedImagesChanged;
            }
#endif
        }

        /// <summary>
        /// 退出时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

#if XDREAMER_AR_FOUNDATION
            var manager = this.planeManager;
            if (manager)
            {
                manager.planesChanged -= OnTrackedImagesChanged;
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
