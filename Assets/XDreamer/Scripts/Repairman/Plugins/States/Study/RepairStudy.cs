using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States.Study
{
    [ComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairStudy))]
    [XCSJ.Attributes.Icon(EIcon.Study)]
    [Tip("拆装学习组件是在拆装任务流程的基础上进行学习的对象。在学习模式下，可使用提示功能，提醒用户所需完成操作。辅助用户一步步的完成拆装流程。所有拆装修理步骤完成，组件切换为完成态。", "Disassembly and assembly learning component is the object of learning based on the disassembly and assembly task process. In the learning mode, the prompt function can be used to remind the user of the required operations. Assist users to complete the disassembly process step by step. All disassembly and repair steps are completed, and the components are switched to the completed state.")]
    public class RepairStudy : RepairGuide<RepairStudy>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "拆装学习";

        [Name(Title, nameof(RepairStudy))]
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("拆装学习组件是在拆装任务流程的基础上进行学习的对象。在学习模式下，可使用提示功能，提醒用户所需完成操作。辅助用户一步步的完成拆装流程。所有拆装修理步骤完成，组件切换为完成态。", "Disassembly and assembly learning component is the object of learning based on the disassembly and assembly task process. In the learning mode, the prompt function can be used to remind the user of the required operations. Assist users to complete the disassembly process step by step. All disassembly and repair steps are completed, and the components are switched to the completed state.")]
        public static State CreateRepairStudy(IGetStateCollection obj) => CreateNormalState(obj);

        public event Action<GameObject, bool> onPartSelected;

        public event Action<PluginRepairman.States.Tool, bool> onToolSelected;

        /// <summary>
        /// 提示
        /// </summary>
        public void Help()
        {
            GetCurrentStep()?.Help();
        }

        /// <summary>
        /// 零件选择回调
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected override void OnSelectionPartChanged(GameObject[] oldSelections, bool flag)
        {
            base.OnSelectionPartChanged(oldSelections, flag);

            if (onPartSelected == null) return;

            if (Selection.selections.Length == 0)
            {
                onPartSelected.Invoke(null, false);
            }
            else
            {
                foreach (var go in Selection.selections)
                {
                    if (go)
                    {
                        onPartSelected.Invoke(go, IsCurrentActiveTaskPart(go));
                    }
                }
            }
        }

        /// <summary>
        /// 工具选择回调
        /// </summary>
        /// <param name="oldSelections"></param>
        /// <param name="flag"></param>
        protected override void OnSelectionToolChanged(ITool[] oldSelections, bool flag)
        {
            base.OnSelectionToolChanged(oldSelections, flag);

            if (onToolSelected == null) return;

            if (ToolSelection.selections.Length == 0)
            {
                onToolSelected.Invoke(null, false);
            }
            else
            {
                foreach (var t in ToolSelection.selections)
                {
                    if (t is PluginRepairman.States.Tool tool)
                    {
                        onToolSelected.Invoke(tool, IsCurrentActiveTaskTool(tool));
                    }
                }
            }
        }

    }
}
