using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using System;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace XCSJ.PluginXAR.Foundation.Cameras.States
{
    /// <summary>
    /// AR相机管理器属性设置
    /// </summary>
    [Name(Title, nameof(ARCameraManagerPropertySet))]
    [Owner(typeof(XARFoundationManager))]
    public class ARCameraManagerPropertySet : BasePropertySet<ARCameraManagerPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "AR相机管理器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ARCameraManagerPropertySet))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(XARFoundationHelper.Title, typeof(XARFoundationManager))]
        [StateComponentMenu(XARFoundationHelper.Title + "/" + Title, typeof(XARFoundationManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// AR相机管理器
        /// </summary>
        [Name("AR相机管理器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ARCameraManager _cameraManager;

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
            /// 请求朝向
            /// </summary>
            [Name("请求朝向")]
            requestedFacingDirection,

            /// <summary>
            /// 自动对焦
            /// </summary>
            [Name("自动对焦")]
            autoFocusRequested,
        }

        /// <summary>
        /// 请求朝向
        /// </summary>
        [Name("请求朝向")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.requestedFacingDirection)]
        [OnlyMemberElements]
        public ECameraFacingDirectionPropertyValue _requestedFacingDirection = new ECameraFacingDirectionPropertyValue();

        /// <summary>
        /// 相机朝向属性值
        /// </summary>
        [Serializable]
        [Name("相机朝向属性值")]
        public class ECameraFacingDirectionPropertyValue : EnumPropertyValue<ECameraFacingDirection> { }

        /// <summary>
        /// 相机朝向
        /// </summary>
        [Name("相机朝向")]
        public enum ECameraFacingDirection
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 世界
            /// </summary>
            [Name("世界")]
            World,

            /// <summary>
            /// 用户
            /// </summary>
            [Name("用户")]
            User,

            /// <summary>
            /// 切换
            /// </summary>
            [Name("切换")]
            Switch,
        }

        /// <summary>
        /// 自动对焦
        /// </summary>
        [Name("自动对焦")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.autoFocusRequested)]
        [OnlyMemberElements]
        public EBoolPropertyValue _autoFocusRequested = new EBoolPropertyValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_AR_FOUNDATION
            if (!_cameraManager) return;
            switch (_propertyName)
            {
                case EPropertyName.requestedFacingDirection:
                    {
                        switch (_requestedFacingDirection.GetValue())
                        {
                            case ECameraFacingDirection.World:
                                {
                                    _cameraManager.requestedFacingDirection = CameraFacingDirection.World;
                                    break;
                                }
                            case ECameraFacingDirection.User:
                                {
                                    _cameraManager.requestedFacingDirection = CameraFacingDirection.User;
                                    break;
                                }
                            case ECameraFacingDirection.Switch:
                                {
                                    if (_cameraManager.requestedFacingDirection == CameraFacingDirection.User)
                                    {
                                        _cameraManager.requestedFacingDirection = CameraFacingDirection.World;
                                    }
                                    else
                                    {
                                        _cameraManager.requestedFacingDirection = CameraFacingDirection.User;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case EPropertyName.autoFocusRequested:
                    {
                        _cameraManager.autoFocusRequested = CommonFun.BoolChange(_cameraManager.autoFocusRequested, _autoFocusRequested.GetValue());
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
            switch (_propertyName)
            {
                case EPropertyName.requestedFacingDirection:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _requestedFacingDirection.ToFriendlyString();
                    }
                case EPropertyName.autoFocusRequested:
                    {
                        return CommonFun.Name(_propertyName) + "=" + _autoFocusRequested.ToFriendlyString();
                    }
                default: return base.ToFriendlyString();
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
#if XDREAMER_AR_FOUNDATION
            if (!_cameraManager) return false;
            switch (_propertyName)
            {
                case EPropertyName.requestedFacingDirection: return _requestedFacingDirection.DataValidity();
                case EPropertyName.autoFocusRequested: return _autoFocusRequested.DataValidity();
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
