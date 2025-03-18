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
    [Name(Title, nameof(RepairTaskPlay))]
    [XCSJ.Attributes.Icon(EIcon.Play)]
    [Tip("拆装演示组件是时间轴播放器播放拆装任务时，辅助启用工具组件和让步骤列表可点击的工具", "The disassembly and assembly demonstration component is a tool to enable the auxiliary tool component and make the step list clickable when the timeline player plays the disassembly and assembly task")]
    [Owner(typeof(RepairmanManager))]
    public class RepairTaskPlay : RepairGuide<RepairTaskPlay>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "拆装演示";

        [Name(Title, nameof(RepairTaskPlay))]
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("拆装演示组件是时间轴播放器播放拆装任务时，辅助启用工具组件和让步骤列表可点击的工具", "The disassembly and assembly demonstration component is a tool to enable the auxiliary tool component and make the step list clickable when the timeline player plays the disassembly and assembly task")]
        public static State CreateRepairStudy(IGetStateCollection obj) => CreateNormalState(obj);

        private bool orgRepairStepIsOnClick = false;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            RepairStep.autoSelectPartAndTool = true;

            orgRepairStepIsOnClick = RepairStep.isOnClick;
            RepairStep.isOnClick = true;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            RepairStep.isOnClick = orgRepairStepIsOnClick;
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished()
        {
            return base.Finished();
        }
    }
}
