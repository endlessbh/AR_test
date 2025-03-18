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

namespace XCSJ.PluginXAR.Foundation.Faces.States
{
    /// <summary>
    /// AR面部管理器属性设置
    /// </summary>
    [Name(Title, nameof(ARFaceManagerPropertySet))]
    [Owner(typeof(XARFoundationManager))]
    public class ARFaceManagerPropertySet : BasePropertySet<ARFaceManagerPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "AR面部管理器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ARFaceManagerPropertySet))]
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
            /// 面部预制件
            /// </summary>
            [Name("面部预制件")]
            facePrefab,

            /// <summary>
            /// 请求的最大面部数
            /// </summary>
            [Name("请求的最大面部数")]
            requestedMaximumFaceCount,
        }

        /// <summary>
        /// 面部预制件
        /// </summary>
        [Name("面部预制件")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.facePrefab)]
        [OnlyMemberElements]
        public GameObjectPropertyValue _facePrefab = new GameObjectPropertyValue();

        /// <summary>
        /// 请求的最大面部数
        /// </summary>
        [Name("请求的最大面部数")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.requestedMaximumFaceCount)]
        [OnlyMemberElements]
        public IntPropertyValue _requestedMaximumFaceCount = new IntPropertyValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_AR_FOUNDATION
            if (!_faceManager) return;
            switch (_propertyName)
            {
                case EPropertyName.facePrefab:
                    {
                        _faceManager.facePrefab = _facePrefab.GetValue();
                        break;
                    }
                case EPropertyName.requestedMaximumFaceCount:
                    {
                        _faceManager.requestedMaximumFaceCount = _requestedMaximumFaceCount.GetValue(1);
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
                case EPropertyName.facePrefab:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _facePrefab.ToFriendlyString();
                    }
                case EPropertyName.requestedMaximumFaceCount:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _requestedMaximumFaceCount.ToFriendlyString();
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
            if (!_faceManager) return false;
            switch (_propertyName)
            {
                case EPropertyName.facePrefab: return _facePrefab.DataValidity();
                case EPropertyName.requestedMaximumFaceCount: return _requestedMaximumFaceCount.DataValidity();
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
