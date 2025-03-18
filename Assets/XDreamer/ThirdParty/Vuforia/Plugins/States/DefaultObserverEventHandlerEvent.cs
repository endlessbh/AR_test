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
    /// 默认观察者事件处理器事件
    /// </summary>
    [Name(Title, nameof(DefaultObserverEventHandlerEvent))]
    [RequireManager(typeof(VuforiaManager))]
    [Owner(typeof(VuforiaManager))]
    public class DefaultObserverEventHandlerEvent : Trigger<DefaultObserverEventHandlerEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "默认观察者事件处理器事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(DefaultObserverEventHandlerEvent))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(VuforiaHelper.Title, typeof(VuforiaManager))]
        [StateComponentMenu(VuforiaHelper.Title + "/" + Title, typeof(VuforiaManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);


#if XDREAMER_VUFORIA

        /// <summary>
        /// 默认观察者事件处理器
        /// </summary>
        [Name("默认观察者事件处理器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public DefaultObserverEventHandler _defaultObserverEventHandler;

        private DefaultObserverEventHandler _defaultObserverEventHandlerRegisted;

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
            /// 当目标找到
            /// </summary>
            [Name("当目标找到")]
            OnTargetFound,

            /// <summary>
            /// 当目标丢失
            /// </summary>
            [Name("当目标丢失")]
            OnTargetLost,
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.OnTargetFound;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            _defaultObserverEventHandlerRegisted = _defaultObserverEventHandler;
            if (_defaultObserverEventHandlerRegisted)
            {
                _defaultObserverEventHandlerRegisted.OnTargetFound.AddListener(OnTargetFound);
                _defaultObserverEventHandlerRegisted.OnTargetLost.AddListener(OnTargetLost);
            }
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            if (_defaultObserverEventHandlerRegisted)
            {
                _defaultObserverEventHandlerRegisted.OnTargetFound.RemoveListener(OnTargetFound);
                _defaultObserverEventHandlerRegisted.OnTargetLost.RemoveListener(OnTargetLost);
            }
        }

        private void OnTargetFound()
        {
            if (finished) return;
            if (_eventType != EEventType.OnTargetFound) return;

            finished = true;
        }

        private void OnTargetLost()
        {
            if (finished) return;
            if (_eventType != EEventType.OnTargetLost) return;

            finished = true;
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _defaultObserverEventHandler;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _eventType.Tr();
        }

#endif
    }
}
