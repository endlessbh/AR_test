using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using UnityEngine;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.States.Interactors
{
    /// <summary>
    /// 可交互性事件:用于捕获XR中XRBaseInteractor（XR基础交互器）的XRInteractorEvent（XR交互器事件）事件
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Trigger)]
    [ComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
    [Name(Title, nameof(InteractorEvent))]
    [Tip("用于捕获XR中XRBaseInteractor（XR基础交互器）的XRInteractorEvent（XR交互器事件）事件", "It is used to capture the xrinteractorevent event of xrbaseinteractor in XR")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Owner(typeof(XXRInteractionToolkitManager))]
    public class InteractorEvent : Trigger<InteractorEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "交互器事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(XRITHelper.CategoryName, typeof(XXRInteractionToolkitManager))]
        [StateComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
        [Name(Title, nameof(InteractorEvent))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("用于捕获XR中XRBaseInteractor（XR基础交互器）的XRInteractorEvent（XR交互器事件）事件", "It is used to capture the xrinteractorevent event of xrbaseinteractor in XR")]
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
        /// 交互器事件类型
        /// </summary>
        [Name("交互器事件类型")]
        [EnumPopup]
        public EXRInteractorEventType _interactorEventType = EXRInteractorEventType.OnHoverEnter;

        /// <summary>
        /// 检查可交互组件
        /// </summary>
        [Name("检查可交互组件")]
        [EnumPopup]
        public ECheckInteractable _checkInteractable = ECheckInteractable.Any;

        /// <summary>
        /// 可交互组件
        /// </summary>
        [Name("可交互组件")]
        [ComponentPopup]
        [HideInSuperInspector(nameof(_checkInteractable), EValidityCheckType.Or | EValidityCheckType.Equal, ECheckInteractable.None, nameof(_checkInteractable), EValidityCheckType.Equal, ECheckInteractable.Any)]
        [ValidityCheck(EValidityCheckType.NotNull)]
#if XDREAMER_XR_INTERACTION_TOOLKIT
        public XRBaseInteractable _interactable;
#else
        public MonoBehaviour _interactable;
#endif

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData">状态数据对象</param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
#if XDREAMER_XR_INTERACTION_TOOLKIT
            AddListen();
#endif
        }

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData">状态数据对象</param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
#if XDREAMER_XR_INTERACTION_TOOLKIT
            RemoveListen();
#endif
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

        private XRBaseInteractor listenedInteractor = null;
        private EXRInteractorEventType listenedInteractorEventType = EXRInteractorEventType.None;

        private void UpdateListen()
        {
            if (parent.isActive)
            {
                RemoveListen();
                AddListen();
            }
        }

        private void AddListen()
        {
            if (!_interactor) return;
            listenedInteractor = _interactor;
            listenedInteractorEventType = _interactorEventType;
            switch (listenedInteractorEventType)
            {
                case EXRInteractorEventType.OnHoverEnter:
                    {
                        listenedInteractor.hoverEntered.AddListener(OnHoverEntered);
                        break;
                    }
                case EXRInteractorEventType.OnHoverExit:
                    {
                        listenedInteractor.hoverExited.AddListener(OnHoverExited);
                        break;
                    }
                case EXRInteractorEventType.OnSelectEnter:
                    {
                        listenedInteractor.selectEntered.AddListener(OnSelectEntered);
                        break;
                    }
                case EXRInteractorEventType.OnSelectExit:
                    {
                        listenedInteractor.selectExited.AddListener(OnSelectExited);
                        break;
                    }
            }
        }

        private void RemoveListen()
        {
            if (!listenedInteractor) return;
            switch (listenedInteractorEventType)
            {
                case EXRInteractorEventType.OnHoverEnter:
                    {
                        listenedInteractor.hoverEntered.RemoveListener(OnHoverEntered);
                        break;
                    }
                case EXRInteractorEventType.OnHoverExit:
                    {
                        listenedInteractor.hoverExited.RemoveListener(OnHoverExited);
                        break;
                    }
                case EXRInteractorEventType.OnSelectEnter:
                    {
                        listenedInteractor.selectEntered.RemoveListener(OnSelectEntered);
                        break;
                    }
                case EXRInteractorEventType.OnSelectExit:
                    {
                        listenedInteractor.selectExited.RemoveListener(OnSelectExited);
                        break;
                    }
            }
            listenedInteractor = null;
            listenedInteractorEventType = EXRInteractorEventType.None;
        }

        private void OnHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractorEvent(hoverEnterEventArgs.interactorObject, hoverEnterEventArgs.interactableObject);
#else
            OnXRInteractorEvent(hoverEnterEventArgs.interactor, hoverEnterEventArgs.interactable);
#endif
        }

        private void OnHoverExited(HoverExitEventArgs hoverExitEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractorEvent(hoverExitEventArgs.interactorObject, hoverExitEventArgs.interactableObject);
#else
            OnXRInteractorEvent(hoverExitEventArgs.interactor, hoverExitEventArgs.interactable);
#endif
        }

        private void OnSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractorEvent(selectEnterEventArgs.interactorObject, selectEnterEventArgs.interactableObject);
#else
            OnXRInteractorEvent(selectEnterEventArgs.interactor, selectEnterEventArgs.interactable);
#endif
        }

        private void OnSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractorEvent(selectExitEventArgs.interactorObject, selectExitEventArgs.interactableObject);
#else
            OnXRInteractorEvent(selectExitEventArgs.interactor, selectExitEventArgs.interactable);
#endif
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
        private void OnXRInteractorEvent(IXRInteractor interactor, IXRInteractable interactable)
        {
            if (interactor as XRBaseInteractor == this._interactor)
            {
                OnXRInteractorEvent(interactable as XRBaseInteractable);
            }
        }

#else
        private void OnXRInteractorEvent(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            if (interactor == this._interactor)
            {
                OnXRInteractorEvent(interactable);
            }
        }

#endif

        /// <summary>
        /// 当捕获到交互器事件时回调
        /// </summary>
        /// <param name="interactable">触发事件的可交互组件</param>
        private void OnXRInteractorEvent(XRBaseInteractable interactable)
        {
            switch (_checkInteractable)
            {
                case ECheckInteractable.Any:
                    {
                        finished = true;
                        break;
                    }
                case ECheckInteractable.Is:
                    {
                        if (!finished)
                        {
                            finished = interactable == this._interactable;
                        }
                        break;
                    }
                case ECheckInteractable.NotIs:
                    {
                        if (!finished)
                        {
                            finished = interactable != this._interactable;
                        }
                        break;
                    }
            }
        }

        private void OnValidate()
        {
            UpdateListen();
        }

#endif

            /// <summary>
            /// 输出友好字符串
            /// </summary>
            /// <returns></returns>
        public override string ToFriendlyString()
        {
            return (_interactor ? _interactor.name : "") + "." + CommonFun.Name(_interactorEventType);
        }

        /// <summary>
        ///  数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (base.DataValidity() && _interactor)
            {
                switch (_checkInteractable)
                {
                    case ECheckInteractable.Is:
                    case ECheckInteractable.NotIs:
                        {
                            return _interactable;
                        }
                    default: return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 交互器事件类型枚举
        /// </summary>
        [Name("交互器事件类型")]
        public enum EXRInteractorEventType
        {
            [Name("无")]
            None,

            [Name("悬停进入")]
            OnHoverEnter,

            [Name("悬停退出")]
            OnHoverExit,

            [Name("选择进入")]
            OnSelectEnter,

            [Name("选择退出")]
            OnSelectExit,
        }

        /// <summary>
        /// 检查可交互性枚举
        /// </summary>
        [Name("检查可交互性")]
        public enum ECheckInteractable
        {
            /// <summary>
            /// 无,不做任何处理
            /// </summary>
            [Name("无")]
            [Tip("不做任何处理", "No treatment")]
            None,

            /// <summary>
            /// 任意:任意可交互组件均可触发事件
            /// </summary>
            [Name("任意")]
            [Tip("任意可交互组件均可触发事件", "Any interactive component can trigger events")]
            Any,

            /// <summary>
            /// 是:检查可交互组件，并且是期望的可交互组件时可触发事件
            /// </summary>
            [Name("是")]
            [Tip("检查可交互组件，并且是期望的可交互组件时可触发事件", "An event can be triggered when an interactive component is checked and is the desired interactive component")]
            Is,

            /// <summary>
            /// 不是:检查可交互组件，并且不是期望的可交互组件时可触发事件
            /// </summary>
            [Name("不是")]
            [Tip("检查可交互组件，并且不是期望的可交互组件时可触发事件", "An event can be triggered when an interactive component is checked and is not the expected interactive component")]
            NotIs,
        }
    }
}
