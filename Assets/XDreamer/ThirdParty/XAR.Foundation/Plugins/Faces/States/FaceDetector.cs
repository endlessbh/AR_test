using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginXAR.Foundation.Base.States;
using XCSJ.PluginXAR.Foundation.Faces.Tools;
using XCSJ.Extension.Base.Extensions;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Faces.States
{
    /// <summary>
    /// 面部检测器
    /// </summary>
    [Name(Title, nameof(FaceDetector))]
    [XCSJ.Attributes.Icon(EIcon.Authentication)]
    public class FaceDetector
#if XDREAMER_AR_FOUNDATION
        : BaseDetector<FaceDetector, XRFace, ARFace, FaceData>
#else
        : BaseDetector<FaceDetector, FaceData>
#endif
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "面部检测器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(FaceDetector))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(XARFoundationHelper.Title, typeof(XARFoundationManager))]
        [StateComponentMenu(XARFoundationHelper.Title + "/" + Title, typeof(XARFoundationManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// AR面部管理器
        /// </summary>
        [Name("AR面部管理器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ARFaceManager _faceManager;

        /// <summary>
        /// AR面部管理器
        /// </summary>
        public ARFaceManager faceManager => this.XGetRootComponentInGlobal(ref _faceManager);

        /// <summary>
        /// 可跟踪对象集
        /// </summary>
        public override TrackableCollection<ARFace> trackables => faceManager ? faceManager.trackables : default;

        private void OnTrackedImagesChanged(ARFacesChangedEventArgs eventArgs)
        {
            OnTrackablesChanged(eventArgs.added, eventArgs.updated, eventArgs.removed);
        }

        /// <summary>
        /// 判断数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && _faceManager;

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
        /// 进入时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

#if XDREAMER_AR_FOUNDATION

            var manager = this.faceManager;
            if (manager)
            {
                manager.facesChanged += OnTrackedImagesChanged;
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

            var manager = this.faceManager;
            if (manager)
            {
                manager.facesChanged -= OnTrackedImagesChanged;
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
