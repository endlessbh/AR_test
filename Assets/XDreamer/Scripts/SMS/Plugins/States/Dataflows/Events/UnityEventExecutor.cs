using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.Events
{
    /// <summary>
    /// Unity事件执行器:Unity事件执行器功能主要是调用函数。组件执行完毕后切换为完成态
    /// </summary>
    [ComponentMenu(DataflowHelper.Events + "/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(UnityEventExecutor))]
    [Tip("Unity事件执行器功能主要是调用函数。组件执行完毕后切换为完成态", "The function of unity event executor is mainly to call functions. Switch to the completed state after the component is executed")]
    [XCSJ.Attributes.Icon(EIcon.Function)]
    public class UnityEventExecutor : LifecycleExecutor<UnityEventExecutor>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Unity事件执行器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(DataflowHelper.Events, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.Events + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(UnityEventExecutor))]
        [Tip("Unity事件执行器功能主要是调用函数。组件执行完毕后切换为完成态", "The function of unity event executor is mainly to call functions. Switch to the completed state after the component is executed")]
        [XCSJ.Attributes.Icon(EIcon.Function)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// Unity事件:指定Unity对象的执行函数
        /// </summary>
        [Name("Unity事件")]
        [Tip("指定Unity对象的执行函数", "Specifies the execution function of the unity object")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UnityEngine.Events.UnityEvent unityEvent = new UnityEvent();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data"></param>
        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            unityEvent?.Invoke();
        }
    }
}
