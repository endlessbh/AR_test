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

namespace XCSJ.PluginXXR.Interaction.Toolkit.States.Locomotions
{
    /// <summary>
    /// 运动事件:用于捕获XR中LocomotionProvider的运动事件；如传送、快速旋转等事件；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Trigger)]
    [ComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
    [Name(Title, nameof(LocomotionEvent))]
    [Tip("用于捕获XR中LocomotionProvider的运动事件；如传送、快速旋转等事件；", "Used to capture motion events of locomotionprovider in XR; Such as transmission, fast rotation and other events;")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Owner(typeof(XXRInteractionToolkitManager))]
    public class LocomotionEvent : Trigger<LocomotionEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "运动事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(XRITHelper.CategoryName, typeof(XXRInteractionToolkitManager))]
        [StateComponentMenu(XRITHelper.CategoryName + "/" + Title, typeof(XXRInteractionToolkitManager))]
        [Name(Title, nameof(LocomotionEvent))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("用于捕获XR中LocomotionProvider的运动事件；如传送、快速旋转等事件；", "Used to capture motion events of locomotionprovider in XR; Such as transmission, fast rotation and other events;")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 运动提供者
        /// </summary>
        [Name("运动提供者")]
        [ComponentPopup(searchFlags = ESearchFlags.Default)]
        [ValidityCheck(EValidityCheckType.NotNull)]
#if XDREAMER_XR_INTERACTION_TOOLKIT
        public LocomotionProvider _locomotionProvider;
#else
        public MonoBehaviour _locomotionProvider;
#endif

        /// <summary>
        /// 运动事件类型
        /// </summary>
        [Name("运动事件类型")]
        [EnumPopup]
        public ELocomotionEventType _locomotionEventType = ELocomotionEventType.Begin;

        /// <summary>
        /// 检查运动系统
        /// </summary>
        [Name("检查运动系统")]
        [EnumPopup]
        public ECheckLocomotionSystem _checkLocomotionSystem = ECheckLocomotionSystem.Any;

        /// <summary>
        /// 运动系统
        /// </summary>
        [Name("运动系统")]
        [ComponentPopup]
        [HideInSuperInspector(nameof(_checkLocomotionSystem), EValidityCheckType.And | EValidityCheckType.NotEqual, ECheckLocomotionSystem.Is, nameof(_checkLocomotionSystem), EValidityCheckType.NotEqual, ECheckLocomotionSystem.NotIs)]
        [ValidityCheck(EValidityCheckType.NotNull)]
#if XDREAMER_XR_INTERACTION_TOOLKIT
        public LocomotionSystem _locomotionSystem;
#else
        public MonoBehaviour _locomotionSystem;
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

        private LocomotionProvider listenedLocomotionProvider = null;
        private ELocomotionEventType listenedLocomotionEventType = ELocomotionEventType.None;

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
            if (!_locomotionProvider) return;
            listenedLocomotionProvider = _locomotionProvider;
            listenedLocomotionEventType = _locomotionEventType;
            switch (_locomotionEventType)
            {
                case ELocomotionEventType.Begin:
                    {
                        listenedLocomotionProvider.beginLocomotion += OnLocomotion;
                        break;
                    }
                case ELocomotionEventType.End:
                    {
                        listenedLocomotionProvider.endLocomotion += OnLocomotion;
                        break;
                    }
            }
        }

        private void RemoveListen()
        {
            if (!listenedLocomotionProvider) return;
            switch (listenedLocomotionEventType)
            {
                case ELocomotionEventType.Begin:
                    {
                        listenedLocomotionProvider.beginLocomotion -= OnLocomotion;
                        break;
                    }
                case ELocomotionEventType.End:
                    {
                        listenedLocomotionProvider.endLocomotion -= OnLocomotion;
                        break;
                    }
            }
            listenedLocomotionProvider = null;
            listenedLocomotionEventType = ELocomotionEventType.None;
        }

        private void OnLocomotion(LocomotionSystem locomotionSystem)
        {
            switch (_checkLocomotionSystem)
            {
                case ECheckLocomotionSystem.Any:
                    {
                        finished = true;
                        break;
                    }
                case ECheckLocomotionSystem.Is:
                    {
                        if (!finished)
                        {
                            finished = locomotionSystem == this._locomotionSystem;
                        }
                        break;
                    }
                case ECheckLocomotionSystem.NotIs:
                    {
                        if (!finished)
                        {
                            finished = locomotionSystem != this._locomotionSystem;
                        }
                        break;
                    }
                case ECheckLocomotionSystem.IsDefined:
                    {
                        if (!finished)
                        {
                            finished = locomotionSystem == _locomotionProvider.system;
                        }
                        break;
                    }
                case ECheckLocomotionSystem.NotIsDefined:
                    {
                        if (!finished)
                        {
                            finished = locomotionSystem == _locomotionProvider.system;
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
            return (_locomotionProvider ? _locomotionProvider.name : "") + "." + CommonFun.Name(_locomotionEventType);
        }

        /// <summary>
        ///  数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (base.DataValidity() && _locomotionProvider)
            {
                switch (_checkLocomotionSystem)
                {
                    case ECheckLocomotionSystem.Is:
                    case ECheckLocomotionSystem.NotIs:
                        {
                            return _locomotionSystem;
                        }
                    default: return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 运动事件类型
        /// </summary>
        [Name("运动事件类型")]
        public enum ELocomotionEventType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 开始
            /// </summary>
            [Name("开始")]
            Begin,

            /// <summary>
            /// 结束
            /// </summary>
            [Name("结束")]
            End,
        }

        /// <summary>
        /// 检查运动系统
        /// </summary>
        [Name("检查运动系统")]
        public enum ECheckLocomotionSystem
        {
            /// <summary>
            /// 无,不做任何处理
            /// </summary>
            [Name("无")]
            [Tip("不做任何处理", "No treatment")]
            None,

            /// <summary>
            /// 任意:任意运动系统均可触发事件
            /// </summary>
            [Name("任意")]
            [Tip("任意运动系统均可触发事件", "Any motion system can trigger events")]
            Any,

            /// <summary>
            /// 是:检查运动系统，并且是期望的运动系统时可触发事件
            /// </summary>
            [Name("是")]
            [Tip("检查运动系统，并且是期望的运动系统时可触发事件", "An event can be triggered when the motion system is checked and is the desired motion system")]
            Is,

            /// <summary>
            /// 不是:检查运动系统，并且不是期望的运动系统时可触发事件
            /// </summary>
            [Name("不是")]
            [Tip("检查运动系统，并且不是期望的运动系统时可触发事件", "An event can be triggered when the motion system is checked and is not the desired motion system")]
            NotIs,

            /// <summary>
            /// 是已定义的:检查运动系统，并且是运动提供者组件中已定义的运动系统时可触发事件
            /// </summary>
            [Name("是已定义的")]
            [Tip("检查运动系统，并且是运动提供者组件中已定义的运动系统时可触发事件", "An event can be triggered when the motion system is checked and is a defined motion system in the motion provider component")]
            IsDefined,

            /// <summary>
            /// 是已定义的:检查运动系统，并且不是运动提供者组件中已定义的运动系统时可触发事件
            /// </summary>
            [Name("是已定义的")]
            [Tip("检查运动系统，并且不是运动提供者组件中已定义的运动系统时可触发事件", "An event can be triggered when the motion system is checked and is not a defined motion system in the motion provider component")]
            NotIsDefined,
        }
    }
}
