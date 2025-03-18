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

namespace XCSJ.PluginXXR.Interaction.Toolkit.States.Interactors
{
    /// <summary>
    /// 交互器属性设置:用于XR中XRBaseInteractor（XR基础交互器）的属性设置
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
    [Name(Title, nameof(InteractorPropertySet))]
    [Tip("用于XR中XRBaseInteractor（XR基础交互器）的属性设置", "Used to set the properties of xrbaseinteractor in XR")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Owner(typeof(XXRInteractionToolkitManager))]
    public class InteractorPropertySet : BasePropertySet<InteractorPropertySet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "交互器属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(XRITHelper.CategoryName, typeof(XXRInteractionToolkitManager))]
        [StateComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
        [Name(Title, nameof(InteractorPropertySet))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("用于XR中XRBaseInteractor（XR基础交互器）的属性设置", "Used to set the properties of xrbaseinteractor in XR")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// XR交互器
        /// </summary>
        [Name("XR交互器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
#if XDREAMER_XR_INTERACTION_TOOLKIT
        public XRBaseInteractor _interactor;
#else
        public MonoBehaviour _interactor;
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
            if (!_interactor) return;
            switch (_propertyName)
            {
                case EPropertyName.interactionManager:
                    {
                        _interactor.interactionManager = _interactionManager.GetValue();
                        break;
                    }
                case EPropertyName.interactionLayerMask:
                    {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
                        _interactor.interactionLayers = _interactionLayerMask.GetValue();

#else
                        _interactor.interactionLayerMask = _interactionLayerMask.GetValue();

#endif
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
            }
            return base.ToFriendlyString();
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_interactor) return false;
            switch (_propertyName)
            {
                case EPropertyName.interactionManager: return _interactionManager.DataValidity();
                case EPropertyName.interactionLayerMask: return _interactionLayerMask.DataValidity();
            }
            return true;
        }
    }
}
