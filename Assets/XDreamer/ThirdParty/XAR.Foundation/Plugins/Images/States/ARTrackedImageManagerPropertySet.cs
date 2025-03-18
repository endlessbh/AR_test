using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
#endif
namespace XCSJ.PluginXAR.Foundation.Images.States
{
    /// <summary>
    /// AR跟踪图像管理器属性设置
    /// </summary>
    [Name(Title, nameof(ARTrackedImageManagerPropertySet))]
    [Owner(typeof(XARFoundationManager))]
    public class ARTrackedImageManagerPropertySet : BasePropertySet<ARTrackedImageManagerPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "AR跟踪图像管理器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ARTrackedImageManagerPropertySet))]
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

#endif

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.None;

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 跟踪图像预制件
            /// </summary>
            [Name("跟踪图像预制件")]
            trackedImagePrefab,

            /// <summary>
            /// 请求的最大运动图像数
            /// </summary>
            [Name("请求的最大运动图像数")]
            requestedMaxNumberOfMovingImages,
        }

        #region 跟踪图像预制件

        /// <summary>
        /// 跟踪图像预制件
        /// </summary>
        [Name("跟踪图像预制件")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.trackedImagePrefab)]
        [OnlyMemberElements]
        public GameObjectPropertyValue _trackedImagePrefab = new GameObjectPropertyValue();

        #endregion

        #region 请求的最大运动图像数

        /// <summary>
        /// 请求的最大运动图像数
        /// </summary>
        [Name("请求的最大运动图像数")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.requestedMaxNumberOfMovingImages)]
        [OnlyMemberElements]
        public IntPropertyValue _requestedMaxNumberOfMovingImages = new IntPropertyValue();

        #endregion

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_AR_FOUNDATION
            if (!_trackedImageManager) return;
            switch (_propertyName)
            {
                case EPropertyName.trackedImagePrefab:
                    {
                        _trackedImageManager.trackedImagePrefab = _trackedImagePrefab.GetValue();
                        break;
                    }
                case EPropertyName.requestedMaxNumberOfMovingImages:
                    {
                        _trackedImageManager.requestedMaxNumberOfMovingImages = _requestedMaxNumberOfMovingImages.GetValue(1);
                        break;
                    }
            }
#endif
        }

        private DirtyString dirtyString = new DirtyString();

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => dirtyString.GetData(_ToFriendlyString);

        private string _ToFriendlyString()
        {
#if XDREAMER_AR_FOUNDATION
            switch (_propertyName)
            {
                case EPropertyName.trackedImagePrefab:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _trackedImagePrefab.ToFriendlyString();
                    }
                case EPropertyName.requestedMaxNumberOfMovingImages:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _requestedMaxNumberOfMovingImages.ToFriendlyString();
                    }
                default: return CommonFun.Name(_propertyName);
            }
#else
            return XARFoundationHelper.Title + "功能未启用！";
#endif
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
#if XDREAMER_AR_FOUNDATION
            if (!_trackedImageManager) return false;
            switch (_propertyName)
            {
                case EPropertyName.trackedImagePrefab: return _trackedImagePrefab.DataValidity();
                case EPropertyName.requestedMaxNumberOfMovingImages: return _requestedMaxNumberOfMovingImages.DataValidity();
            }
#endif
            return false;
        }

        /// <summary>
        /// 验证
        /// </summary>
        public void OnValidate()
        {
            dirtyString.SetDirty();
        }
    }
}
