using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Languages;

#if XDREAMER_VUFORIA
using Vuforia;
#endif

namespace XCSJ.PluginVuforia.States
{
    /// <summary>
    /// 虚拟按钮事件
    /// </summary>
    [Name(Title, nameof(VirtualButtonBehaviourEvent))]
    [RequireManager(typeof(VuforiaManager))]
    [Owner(typeof(VuforiaManager))]
    public class VirtualButtonBehaviourEvent : Trigger<VirtualButtonBehaviourEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "虚拟按钮事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(VirtualButtonBehaviourEvent))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(VuforiaHelper.Title, typeof(VuforiaManager))]
        [StateComponentMenu(VuforiaHelper.Title + "/" + Title, typeof(VuforiaManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_VUFORIA

        /// <summary>
        /// 虚拟按钮
        /// </summary>
        [Name("虚拟按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public VirtualButtonBehaviour _virtualButtonBehaviour;

        private VirtualButtonBehaviour _virtualButtonBehaviourRegisted;

        /// <summary>
        /// 事件类型
        /// </summary>
        public enum EEventType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 当按钮按压
            /// </summary>
            [Name("当按钮按压")]
            OnButtonPressed,

            /// <summary>
            /// 当按钮释放
            /// </summary>
            [Name("当按钮释放")]
            OnButtonReleased,
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.OnButtonPressed;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            _virtualButtonBehaviourRegisted = _virtualButtonBehaviour;
            if (_virtualButtonBehaviourRegisted)
            {
                _virtualButtonBehaviourRegisted.RegisterOnButtonPressed(OnButtonPressed);
                _virtualButtonBehaviourRegisted.RegisterOnButtonReleased(OnButtonReleased);
            }
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            if (_virtualButtonBehaviourRegisted)
            {
                _virtualButtonBehaviourRegisted.UnregisterOnButtonPressed(OnButtonPressed);
                _virtualButtonBehaviourRegisted.UnregisterOnButtonReleased(OnButtonReleased);
            }
        }

        private void OnButtonPressed(VirtualButtonBehaviour vb)
        {
            if (finished) return;
            if (_eventType != EEventType.OnButtonPressed) return;
            if (vb != _virtualButtonBehaviourRegisted) return;

            finished = true;
        }

        private void OnButtonReleased(VirtualButtonBehaviour vb)
        {
            if (finished) return;
            if (_eventType != EEventType.OnButtonReleased) return;
            if (vb != _virtualButtonBehaviourRegisted) return;

            finished = true;
        }

#endif
    }
}
