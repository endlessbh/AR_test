using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.PluginSMS.Kernel;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.TimeLine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 计划:计划组件是步骤和步骤组的容器。依附于子状态机，由若干步骤和步骤组构成，可与播放内容相关联，进行联合播放，组件激活后为完成态。
    /// </summary>
    [ComponentMenu("展示/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(Plan))]
    [Tip("计划组件是步骤和步骤组的容器。依附于子状态机，由若干步骤和步骤组构成，可与播放内容相关联，进行联合播放，组件激活后为完成态。", "A schedule component is a container for steps and step groups. Attached to the sub state machine, it is composed of several steps and step groups, which can be associated with the playback content for joint playback. After the component is activated, it is in the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Task)]
    [RequireState(typeof(SubStateMachine))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimeLinePlayContent))]
    [UniqueComponent(EUnique.Hierarchy)]
    public sealed class Plan : StepGroupRoot
    {
        /// <summary>
        /// 标题
        /// </summary>
        public new const string Title = "计划";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("展示", typeof(SMSManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu("展示/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(Plan))]
        [Tip("计划组件是步骤和步骤组的容器。依附于子状态机，由若干步骤和步骤组构成，可与播放内容相关联，进行联合播放，组件激活后为完成态。", "A schedule component is a container for steps and step groups. Attached to the sub state machine, it is composed of several steps and step groups, which can be associated with the playback content for joint playback. After the component is activated, it is in the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreatePlan(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(Plan)), null, typeof(Plan));
        }

        public override void OnCreated()
        {
            base.OnCreated();

            timeLinePlayContent = GetComponent<TimeLinePlayContent>();
        }
    }
}
