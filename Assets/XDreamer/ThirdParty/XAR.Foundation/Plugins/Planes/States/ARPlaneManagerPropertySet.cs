using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Planes.States
{
    /// <summary>
    /// AR平面管理器属性设置
    /// </summary>
    [Name(Title, nameof(ARPlaneManagerPropertySet))]
    [Owner(typeof(XARFoundationManager))]
    public class ARPlaneManagerPropertySet : BasePropertySet<ARPlaneManagerPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "AR平面管理器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ARPlaneManagerPropertySet))]
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
            /// 平面预制件
            /// </summary>
            [Name("平面预制件")]
            planePrefab,

            /// <summary>
            /// 请求的检测模式
            /// </summary>
            [Name("请求的检测模式")]
            requestedDetectionMode,
        }

        #region 平面预制件

        /// <summary>
        /// 平面预制件
        /// </summary>
        [Name("平面预制件")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.planePrefab)]
        [OnlyMemberElements]
        public GameObjectPropertyValue _planePrefab = new GameObjectPropertyValue();

        #endregion

        #region 请求的检测模式

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// 请求的检测模式
        /// </summary>
        [Name("请求的检测模式")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.requestedDetectionMode)]
        [OnlyMemberElements]
        public PlaneDetectionModePropertyValue _requestedDetectionMode = new PlaneDetectionModePropertyValue();

        /// <summary>
        /// 请求的检测模式属性值
        /// </summary>
        [Serializable]
        public class PlaneDetectionModePropertyValue : EnumPropertyValue<PlaneDetectionMode> { }

#endif

        #endregion

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_AR_FOUNDATION
            if (!_planeManager) return;
            switch (_propertyName)
            {
                case EPropertyName.planePrefab:
                    {
                        _planeManager.planePrefab = _planePrefab.GetValue();
                        break;
                    }
                case EPropertyName.requestedDetectionMode:
                    {
                        _planeManager.requestedDetectionMode = _requestedDetectionMode.GetValue();
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
                case EPropertyName.planePrefab:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _planePrefab.ToFriendlyString();
                    }
                case EPropertyName.requestedDetectionMode:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _requestedDetectionMode.ToFriendlyString();
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
            if (!_planeManager) return false;
            switch (_propertyName)
            {
                case EPropertyName.planePrefab: return _planePrefab.DataValidity();
                case EPropertyName.requestedDetectionMode: return _requestedDetectionMode.DataValidity();
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
