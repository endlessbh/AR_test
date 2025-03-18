using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    [ComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairTaskWork))]
    [XCSJ.Attributes.Icon(index = 34486)]
    [UniqueComponent(EUnique.Hierarchy)]
    [DisallowMultipleComponent]
    [RequireState(typeof(SubStateMachine))]
    [RequireManager(typeof(RepairmanManager))]
    [Owner(typeof(RepairmanManager))]
    [Tip("修理任务组件是包含修理步骤组和修理步骤的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。所有的步骤组和步骤节点都应该放在修理任务内，构成一个集合体。修理任务可以通过拆装修理步骤树视图来呈现在界面上。只有内部包含的修理步骤组合修理步骤都完成了，组件才切换为完成态。", "The repair task component is a container that contains repair step groups and repair steps. It is realized by sub state machine. Is a data organization object in which data is provided to other state components for use. All step groups and step nodes should be placed in the repair task to form an aggregate. The repair task can be presented on the interface through the disassembly and repair step tree view. Only when the internal repair steps and combined repair steps are completed, the component can be switched to the completed state.")]
    public sealed class RepairTaskWork : StepGroupRoot
    {
        /// <summary>
        /// 名称
        /// </summary>
        public new const string Title = "拆装任务";

        [Name(Title, nameof(RepairTaskWork))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("修理任务组件是包含修理步骤组和修理步骤的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。所有的步骤组和步骤节点都应该放在修理任务内，构成一个集合体。修理任务可以通过拆装修理步骤树视图来呈现在界面上。只有内部包含的修理步骤组合修理步骤都完成了，组件才切换为完成态。", "The repair task component is a container that contains repair step groups and repair steps. It is realized by sub state machine. Is a data organization object in which data is provided to other state components for use. All step groups and step nodes should be placed in the repair task to form an aggregate. The repair task can be presented on the interface through the disassembly and repair step tree view. Only when the internal repair steps and combined repair steps are completed, the component can be switched to the completed state.")]
        public static State CreateRepairTaskWork(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(RepairTaskWork)), null, typeof(RepairTaskWork));
        }

        /// <summary>
        /// 当前任务内所有拆装步骤
        /// </summary>
        public List<RepairStep> repairSteps { get; private set; } = new List<RepairStep>();

        /// <summary>
        /// 步骤中所有零件
        /// </summary>
        public List<Part> parts { get; private set; } = new List<Part>();

        /// <summary>
        /// 步骤中所有模块
        /// </summary>
        public List<Module> modules { get; private set; } = new List<Module>();

        private void FindPartModuleStep()
        {
            parts.Clear();
            modules.Clear();
            repairSteps.Clear();

            foreach (var s in steps)
            {
                if (s is RepairStep step && step)
                {
                    repairSteps.Add(step);

                    step.selectedParts.ForEach(p =>
                    {
                        if (p is Module m && m)
                        {
                            if (modules.Contains(m))
                            {
                                Debug.Log(name + "的子步骤中包含相同的模块[" + m.showName + "], 模块详细路径：" + m.GetNamePath());
                            }
                            else
                            {
                                modules.Add(m);
                            }
                        }
                        else
                        {
                            if (parts.Contains(p))
                            {
                                Debug.Log(name + "的子步骤中包含相同的零件[" + p.showName + "], 零件详细路径：" + p.GetNamePath());
                            }
                            else
                            {
                                parts.Add(p);
                            }
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 包含零件
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public bool ContainPart(Part part)
        {
            return parts.Contains(part) || modules.Exists(m => m.ContainPart(part));
        }

        /// <summary>
        /// 预进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);

            FindPartModuleStep();
        }

        /// <summary>
        /// 后退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterExit(StateData stateData)
        {
            base.OnAfterExit(stateData);
        }

    }
}
