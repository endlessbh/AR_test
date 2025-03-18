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
    /// 观察者事件
    /// </summary>
    [Name(Title, nameof(ObserverBehaviourEvent))]
    [RequireManager(typeof(VuforiaManager))]
    [Owner(typeof(VuforiaManager))]
    public class ObserverBehaviourEvent : Trigger<ObserverBehaviourEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "观察者事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ObserverBehaviourEvent))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(VuforiaHelper.Title, typeof(VuforiaManager))]
        [StateComponentMenu(VuforiaHelper.Title + "/" + Title, typeof(VuforiaManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

#if XDREAMER_VUFORIA

        /// <summary>
        /// 观察者
        /// </summary>
        [Name("观察者")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ObserverBehaviour _observerBehaviour;

        private ObserverBehaviour _observerBehaviourRegisted;

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
            /// 当目标状态已更改
            /// </summary>
            [Name("当目标状态已更改")]
            OnTargetStatusChanged,

            /// <summary>
            /// 当行为已销毁
            /// </summary>
            [Name("当行为已销毁")]
            OnBehaviourDestroyed,
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.OnTargetStatusChanged;

        /// <summary>
        /// 检查目标状态规则
        /// </summary>
        [Name("检查目标状态规则")]
        public enum ECheckTargetStatusRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 当前状态
            /// </summary>
            [Name("当前状态")]
            CurrentStatus,

            /// <summary>
            /// 当前状态信息
            /// </summary>
            [Name("当前状态信息")]
            CurrentStatusInfo,

            /// <summary>
            /// 当前状态与状态信息
            /// </summary>
            [Name("当前状态与状态信息")]
            CurrentStatusAndStatusInfo,

            /// <summary>
            /// 当前状态或状态信息
            /// </summary>
            [Name("当前状态或状态信息")]
            CurrentStatusOrStatusInfo,
        }

        /// <summary>
        /// 检查目标状态规则
        /// </summary>
        [Name("检查目标状态规则")]
        [EnumPopup]
        public ECheckTargetStatusRule _checkTargetStatusRule = ECheckTargetStatusRule.None;

        /// <summary>
        /// 当前状态
        /// </summary>
        [Name("当前状态")]
        public Status _currentStatus = Status.TRACKED;

        /// <summary>
        /// 当前状态信息
        /// </summary>
        [Name("当前状态信息")]
        public StatusInfo _currentStatusInfo = StatusInfo.NORMAL;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            _observerBehaviourRegisted = _observerBehaviour;
            if (_observerBehaviourRegisted)
            {
                _observerBehaviourRegisted.OnTargetStatusChanged += OnTargetStatusChanged;
                _observerBehaviourRegisted.OnBehaviourDestroyed += OnBehaviourDestroyed;
            }
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            if (_observerBehaviourRegisted)
            {
                _observerBehaviourRegisted.OnTargetStatusChanged -= OnTargetStatusChanged;
                _observerBehaviourRegisted.OnBehaviourDestroyed -= OnBehaviourDestroyed;
            }
        }

        private void OnTargetStatusChanged(ObserverBehaviour observerBehaviour, TargetStatus targetStatus)
        {
            if (finished) return;
            if (observerBehaviour != _observerBehaviourRegisted) return;
            switch (_checkTargetStatusRule)
            {
                case ECheckTargetStatusRule.CurrentStatus:
                    {
                        finished = targetStatus.Status == _currentStatus;
                        break;
                    }
                case ECheckTargetStatusRule.CurrentStatusInfo:
                    {
                        finished = targetStatus.StatusInfo == _currentStatusInfo;
                        break;
                    }
                case ECheckTargetStatusRule.CurrentStatusAndStatusInfo:
                    {
                        finished = targetStatus.Status == _currentStatus && targetStatus.StatusInfo == _currentStatusInfo;
                        break;
                    }
                case ECheckTargetStatusRule.CurrentStatusOrStatusInfo:
                    {
                        finished = targetStatus.Status == _currentStatus || targetStatus.StatusInfo == _currentStatusInfo;
                        break;
                    }
                default:
                    {
                        finished = true;
                        break;
                    }
            }
        }

        private void OnBehaviourDestroyed(ObserverBehaviour observerBehaviour)
        {
            if (finished) return;
            if (observerBehaviour != _observerBehaviourRegisted) return;
            finished = true;
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _observerBehaviour;
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
