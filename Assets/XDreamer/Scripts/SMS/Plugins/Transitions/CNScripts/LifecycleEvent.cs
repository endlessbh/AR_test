using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.Transitions.CNScripts
{
    /// <summary>
    /// 生命周期事件
    /// </summary>
    [Serializable]
    [ComponentMenu("中文脚本/生命周期事件", typeof(SMSManager))]
    [Name("生命周期事件")]
    public class LifecycleEvent : TransitionScriptComponent<ELifecycleEvent, LifecycleEventFunction, LifecycleEventFunctionCollection>
    {
        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _funcCollection.EnsureFunctionExist(ELifecycleEvent.OnEntry);
            _funcCollection.EnsureFunctionExist(ELifecycleEvent.OnExit);
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
