using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;

namespace XCSJ.PluginSMS.States.CNScripts
{
    /// <summary>
    /// 生命周期事件:生命周期事件组件是响应状态生命周期发生的事件并运行中文脚本的执行体。事件包括预进入、进入、已进入、更新、预已退、退出和已退出，组件激活后即刻切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(LifecycleEvent))]
    [Tip("生命周期事件组件是响应状态生命周期发生的事件并运行中文脚本的执行体。事件包括预进入、进入、已进入、更新、预已退、退出和已退出，组件激活后即刻切换为完成态。", "The lifecycle event component is an executive body that responds to events occurring in the state lifecycle and runs Chinese scripts. Events include pre entry, entry, entered, update, pre retired, exit and exited. The component will switch to the completed state immediately after activation.")]
    [XCSJ.Attributes.Icon(EIcon.CNScript)]
    public class LifecycleEvent : StateScriptComponent<LifecycleEvent, ELifecycleEvent, LifecycleEventFunction, LifecycleEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "生命周期事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.CNScriptCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.CNScriptCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(LifecycleEvent))]
        [Tip("生命周期事件组件是响应状态生命周期发生的事件并运行中文脚本的执行体。事件包括预进入、进入、已进入、更新、预已退、退出和已退出，组件激活后即刻切换为完成态。", "The lifecycle event component is an executive body that responds to events occurring in the state lifecycle and runs Chinese scripts. Events include pre entry, entry, entered, update, pre retired, exit and exited. The component will switch to the completed state immediately after activation.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _funcCollection.EnsureFunctionExist(ELifecycleEvent.OnEntry).Enable = true;
            _funcCollection.EnsureFunctionExist(ELifecycleEvent.OnExit).Enable = true;
        }

        /// <summary>
        /// 当进入之前
        /// </summary>
        /// <param name="data"></param>
        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);
            ExecuteScriptEvent(ELifecycleEvent.OnBeforeEntry);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            ExecuteScriptEvent(ELifecycleEvent.OnEntry);
        }

        /// <summary>
        /// 当进入之后
        /// </summary>
        /// <param name="data"></param>
        public override void OnAfterEntry(StateData data)
        {
            base.OnAfterEntry(data);
            ExecuteScriptEvent(ELifecycleEvent.OnAfterEntry);
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);
            ExecuteScriptEvent(ELifecycleEvent.OnUpdate);
        }

        /// <summary>
        /// 当退出之前
        /// </summary>
        /// <param name="data"></param>
        public override void OnBeforeExit(StateData data)
        {
            base.OnBeforeExit(data);
            ExecuteScriptEvent(ELifecycleEvent.OnBeforeExit);
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);
            ExecuteScriptEvent(ELifecycleEvent.OnExit);
        }

        /// <summary>
        /// 当退出之后
        /// </summary>
        /// <param name="data"></param>
        public override void OnAfterExit(StateData data)
        {
            base.OnAfterExit(data);
            ExecuteScriptEvent(ELifecycleEvent.OnAfterExit);
        }
    }
}
