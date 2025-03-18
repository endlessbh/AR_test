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

namespace XCSJ.PluginXXR.Interaction.Toolkit.States.Interactables
{
    /// <summary>
    /// 可交互组件事件:用于捕获XR中XRBaseInteractable（XR基础可交互组件）的XRInteractableEvent（XR可交互组件事件）事件
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Trigger)]
    [ComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
    [Name(Title, nameof(InteractableEvent))]
    [Tip("用于捕获XR中XRBaseInteractable（XR基础可交互组件）的XRInteractableEvent（XR可交互组件事件）事件", "It is used to capture xrinteractableevent (XR interactable component event) events of xrbaseinteractable (XR basic interactable component) in XR")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Owner(typeof(XXRInteractionToolkitManager))]
    public class InteractableEvent : Trigger<InteractableEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "可交互组件事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(XRITHelper.CategoryName, typeof(XXRInteractionToolkitManager))]
        [StateComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
        [Name(Title, nameof(InteractableEvent))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("用于捕获XR中XRBaseInteractable（XR基础可交互组件）的XRInteractableEvent（XR可交互组件事件）事件", "It is used to capture xrinteractableevent (XR interactable component event) events of xrbaseinteractable (XR basic interactable component) in XR")]
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
        /// 可交互组件事件类型
        /// </summary>
        [Name("可交互组件事件类型")]
        [EnumPopup]
        public EInteractableEventType _interactableEventType = EInteractableEventType.OnFirstHoverEnter;

        /// <summary>
        /// 检查交互器
        /// </summary>
        [Name("检查交互器")]
        [EnumPopup]
        public ECheckInteractor _checkInteractor = ECheckInteractor.Any;

        /// <summary>
        /// XR交互器
        /// </summary>
        [Name("XR交互器")]
        [ComponentPopup]
        [HideInSuperInspector(nameof(_checkInteractor), EValidityCheckType.Or | EValidityCheckType.Equal, ECheckInteractor.None, nameof(_checkInteractor), EValidityCheckType.Equal, ECheckInteractor.Any)]
        [ValidityCheck(EValidityCheckType.NotNull)]
#if XDREAMER_XR_INTERACTION_TOOLKIT
        public XRBaseInteractor _interactor;
#else
        public MonoBehaviour _interactor;
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

        private XRBaseInteractable listenedInteractable = null;
        private EInteractableEventType listenedInteractableEventType = EInteractableEventType.None;

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
            if (!_interactable) return;
            listenedInteractable = _interactable;
            listenedInteractableEventType = _interactableEventType;
            switch (listenedInteractableEventType)
            {
                case EInteractableEventType.OnFirstHoverEnter:
                    {
                        listenedInteractable.firstHoverEntered.AddListener(OnFirstHoverEntered);
                        break;
                    }
                case EInteractableEventType.OnLastHoverExit:
                    {
                        listenedInteractable.lastHoverExited.AddListener(OnLastHoverExited);
                        break;
                    }
                case EInteractableEventType.OnHoverEnter:
                    {
                        listenedInteractable.hoverEntered.AddListener(OnHoverEntered);
                        break;
                    }
                case EInteractableEventType.OnHoverExit:
                    {
                        listenedInteractable.hoverExited.AddListener(OnHoverExited);
                        break;
                    }
                case EInteractableEventType.OnSelectEnter:
                    {
                        listenedInteractable.selectEntered.AddListener(OnSelectEntered);
                        break;
                    }
                case EInteractableEventType.OnSelectExit:
                    {
                        listenedInteractable.selectExited.AddListener(OnSelectExited);
                        break;
                    }
                case EInteractableEventType.OnActivate:
                    {
                        listenedInteractable.activated.AddListener(OnActivate);
                        break;
                    }
                case EInteractableEventType.OnDeactivate:
                    {
                        listenedInteractable.deactivated.AddListener(OnDeactivate);
                        break;
                    }
            }
        }

        private void RemoveListen()
        {
            if (!listenedInteractable) return;
            switch (listenedInteractableEventType)
            {
                case EInteractableEventType.OnFirstHoverEnter:
                    {
                        listenedInteractable.firstHoverEntered.RemoveListener(OnFirstHoverEntered);
                        break;
                    }
                case EInteractableEventType.OnLastHoverExit:
                    {
                        listenedInteractable.lastHoverExited.RemoveListener(OnLastHoverExited);
                        break;
                    }
                case EInteractableEventType.OnHoverEnter:
                    {
                        listenedInteractable.hoverEntered.RemoveListener(OnHoverEntered);
                        break;
                    }
                case EInteractableEventType.OnHoverExit:
                    {
                        listenedInteractable.hoverExited.RemoveListener(OnHoverExited);
                        break;
                    }
                case EInteractableEventType.OnSelectEnter:
                    {
                        listenedInteractable.selectEntered.RemoveListener(OnSelectEntered);
                        break;
                    }
                case EInteractableEventType.OnSelectExit:
                    {
                        listenedInteractable.selectExited.RemoveListener(OnSelectExited);
                        break;
                    }
                case EInteractableEventType.OnActivate:
                    {
                        listenedInteractable.activated.RemoveListener(OnActivate);
                        break;
                    }
                case EInteractableEventType.OnDeactivate:
                    {
                        listenedInteractable.deactivated.RemoveListener(OnDeactivate);
                        break;
                    }
            }
            listenedInteractable = null;
            listenedInteractableEventType = EInteractableEventType.None;
        }

        private void OnFirstHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractableEvent(hoverEnterEventArgs.interactorObject, hoverEnterEventArgs.interactableObject);
#else
            OnXRInteractableEvent(hoverEnterEventArgs.interactor, hoverEnterEventArgs.interactable);
#endif
        }

        private void OnLastHoverExited(HoverExitEventArgs hoverExitEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractableEvent(hoverExitEventArgs.interactorObject, hoverExitEventArgs.interactableObject);
#else
            OnXRInteractableEvent(hoverExitEventArgs.interactor, hoverExitEventArgs.interactable);
#endif
        }

        private void OnHoverEntered(HoverEnterEventArgs hoverEnterEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractableEvent(hoverEnterEventArgs.interactorObject, hoverEnterEventArgs.interactableObject);
#else
            OnXRInteractableEvent(hoverEnterEventArgs.interactor, hoverEnterEventArgs.interactable);
#endif
        }

        private void OnHoverExited(HoverExitEventArgs hoverExitEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractableEvent(hoverExitEventArgs.interactorObject, hoverExitEventArgs.interactableObject);
#else
            OnXRInteractableEvent(hoverExitEventArgs.interactor, hoverExitEventArgs.interactable);
#endif
        }

        private void OnSelectEntered(SelectEnterEventArgs selectEnterEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractableEvent(selectEnterEventArgs.interactorObject, selectEnterEventArgs.interactableObject);
#else
            OnXRInteractableEvent(selectEnterEventArgs.interactor, selectEnterEventArgs.interactable);
#endif
        }

        private void OnSelectExited(SelectExitEventArgs selectExitEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractableEvent(selectExitEventArgs.interactorObject, selectExitEventArgs.interactableObject);
#else
            OnXRInteractableEvent(selectExitEventArgs.interactor, selectExitEventArgs.interactable);
#endif
        }

        private void OnActivate(ActivateEventArgs activateEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractableEvent(activateEventArgs.interactorObject, activateEventArgs.interactableObject);
#else
            OnXRInteractableEvent(activateEventArgs.interactor, activateEventArgs.interactable);
#endif
        }

        private void OnDeactivate(DeactivateEventArgs deactivateEventArgs)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            OnXRInteractableEvent(deactivateEventArgs.interactorObject, deactivateEventArgs.interactableObject);
#else
            OnXRInteractableEvent(deactivateEventArgs.interactor, deactivateEventArgs.interactable);
#endif
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
        private void OnXRInteractableEvent(IXRInteractor interactor, IXRInteractable interactable)
        {
            if (interactable as XRBaseInteractable == this._interactable)
            {
                OnXRInteractableEvent(interactor as XRBaseInteractor);
            }
        }
#else
        private void OnXRInteractableEvent(XRBaseInteractor interactor, XRBaseInteractable interactable)
        {
            if (interactable == this._interactable)
            {
                OnXRInteractableEvent(interactor);
            }
        }

#endif

        /// <summary>
        /// 当捕获到可交互组件事件时回调
        /// </summary>
        /// <param name="interactor">触发事件的交互器</param>
        private void OnXRInteractableEvent(XRBaseInteractor interactor)
        {
            switch (_checkInteractor)
            {
                case ECheckInteractor.Any:
                    {
                        finished = true;
                        break;
                    }
                case ECheckInteractor.Is:
                    {
                        if (!finished)
                        {
                            finished = interactor == this._interactor;
                        }
                        break;
                    }
                case ECheckInteractor.NotIs:
                    {
                        if (!finished)
                        {
                            finished = interactor != this._interactor;
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
            return (_interactable ? _interactable.name : "") + "." + CommonFun.Name(_interactableEventType);
        }

        /// <summary>
        ///  数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (base.DataValidity() && _interactable)
            {
                switch (_checkInteractor)
                {
                    case ECheckInteractor.Is:
                    case ECheckInteractor.NotIs:
                        {
                            return _interactor;
                        }
                    default: return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 可交互组件事件类型枚举
        /// </summary>
        [Name("可交互组件事件类型")]
        public enum EInteractableEventType
        {
            [Name("无")]
            None,

            [Name("第一次悬停进入")]
            OnFirstHoverEnter,

            [Name("最后一次悬停退出")]
            OnLastHoverExit,

            [Name("悬停进入")]
            OnHoverEnter,

            [Name("悬停退出")]
            OnHoverExit,

            [Name("选择进入")]
            OnSelectEnter,

            [Name("选择退出")]
            OnSelectExit,

            [Name("激活")]
            OnActivate,

            [Name("停用")]
            OnDeactivate,
        }

        /// <summary>
        /// 检查交互器枚举
        /// </summary>
        [Name("检查交互器")]
        public enum ECheckInteractor
        {
            /// <summary>
            /// 无,不做任何处理
            /// </summary>
            [Name("无")]
            [Tip("不做任何处理", "No treatment")]
            None,

            /// <summary>
            /// 任意:任意交互器均可触发事件
            /// </summary>
            [Name("任意")]
            [Tip("任意交互器均可触发事件", "Any interactor can trigger an event")]
            Any,

            /// <summary>
            /// 是:检查交互器，并且是期望的交互器时可触发事件
            /// </summary>
            [Name("是")]
            [Tip("检查交互器，并且是期望的交互器时可触发事件", "An event can be triggered when the interactor is checked and is the desired interactor")]
            Is,

            /// <summary>
            /// 不是:检查交互器，并且不是期望的交互器时可触发事件
            /// </summary>
            [Name("不是")]
            [Tip("检查交互器，并且不是期望的交互器时可触发事件", "An event can be triggered when the interactor is checked and is not the expected interactor")]
            NotIs,
        }
    }
}
