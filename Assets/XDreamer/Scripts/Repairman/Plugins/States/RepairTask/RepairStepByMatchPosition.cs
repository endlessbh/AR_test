using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginTools.GameObjects;
using static XCSJ.PluginSMS.States.Show.Step;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    /// <summary>
    /// 拆装步骤通过匹配位置：使用步骤中的零件生成对应的匹配槽对象，使用拖拽工具将所有零件拖拽至与槽重合的位置时，认为步骤完成
    /// </summary>
    [ComponentMenu(RepairmanHelperExtension.StepStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairStepByMatchPosition))]
    [XCSJ.Attributes.Icon(EIcon.Step)]
    [DisallowMultipleComponent]
    [Tip("使用步骤中的零件生成对应的匹配槽对象，使用拖拽工具将所有零件拖拽至与槽重合的位置时，认为步骤完成", "Use the parts in the step to generate the corresponding matching slot object. When all parts are dragged to the position coincident with the slot with the drag tool, the step is considered to be completed")]
    [RequireComponent(typeof(RepairStep))]
    [Owner(typeof(RepairmanManager))]
    public class RepairStepByMatchPosition : StateComponent<RepairStepByMatchPosition>, ITrigger
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "拆装步骤通过位置匹配";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Name(Title, nameof(RepairStepByMatchPosition))]
        [Tip("使用步骤中的零件生成对应的匹配槽对象，使用拖拽工具将所有零件拖拽至与槽重合的位置时，认为步骤完成", "Use the parts in the step to generate the corresponding matching slot object. When all parts are dragged to the position coincident with the slot with the drag tool, the step is considered to be completed")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj)
        {
            return obj?.CreateNormalState(CommonFun.Name(typeof(RepairStepByMatchPosition)), null, typeof(RepairStepByMatchPosition));
        }

        /// <summary>
        /// 关联的修理步骤
        /// </summary>
        public RepairStep repairStep
        {
            get
            {
                if (!_repairStep)
                {
                    _repairStep = GetComponent<RepairStep>();
                }
                return _repairStep;
            }
        }
        private RepairStep _repairStep;

        /// <summary>
        /// 预进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnBeforeEntry(StateData stateData)
        {
            base.OnBeforeEntry(stateData);

            repairStep._finishRule = EFinishRule.ExtensionCondition;
            repairStep.extensionFinishCondition += FinishCondition;
        }

        /// <summary>
        /// 预退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnAfterExit(StateData stateData)
        {
            base.OnAfterExit(stateData);

            repairStep.extensionFinishCondition -= FinishCondition;
        }

        private bool FinishCondition() => DataValidity() && repairStep.selectedParts.All(s => s.assembleState == EAssembleState.Assembled);

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => FinishCondition();

        /// <summary>
        /// 有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => repairStep.selectedParts.Count > 0;
    }
}
