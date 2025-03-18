using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;
using XCSJ.PluginSMS.States.TimeLine;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    [ComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairStepGroup))]
    [XCSJ.Attributes.Icon(index = 34485)]
    [RequireComponent(typeof(TimeLinePlayContent))]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    [Owner(typeof(RepairmanManager))]
    [Tip("步骤组组件是包含多个步骤的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。修理步骤组是介于修理任务与修理步骤之间的中间层概念，修理步骤组可以嵌套修理步骤组。只有组内所有的步骤或步骤组都完成了，组件才切换为完成态。", "A step group component is a container that contains multiple steps. It is realized by sub state machine. Is a data organization object in which data is provided to other state components for use. Repair step group is an intermediate concept between repair task and repair step. Repair step group can nest repair step group. Only when all steps or step groups in the group are completed can the component switch to the completed state.")]
    public class RepairStepGroup : StepGroup
    {
        /// <summary>
        /// 名称
        /// </summary>
        public new const string Title = "拆装步骤组";

        [Name(Title, nameof(RepairStepGroup))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("步骤组组件是包含多个步骤的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。修理步骤组是介于修理任务与修理步骤之间的中间层概念，修理步骤组可以嵌套修理步骤组。只有组内所有的步骤或步骤组都完成了，组件才切换为完成态。", "A step group component is a container that contains multiple steps. It is realized by sub state machine. Is a data organization object in which data is provided to other state components for use. Repair step group is an intermediate concept between repair task and repair step. Repair step group can nest repair step group. Only when all steps or step groups in the group are completed can the component switch to the completed state.")]
        public static State CreateRepairStepGroup(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(RepairStepGroup)), null, typeof(RepairStepGroup));
        }
                             
        public override void OnClick()
        {
            if (RepairStep.isOnClick)
            {
                base.OnClick();
            }
        }

        public override string ToFriendlyString()
        {
            return InStepGroup() == false ? "需放置" + CommonFun.Name(typeof(RepairTaskWork)) + "内!" : "";
        }

        public virtual void Help() { }

    }
}
