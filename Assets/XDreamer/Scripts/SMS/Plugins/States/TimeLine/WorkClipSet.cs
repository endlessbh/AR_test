using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.TimeLine
{
    /// <summary>
    /// 工作剪辑集合：用于管理当前普通状态上状态组件(工作剪辑型)的工作剪辑执行问题;可防止状态组件的工作剪辑产生冲突;
    /// </summary>
    [ComponentMenu("时间轴/工作剪辑集合", typeof(SMSManager))]
    [Name("工作剪辑集合", nameof(WorkClipSet))]
    [Tip("用于管理当前普通状态上状态组件(工作剪辑型)的工作剪辑执行问题;可防止状态组件的工作剪辑产生冲突;", "It is used to manage the work clip execution problem of the state component (work clip type) in the current general state; It can prevent the working clips of state components from conflict;")]
    [XCSJ.Attributes.Icon(index = 33660)]
    [DisallowMultipleComponent]
    [RequireState(typeof(NormalState))]
    public class WorkClipSet : StateComponent<WorkClipSet>
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("时间轴", typeof(SMSManager))]
        [StateComponentMenu("时间轴/工作剪辑集合", typeof(SMSManager))]
#endif
        [Name("工作剪辑集合", nameof(WorkClipSet))]
        [Tip("用于管理当前普通状态上状态组件(工作剪辑型)的工作剪辑执行问题;可防止状态组件的工作剪辑产生冲突;", "It is used to manage the work clip execution problem of the state component (work clip type) in the current general state; It can prevent the working clips of state components from conflict;")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;
    }
}

