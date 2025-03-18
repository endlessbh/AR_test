using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginXAR.Foundation.Images.Tools;
using XCSJ.PluginXAR.Foundation.Base.States;
using XCSJ.Extension.Base.Extensions;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Images.States
{
    /// <summary>
    /// 设置AR跟踪图像管理器属性
    /// </summary>
    [Name(Title, nameof(ImageDetector))]
    [XCSJ.Attributes.Icon(EIcon.Image)]
    public class ImageDetector
#if XDREAMER_AR_FOUNDATION
        : BaseDetector<ImageDetector, XRTrackedImage, ARTrackedImage, ImageData>
#else
        : BaseDetector<ImageDetector, ImageData>
#endif
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "图像检测器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ImageDetector))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(XARFoundationHelper.Title, typeof(XARFoundationManager))]
        [StateComponentMenu(XARFoundationHelper.Title + "/" + Title, typeof(XARFoundationManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// AR跟踪图像管理器
        /// </summary>
        [Name("AR跟踪图像管理器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ARTrackedImageManager _trackedImageManager = null;

        /// <summary>
        /// AR跟踪图像管理器
        /// </summary>
        public ARTrackedImageManager trackedImageManager => this.XGetRootComponentInGlobal(ref _trackedImageManager);

        /// <summary>
        /// 可跟踪对象集
        /// </summary>
        public override TrackableCollection<ARTrackedImage> trackables => trackedImageManager ? trackedImageManager.trackables : default;

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            OnTrackablesChanged(eventArgs.added, eventArgs.updated, eventArgs.removed);
        }

        /// <summary>
        /// 判断数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && _trackedImageManager;

#endif

        /// <summary>
        /// 跟踪器数据
        /// </summary>
        public override ImageData trackData => _imageData;

        /// <summary>
        /// 图像数据
        /// </summary>
        [Name("图像数据")]
        [OnlyMemberElements]
        public ImageData _imageData = new ImageData();

        /// <summary>
        /// 进入时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

#if XDREAMER_AR_FOUNDATION

            var manager = this.trackedImageManager;
            if (manager)
            {
                manager.trackedImagesChanged += OnTrackedImagesChanged;
                trackData.AddToLibraryByLinkRule(manager);
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
            var manager = this.trackedImageManager;
            if (manager)
            {
                manager.trackedImagesChanged -= OnTrackedImagesChanged;
                trackData.RemoveFromLibiaryByLinkRule(manager);
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
            if (!trackedImageManager) { }
#endif
        }
    }
}
