using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.States.Operations
{
    /// <summary>
    /// 状态激活:状态激活组件是对某个状态执行激活的执行体，组件执行完毕后切换为完成态。
    /// </summary>
    [ComponentMenu("状态操作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(StateActive))]
    [XCSJ.Attributes.Icon(index = 33661)]
    [Tip("状态激活组件是对某个状态执行激活的执行体，组件执行完毕后切换为完成态。", "The status activation component is the actuator that activates a certain status. After the component is executed, it switches to the completed state.")]
    public class StateActive : LifecycleExecutor<StateActive>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "状态激活";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("状态操作", typeof(SMSManager))]
        [StateComponentMenu("状态操作/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(StateActive))]
        [Tip("状态激活组件是对某个状态执行激活的执行体，组件执行完毕后切换为完成态。", "The status activation component is the actuator that activates a certain status. After the component is executed, it switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 状态:将状态/子状态机/状态机设置为激活;不可用于控制当前组件所在状态以及所在状态集的任何父级；
        /// </summary>
        [Name("状态")]
        [Tip("将状态/子状态机/状态机设置为激活;不可用于控制当前组件所在状态以及所在状态集的任何父级；", "Set the state / sub state machine / state machine to active; It cannot be used to control the state of the current component and any parent of the state set;")]
        [StatePopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public State state;

        /// <summary>
        /// 激活:如果期望激活的状态所在父级状态(子状态机/状态机）处于非激活态，那么状态不会激活；如期望激活，请依次手动将所有父级状态激活；
        /// </summary>
        [Name("激活")]
        [Tip("如果期望激活的状态所在父级状态(子状态机/状态机）处于非激活态，那么状态不会激活；如期望激活，请依次手动将所有父级状态激活；", "If the parent state (child state machine / state machine) of the desired activated state is inactive, the state will not be activated; Activate as expected, please manually activate all parent states in turn;")]
        [EnumPopup]
        public EBool active = EBool.Yes;

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            if (state && !IsChildOf(state))
            {
                state.active = CommonFun.BoolChange(state.active, active);
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return state;
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            if (state)
            {
                return string.Format("{0}:{1}", state.name, CommonFun.Name(active));
            }
            return "";
        }
    }
}
