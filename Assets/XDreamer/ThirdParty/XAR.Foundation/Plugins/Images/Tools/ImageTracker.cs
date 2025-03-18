using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using XCSJ.Extension.Base.Extensions;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Images.Tools
{
    /// <summary>
    /// 图像跟踪器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Image)]
    [DisallowMultipleComponent]
    public class ImageTracker
#if XDREAMER_AR_FOUNDATION
        : BaseMainTracker<XRTrackedImage, ARTrackedImage, ImageData>
#else
        : BaseMainTracker<ImageData>
#endif
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "图像跟踪器";

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
        public ARTrackedImageManager trackedImageManager => this.XGetComponentInParentOrGlobal(ref _trackedImageManager);

        /// <summary>
        /// 可跟踪对象集
        /// </summary>
        public override TrackableCollection<ARTrackedImage> trackables => trackedImageManager ? trackedImageManager.trackables : default;

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            OnTrackablesChanged(eventArgs.added, eventArgs.updated, eventArgs.removed);
        }

#endif
        /// <summary>
        /// 跟踪数据
        /// </summary>
        public override ImageData trackData => _imageData;

        /// <summary>
        /// 图像数据
        /// </summary>
        [Name("图像数据")]
        [OnlyMemberElements]
        public ImageData _imageData = new ImageData();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
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
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
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
