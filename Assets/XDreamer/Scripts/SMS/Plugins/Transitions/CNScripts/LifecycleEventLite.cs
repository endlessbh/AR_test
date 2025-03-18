using System;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;

namespace XCSJ.PluginSMS.Transitions.CNScripts
{
    [ComponentMenu("中文脚本/生命周期事件简版", typeof(SMSManager))]
    [Name("生命周期事件简版")]
    [Serializable]
    public class LifecycleEventLite : TransitionScriptComponent<ELifecycleEventLite, LifecycleEventLiteFunction, LifecycleEventLiteFunctionCollection>
    {
        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _funcCollection.EnsureFunctionExist(ELifecycleEventLite.OnEntry);
            _funcCollection.EnsureFunctionExist(ELifecycleEventLite.OnExit);
        }

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            ExecuteScriptEvent(ELifecycleEventLite.OnEntry);
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);
            ExecuteScriptEvent(ELifecycleEventLite.OnExit);
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="data"></param>
        public override void OnUpdate(StateData data)
        {
            base.OnUpdate(data);
            ExecuteScriptEvent(ELifecycleEventLite.OnUpdate);
        }
    }
}
