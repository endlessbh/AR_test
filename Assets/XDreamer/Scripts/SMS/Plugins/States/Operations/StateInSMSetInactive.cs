using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Operations
{
    /// <summary>
    /// 状态机中状态设置非激活:状态机中状态设置非激活组件是对某个状态由激活切换为非激活的执行体。组件执行完毕后切换为完成态。
    /// </summary>
    [ComponentMenu("状态操作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(StateInSMSetInactive))]
    [XCSJ.Attributes.Icon(index = 33662)]
    [Tip("状态机中状态设置非激活组件是对某个状态由激活切换为非激活的执行体。组件执行完毕后切换为完成态。", "The inactive component of state setting in the state machine is the actuator that switches a state from active to inactive. After the component is executed, it is switched to the completed state.")]
    public class StateInSMSetInactive : StateComponent<StateInSMSetInactive>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "状态机中状态设置非激活";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("状态操作", typeof(SMSManager))]
        [StateComponentMenu("状态操作/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(StateInSMSetInactive))]
        [Tip("激活状态非激活组件是对某个状态由激活切换为非激活的执行体。组件执行完毕后切换为完成态。", "An active or inactive component is an actuator that switches a state from active to inactive. After the component is executed, it is switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("子状态机/状态机")]
        [Tip("将子状态机/状态机中所有激活的状态设置为非激活;不可用于控制当前组件所在状态以及所在状态集的任何父级；", "Set all activated states in the sub state machine / state machine to inactive; It cannot be used to control the state of the current component and any parent of the state set;")]
        [StatePopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public SubStateMachine subStateMachine;

        [Name("等待非激活完成")]
        [Tip("将组件设置为非激活后因正常的状态执行逻辑,可能某些组件会重新激活;所以本参数项推荐不启用；检测时忽略任意状态；", "Set all activated states in the sub state machine / state machine to inactive; It cannot be used to control the state of the current component and any parent of the state set;")]
        public bool waitInactiveFinished = false;

        [Name("需要完全非激活")]
        public bool needFullInactive = false;

        private Dictionary<State, bool> states = new Dictionary<State, bool>();

        public override double progress
        {
            get
            {
                if (states.Count == 0 || finished) return 1;
                return base.progress = states.Count(s => !s.Key.isActive || s.Key is AnyState) * 1f / states.Count;
            }
            set
            {
                base.progress = value;
            }
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            states.Clear();
            if (subStateMachine && !IsChildOf(subStateMachine))
            {
                foreach (var s in subStateMachine.activeStates)
                {
                    if (s is AnyState) continue;
                    states.Add(s, true);
                    s.active = false;
                }
            }
            if (!waitInactiveFinished) finished = true;
        }

        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);
            if (finished) return;
            foreach (var kv in states)
            {
                var state = kv.Key;
                if (!state.isActive)
                {
                    states[state] = false;
                }
                else if (needFullInactive || kv.Value)
                {
                    return;
                }
            }
            finished = true;
        }

        public override bool DataValidity()
        {
            return subStateMachine;
        }

        public override string ToFriendlyString()
        {
            return subStateMachine ? subStateMachine.name : "";
        }
    }
}
