using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using UnityEngine;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Scripts;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.States.UI
{
    /// <summary>
    /// 跟踪设备图形射线检测器属性设置:用于XR中TrackedDeviceGraphicRaycaster（跟踪设备图形射线检测器）的属性设置
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
    [Name(Title, nameof(TrackedDeviceGraphicRaycasterPropertySet))]
    [Tip("用于XR中TrackedDeviceGraphicRaycaster（跟踪设备图形射线检测器）的属性设置", "Used to set the properties of TrackedDeviceGraphicRaycaster in XR")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Owner(typeof(XXRInteractionToolkitManager))]
    public class TrackedDeviceGraphicRaycasterPropertySet : BasePropertySet<TrackedDeviceGraphicRaycasterPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "跟踪设备图形射线检测器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(XRITHelper.CategoryName, typeof(XXRInteractionToolkitManager))]
        [StateComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
        [Name(Title, nameof(TrackedDeviceGraphicRaycasterPropertySet))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("用于XR中TrackedDeviceGraphicRaycaster（跟踪设备图形射线检测器）的属性设置", "Used to set the properties of TrackedDeviceGraphicRaycaster in XR")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 跟踪设备图形射线检测器
        /// </summary>
        [Name("跟踪设备图形射线检测器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
#if XDREAMER_XR_INTERACTION_TOOLKIT
        public TrackedDeviceGraphicRaycaster _raycaster;
#else
        public MonoBehaviour _raycaster;
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
            /// 忽略反转图形
            /// </summary>
            [Name("忽略反转图形")]
            ignoreReversedGraphics,

            /// <summary>
            /// 检查2D遮挡
            /// </summary>
            [Name("检查2D遮挡")]
            checkFor2DOcclusion,

            /// <summary>
            /// 检查3D遮挡
            /// </summary>
            [Name("检查3D遮挡")]
            checkFor3DOcclusion,

            /// <summary>
            /// 屏蔽遮罩
            /// </summary>
            [Name("屏蔽遮罩")]
            blockingMask,

            /// <summary>
            /// 射线检测触发器交互
            /// </summary>
            [Name("射线检测触发器交互")]
            raycastTriggerInteraction,

            /// <summary>
            /// 画布事件相机：设置射线检测器所在游戏对象上画布的事件相机为新相机；如果画布渲染是世界空间时，才能设置；
            /// </summary>
            [Name("画布事件相机")]
            [Tip("设置射线检测器所在游戏对象上画布的事件相机为新相机；如果画布渲染是世界空间时，才能设置；", "Set the event camera on the canvas of the game object where the ray detector is located as a new camera; It can only be set if the canvas rendering is world space;")]
            Canvas_EventCamera = 1000,

            /// <summary>
            /// 所有画布事件相机:设置所有射线检测器所在游戏对象上画布的事件相机为新相机；如果画布渲染是世界空间时，才能设置；
            /// </summary>
            [Name("所有画布事件相机")]
            [Tip("设置所有射线检测器所在游戏对象上画布的事件相机为新相机；如果画布渲染是世界空间时，才能设置；", "Set the event camera on the canvas of the game object where all ray detectors are located as a new camera; It can only be set if the canvas rendering is world space;")]
            AllCanvas_EventCamera,
        }

        /// <summary>
        /// 忽略反转图形
        /// </summary>
        [Name("忽略反转图形")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.ignoreReversedGraphics)]
        [OnlyMemberElements]
        public EBoolPropertyValue _ignoreReversedGraphics = new EBoolPropertyValue();

        /// <summary>
        /// 检查2D遮挡
        /// </summary>
        [Name("检查2D遮挡")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.checkFor2DOcclusion)]
        [OnlyMemberElements]
        public EBoolPropertyValue _checkFor2DOcclusion = new EBoolPropertyValue();

        /// <summary>
        /// 检查3D遮挡
        /// </summary>
        [Name("检查3D遮挡")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.checkFor3DOcclusion)]
        [OnlyMemberElements]
        public EBoolPropertyValue _checkFor3DOcclusion = new EBoolPropertyValue();

        /// <summary>
        /// 屏蔽遮罩
        /// </summary>
        [Name("屏蔽遮罩")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.blockingMask)]
        [OnlyMemberElements]
        public LayerMaskPropertyValue _blockingMask = new LayerMaskPropertyValue(-1);

        /// <summary>
        /// 射线检测触发器交互
        /// </summary>
        [Name("射线检测触发器交互")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.raycastTriggerInteraction)]
        [OnlyMemberElements]
        public QueryTriggerInteractionPropertyValue _raycastTriggerInteraction = new QueryTriggerInteractionPropertyValue();

        /// <summary>
        /// 画布事件相机：设置射线检测器所在游戏对象上画布的事件相机为新相机；如果画布渲染是世界空间时，才能设置；
        /// </summary>
        [Name("画布事件相机")]
        [Tip("设置射线检测器所在游戏对象上画布的事件相机为新相机；如果画布渲染是世界空间时，才能设置；", "Set the event camera on the canvas of the game object where the ray detector is located as a new camera; It can only be set if the canvas rendering is world space;")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.Canvas_EventCamera)]
        [OnlyMemberElements]
        public CameraPropertyValue _Canvas_EventCamera = new CameraPropertyValue();

        /// <summary>
        /// 所有画布事件相机:设置所有射线检测器所在游戏对象上画布的事件相机为新相机；如果画布渲染是世界空间时，才能设置；
        /// </summary>
        [Name("所有画布事件相机")]
        [Tip("设置所有射线检测器所在游戏对象上画布的事件相机为新相机；如果画布渲染是世界空间时，才能设置；", "Set the event camera on the canvas of the game object where all ray detectors are located as a new camera; It can only be set if the canvas rendering is world space;")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.AllCanvas_EventCamera)]
        [OnlyMemberElements]
        public CameraPropertyValue _AllCanvas_EventCamera = new CameraPropertyValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            switch (_propertyName)
            {
                case EPropertyName.ignoreReversedGraphics:
                    {
                        if (_raycaster)
                        {
                            _raycaster.ignoreReversedGraphics = _ignoreReversedGraphics.GetValue(_raycaster.ignoreReversedGraphics, EBool.No);
                        }
                        break;
                    }
                case EPropertyName.checkFor2DOcclusion:
                    {
                        if (_raycaster)
                        {
                            _raycaster.checkFor2DOcclusion = _checkFor2DOcclusion.GetValue(_raycaster.checkFor2DOcclusion, EBool.No);
                        }
                        break;
                    }
                case EPropertyName.checkFor3DOcclusion:
                    {
                        if (_raycaster)
                        {
                            _raycaster.checkFor3DOcclusion = _checkFor3DOcclusion.GetValue(_raycaster.checkFor3DOcclusion, EBool.No);
                        }
                        break;
                    }
                case EPropertyName.blockingMask:
                    {
                        if (_raycaster)
                        {
                            _raycaster.blockingMask = _blockingMask.GetValue(-1);
                        }
                        break;
                    }
                case EPropertyName.raycastTriggerInteraction:
                    {
                        if (_raycaster)
                        {
                            _raycaster.raycastTriggerInteraction = _raycastTriggerInteraction.GetValue(QueryTriggerInteraction.Ignore);
                        }
                        break;
                    }
                case EPropertyName.Canvas_EventCamera:
                    {
                        if (_raycaster && _Canvas_EventCamera.TryGetValue(out Camera camera) && camera && _raycaster.GetComponent<Canvas>() is Canvas canvas && canvas && canvas.renderMode == RenderMode.WorldSpace)
                        {
                            canvas.worldCamera = camera;
                        }
                        break;
                    }
                case EPropertyName.AllCanvas_EventCamera:
                    {
                        if (_AllCanvas_EventCamera.TryGetValue(out Camera camera) && camera)
                        {
                            var value = ComponentCache.Get<TrackedDeviceGraphicRaycaster>(true);
                            if (value != null)
                            {
                                foreach (var c in value.components)
                                {
                                    if (c && c.GetComponent<Canvas>() is Canvas canvas && canvas && canvas.renderMode == RenderMode.WorldSpace)
                                    {
                                        canvas.worldCamera = camera;
                                    }
                                }
                            }
                        }
                        break;
                    }
            }
#endif
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_propertyName)
            {
                case EPropertyName.ignoreReversedGraphics: return CommonFun.Name(_propertyName) + "=" + _ignoreReversedGraphics.ToFriendlyString();
                case EPropertyName.checkFor2DOcclusion: return CommonFun.Name(_propertyName) + "=" + _checkFor2DOcclusion.ToFriendlyString();
                case EPropertyName.checkFor3DOcclusion: return CommonFun.Name(_propertyName) + "=" + _checkFor3DOcclusion.ToFriendlyString();
                case EPropertyName.blockingMask: return CommonFun.Name(_propertyName) + "=" + _blockingMask.ToFriendlyString();
                case EPropertyName.raycastTriggerInteraction: return CommonFun.Name(_propertyName) + "=" + _raycastTriggerInteraction.ToFriendlyString();
                case EPropertyName.Canvas_EventCamera: return CommonFun.Name(_propertyName) + "=" + _Canvas_EventCamera.ToFriendlyString();
                case EPropertyName.AllCanvas_EventCamera: return CommonFun.Name(_propertyName) + "=" + _AllCanvas_EventCamera.ToFriendlyString();
            }
            return base.ToFriendlyString();
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            switch (_propertyName)
            {
                case EPropertyName.ignoreReversedGraphics: return _raycaster && _ignoreReversedGraphics.DataValidity();
                case EPropertyName.checkFor2DOcclusion: return _raycaster && _checkFor2DOcclusion.DataValidity();
                case EPropertyName.checkFor3DOcclusion: return _raycaster && _checkFor3DOcclusion.DataValidity();
                case EPropertyName.blockingMask: return _raycaster && _blockingMask.DataValidity();
                case EPropertyName.raycastTriggerInteraction: return _raycaster && _raycastTriggerInteraction.DataValidity();
                case EPropertyName.Canvas_EventCamera: return _raycaster && _Canvas_EventCamera.DataValidity();
                case EPropertyName.AllCanvas_EventCamera: return _AllCanvas_EventCamera.DataValidity();
            }
            return _raycaster;
        }
    }
}
