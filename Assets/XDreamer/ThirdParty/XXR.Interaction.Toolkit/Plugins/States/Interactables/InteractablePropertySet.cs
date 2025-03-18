using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using UnityEngine;
using XCSJ.Extension.Base.Dataflows.Base;
using System;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.States.Interactables
{
    /// <summary>
    /// 可交互组件属性设置:用于XR中XRBaseInteractable（XR基础可交互组件）的属性设置
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
    [Name(Title, nameof(InteractablePropertySet))]
    [Tip("用于XR中XRBaseInteractable（XR基础可交互组件）的属性设置", "Used to set the properties of xrbaseinteractive (XR basic interactive component) in XR")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Owner(typeof(XXRInteractionToolkitManager))]
    public class InteractablePropertySet : BasePropertySet<InteractablePropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "可交互组件属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(XRITHelper.CategoryName, typeof(XXRInteractionToolkitManager))]
        [StateComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
        [Name(Title, nameof(InteractablePropertySet))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("用于XR中XRBaseInteractable（XR基础可交互组件）的属性设置", "Used to set the properties of xrbaseinteractive (XR basic interactive component) in XR")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// XR可交互组件
        /// </summary>
        [Name("XR可交互组件")]
        [ComponentPopup(searchFlags = ESearchFlags.Default)]
        [ValidityCheck(EValidityCheckType.NotNull)]
#if XDREAMER_XR_INTERACTION_TOOLKIT
        public XRBaseInteractable _interactable;
#else
        public MonoBehaviour _interactable;
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
            /// 交互管理器
            /// </summary>
            [Name("交互管理器")]
            interactionManager,

            /// <summary>
            /// 交互图层遮罩
            /// </summary>
            [Name("交互图层遮罩")]
            interactionLayerMask,

            #region XRGrabInteractable 1000


            #endregion

            #region BaseTeleportationInteractable 2000

            /// <summary>
            /// 发送传送请求
            /// </summary>
            [Name("发送传送请求")]
            SendTeleportRequest = 2000 + 100,

            #endregion
        }

        /// <summary>
        /// XR交互管理器
        /// </summary>
        [Name("XR交互管理器")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.interactionManager)]
        [OnlyMemberElements]
        public XRInteractionManagerPropertyValue _interactionManager = new XRInteractionManagerPropertyValue();

        /// <summary>
        /// 交互图层遮罩
        /// </summary>
        [Name("交互图层遮罩")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.interactionLayerMask)]
        [OnlyMemberElements]
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
        public InteractionLayerMaskPropertyValue _interactionLayerMask = new InteractionLayerMaskPropertyValue();
#else
        public LayerMaskPropertyValue _interactionLayerMask = new LayerMaskPropertyValue();
#endif

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            if (!_interactable) return;
            switch (_propertyName)
            {
                case EPropertyName.interactionManager:
                    {
                        _interactable.interactionManager = _interactionManager.GetValue();
                        break;
                    }
                case EPropertyName.interactionLayerMask:
                    {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
                        _interactable.interactionLayers = _interactionLayerMask.GetValue();
#else
                        _interactable.interactionLayerMask = _interactionLayerMask.GetValue();
#endif
                        break;
                    }
                case EPropertyName.SendTeleportRequest:
                    {
                        if (_interactable is BaseTeleportationInteractable teleportationInteractable
                            && teleportationInteractable.teleportationProvider)
                        {
                            TeleportRequest teleportRequest = default(TeleportRequest);
                            teleportRequest.matchOrientation = teleportationInteractable.matchOrientation;
                            teleportRequest.requestTime = Time.time;
                            if (teleportationInteractable is TeleportationAnchor anchor)
                            {
                                var dstTransform = anchor.teleportAnchorTransform ? anchor.teleportAnchorTransform : anchor.transform;
                                teleportRequest.destinationPosition = dstTransform.position;
                                teleportRequest.destinationRotation = dstTransform.rotation;
                                teleportationInteractable.teleportationProvider.QueueTeleportRequest(teleportRequest);
                            }
                            else// if (teleportationInteractable is TeleportationArea area) { }
                            {
                                var dstTransform = teleportationInteractable.transform;
                                teleportRequest.destinationPosition = dstTransform.position;
                                teleportRequest.destinationRotation = dstTransform.rotation;
                                teleportationInteractable.teleportationProvider.QueueTeleportRequest(teleportRequest);
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
                case EPropertyName.interactionManager: return CommonFun.Name(_propertyName) + "=" + _interactionManager.ToFriendlyString();
                case EPropertyName.interactionLayerMask: return CommonFun.Name(_propertyName) + "=" + _interactionLayerMask.ToFriendlyString();
                case EPropertyName.SendTeleportRequest: return (_interactable ? _interactable.name : "") + "." + CommonFun.Name(_propertyName);
                default:return  CommonFun.Name(_propertyName);
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            if (!_interactable) return false;
            switch (_propertyName)
            {
                case EPropertyName.interactionManager: return _interactionManager.DataValidity();
                case EPropertyName.interactionLayerMask: return _interactionLayerMask.DataValidity();
                case EPropertyName.SendTeleportRequest: return _interactable is BaseTeleportationInteractable;
            }
            return true;
#else
            return false;
#endif
        }
    }
}
